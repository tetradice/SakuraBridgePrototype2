using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

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
                    Module.Load(asm.Location);

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
            var message = Marshal.PtrToStringAnsi(messagePtr);    // IntPtrをstring型に変換
            var len = Marshal.ReadInt32(lenPtr);  // メッセージ長を読み取る

            // 受け取った文字列のハンドルを解放
            Marshal.FreeHGlobal(messagePtr);

            // ModuleのRequest処理を呼び出す
            string resStr = Module.Request(message);

            // responseを返す
            Marshal.WriteInt32(lenPtr, resStr.Length);
            return Marshal.StringToHGlobalAnsi(resStr);
        }
    }
}
