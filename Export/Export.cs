using SakuraBridge.Library;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SakuraBridge.Export
{
    /// <summary>
    /// ベースウェアから直接呼び出されるDLL関数のエクスポート
    /// </summary>
    public static class Export
    {
        /// <summary>
        /// .NETアセンブリから読み込んだSakuraBridgeモジュール
        /// </summary>
        public static IModule Module;

        /// <summary>
        /// staticコンストラクタ
        /// </summary>
        static Export()
        {
            // アセンブリ解決処理を追加
            // (クラスライブラリとして外部から呼び出されるため、この処理を加えないとアセンブリがどこにあるのかを特定できない)
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        /// <summary>
        /// アセンブリ解決処理
        /// </summary>
        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name.Split(',')[0];
            var assemblyPath = Path.Combine(Path.GetDirectoryName(args.RequestingAssembly.Location), string.Format("{0}.dll", assemblyName));
            return Assembly.LoadFrom(assemblyPath);
        }

        #region エクスポート関数

        [DllExport]
        public static bool load(IntPtr dllDirPathPtr, int len)
        {
            var dllDirPath = Marshal.PtrToStringAnsi(dllDirPathPtr);    // IntPtrをstring型に変換

            // 受け取った文字列のハンドルを解放
            Marshal.FreeHGlobal(dllDirPathPtr);

            // SakuraBridge.txtを読み込む
            var lines = File.ReadAllLines(Path.Combine(dllDirPath, @"SakuraBridge.txt"));
            var dllName = "module.dll";
            foreach(var line in lines)
            {
                if (line.StartsWith("maindll,"))
                {
                    dllName = line.Split(',')[1];
                    break;
                }
            }

            // モジュールを読み込む
            var iModuleName = typeof(IModule).FullName;
            var asm = Assembly.LoadFrom(Path.Combine(dllDirPath, dllName));
            foreach (var type in asm.GetTypes())
            {
                // SakuraBridge.Library.IModule 型を実装したクラス1つを探す
                if (type.GetInterface(iModuleName) != null)
                {
                    Module = (IModule)asm.CreateInstance(type.FullName);

                    // ModuleのLoad処理を呼び出す
                    Module.Load(Path.GetDirectoryName(asm.Location));

                    break;
                }
            }

            return true; // 正常終了
        }

        [DllExport]
        public static bool unload()
        {
            // ModuleのUnload処理を呼び出す
            Module.Unload();

            return true; // 正常終了
        }

        [DllExport]
        public static IntPtr request([MarshalAs(UnmanagedType.HString)]IntPtr messagePtr, IntPtr lenPtr)
        {
            // メッセージ長を読み取る
            var len = Marshal.ReadInt32(lenPtr);  

            // メッセージの内容をbyte配列に格納
            var messageBytes = new byte[len];
            Marshal.Copy(messagePtr, messageBytes, 0, len);

            // メッセージの内容をUTF-8と解釈して文字列に変換
            var message = Encoding.UTF8.GetString(messageBytes);

            // 受け取った文字列のハンドルを解放
            Marshal.FreeHGlobal(messagePtr);

            // ModuleのRequest処理を呼び出す
            string resStr = Module.Request(message);

            // バイト配列に変換
            var resBytes = Encoding.UTF8.GetBytes(resStr);

            // レスポンス文字列の領域を確保
            var resPtr = Marshal.AllocHGlobal(resBytes.Length);

            // レスポンス文字列をコピー
            Marshal.Copy(resBytes, 0, resPtr, resBytes.Length);

            // responseを返す
            Marshal.WriteInt32(lenPtr, resBytes.Length);
            return resPtr;
        }

        #endregion


    }
}
