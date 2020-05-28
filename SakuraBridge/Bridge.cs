using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace SakuraBridge
{
    public class Bridge
    {
        [DllExport]
        public static bool load(IntPtr dllDirPathPtr, int len)
        {
            var dllDirPath = Marshal.PtrToStringAnsi(dllDirPathPtr);    // IntPtrをstring型に変換

            // 受け取った文字列のハンドルを解放
            Marshal.FreeHGlobal(dllDirPathPtr);

            return true; // 正常終了
        }

        [DllExport]
        public static bool unload()
        {
            return true; // 正常終了
        }

        [DllExport]
        public static IntPtr request(IntPtr messagePtr, IntPtr lenPtr)
        {
            var message = Marshal.PtrToStringAnsi(messagePtr);    // IntPtrをstring型に変換
            var len = Marshal.ReadInt32(lenPtr);  // メッセージ長を読み取る

            // 受け取った文字列のハンドルを解放
            Marshal.FreeHGlobal(messagePtr);

            // responseを返す
            var resStr = "PLUGIN/2.0 200 OK\r\n\r\n";
            Marshal.WriteInt32(lenPtr, resStr.Length);
            return Marshal.StringToHGlobalAnsi(resStr);
        }
    }
}
