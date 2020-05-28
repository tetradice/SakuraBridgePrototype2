using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace SakuraBridge
{
    public class Bridge
    {
        [DllExport]
        public static bool load(IntPtr dllDirPathPtr, long len)
        {
            var dllDirPath = Marshal.PtrToStringAuto(dllDirPathPtr);    // IntPtrをstring型に変換
            return true; // 正常終了
        }

        [DllExport]
        public static bool unload()
        {
            return true; // 正常終了
        }

        [DllExport]
        public static IntPtr request(IntPtr messagePtr, long len)
        {
            var message = Marshal.PtrToStringAuto(messagePtr);    // IntPtrをstring型に変換

            var res = "SHIORI/2.0 200 OK";

            return Marshal.StringToHGlobalAuto(res); // stringをHGLOBALに変換
        }
    }
}
