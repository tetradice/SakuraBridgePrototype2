using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SakuraBridge.EntryPoint
{
    public class EntryPoint
    {
        [DllExport]
        public static bool load(IntPtr dllDirPathPtr, int len)
        {
            var dllDirPath = Marshal.PtrToStringAnsi(dllDirPathPtr);    // IntPtrをstring型に変換

            // 受け取った文字列のハンドルを解放
            Marshal.FreeHGlobal(dllDirPathPtr);

            // IClient型の名前を取得
            var iClientName = typeof(Shared.IClient).FullName;

            // クライアントのLoadメソッドを実行
            var asm = Assembly.LoadFrom(Path.Combine(dllDirPath, @"..\BridgeTest.dll"));
            foreach (var type in asm.GetTypes())
            {
                // IClient型を実装したクラスを探す
                if(type.GetInterface(iClientName) != null)
                {
                    var t1 = typeof(Shared.IClient);
                    var t2 = asm.GetType(iClientName);
                    var client = (Shared.IClient)asm.CreateInstance(type.FullName);
                    Debug.WriteLine(client);
                }
            }

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
