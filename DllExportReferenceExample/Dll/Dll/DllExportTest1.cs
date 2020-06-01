using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Dll
{
    public static class DllExportTest1
    {
        static DllExportTest1() { 
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        { 
            return Assembly.LoadFrom($@"{new FileInfo(args.RequestingAssembly.Location).DirectoryName}\{args.Name.Split(',')[0]}.dll");
        }


        [DllExport]
        public static bool load(IntPtr dllDirPathPtr, int len)
        {
            var util = new Others.Util();

            return true;
        }
        [DllExport]
        public static bool unload()
        {
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
            string resStr = "PLUGIN/2.0 200 OK\r\n\r\n";

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
