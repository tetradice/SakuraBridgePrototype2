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
        public static dynamic Module;

        [DllExport]
        public static bool load(IntPtr dllDirPathPtr, int len)
        {
            var dllDirPath = Marshal.PtrToStringAnsi(dllDirPathPtr);    // IntPtrをstring型に変換

            // 受け取った文字列のハンドルを解放
            Marshal.FreeHGlobal(dllDirPathPtr);

            // モジュールを読み込む
            var asm = Assembly.LoadFrom(Path.Combine(dllDirPath, @"..\BridgeTest.dll"));
            foreach (var type in asm.GetTypes())
            {
                // SakuraBridge.Library.IModule 型を実装したクラス1つを探す
                // (DllExportでは通常のプロジェクト参照は動作しない？ ように思われるため、型名を直接指定してdynamic型で処理)
                if (type.GetInterface("SakuraBridge.Library.IModule") != null)
                {
                    Module = asm.CreateInstance(type.FullName);

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
        public static IntPtr request(IntPtr messagePtr, IntPtr lenPtr)
        {
            // var message = Marshal.PtrToStringAnsi(messagePtr);    // IntPtrをstring型に変換

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

            // 文字列の領域を確保
            var resPtr = Marshal.AllocHGlobal(resBytes.Length);

            // 文字列をコピー
            Marshal.Copy(resBytes, 0, resPtr, resBytes.Length);

            // responseを返す
            Marshal.WriteInt32(lenPtr, resBytes.Length);
            return resPtr;
        }
    }
}
