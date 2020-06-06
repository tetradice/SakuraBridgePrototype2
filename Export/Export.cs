using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using SakuraBridge.Base;

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
        /// モジュールを実行するアプリケーションドメイン
        /// </summary>
        public static AppDomain ModuleDomain;

        /// <summary>
        /// Charsetヘッダのマッチ用正規表現
        /// </summary>
        public static Regex CharsetHeaderPattern;

        /// <summary>
        /// staticコンストラクタ
        /// </summary>
        static Export()
        {
            // アセンブリ解決処理を追加
            // (クラスライブラリとして外部から呼び出されるため、この処理を加えないとアセンブリがどこにあるのかを特定できない)
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            // 正規表現の初期化
            CharsetHeaderPattern = new Regex(@"^Charset\s*:\s*(.+?)$", RegexOptions.Multiline | RegexOptions.Compiled);
        }

        /// <summary>
        /// アセンブリ解決処理
        /// </summary>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name.Split(',')[0];
            var assemblyPath = Path.Combine(Path.GetDirectoryName(args.RequestingAssembly.Location), "module", string.Format("{0}.dll", assemblyName));
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
            var lines = File.ReadAllLines(Path.Combine(dllDirPath, @"bridge.txt"));
            string dllName = null;
            foreach (var line in lines)
            {
                if (line.StartsWith("module,"))
                {
                    dllName = line.Split(',')[1];
                }
            }
            if (dllName == null)
            {
                throw new Exception("bridge.txt 内でmodule dll名が指定されていません。");
            }

            // モジュールを読み込むための新しいアプリケーションドメインを生成
            ModuleDomain = AppDomain.CreateDomain("ModuleDomain");

            // アプリケーションドメイン内でアセンブリを読み込み、IModuleを継承したモジュールクラスを探す
            var dllPath = Path.Combine(dllDirPath, "module", dllName);
            var baseLibPath = Path.Combine(dllDirPath, @"module\SakuraBridge.Base.dll");
            var finder = (IModuleClassFinder)ModuleDomain.CreateInstanceFromAndUnwrap(baseLibPath, typeof(ModuleClassFinder).FullName);
            var moduleClassNames = finder.GetModuleClassFullNames(dllPath);

            // モジュールクラスが1つもない、もしくは2つ以上ある場合はエラー
            if (moduleClassNames.Length == 0)
            {
                throw new Exception(string.Format("アセンブリ内にIModuleを継承したクラスが含まれていません。 - {0}", dllName));
            }
            if (moduleClassNames.Length >= 2)
            {
                throw new Exception(string.Format("アセンブリ内にIModuleを継承したクラスが2つ以上含まれています。 - {0}", dllName));
            }

            // モジュールクラスのインスタンスを作成
            Module = (IModule)ModuleDomain.CreateInstanceFromAndUnwrap(dllPath, moduleClassNames.Single());

            // ModuleのLoad処理を呼び出す
            Module.Load(dllDirPath);

            return true; // 正常終了
        }

        [DllExport]
        public static bool unload()
        {
            // ModuleのUnload処理を呼び出す
            Module.Unload();

            // モジュールのアプリケーションドメインを開放
            AppDomain.Unload(ModuleDomain);

            return true; // 正常終了
        }

        [DllExport]
        public static IntPtr request(IntPtr messagePtr, IntPtr lenPtr)
        {
            // メッセージ長を読み取る
            var len = Marshal.ReadInt32(lenPtr);

            // メッセージの内容をbyte配列に格納
            var messageBytes = new byte[len];
            Marshal.Copy(messagePtr, messageBytes, 0, len);

            // 受け取った文字列のハンドルを解放
            Marshal.FreeHGlobal(messagePtr);

            // リクエストメッセージの内容をUTF-8と解釈して文字列に変換
            var reqStr = Encoding.UTF8.GetString(messageBytes);

            // リクエストメッセージの中にCharset指定があり、かつUTF8でない場合、そのCharset指定を使って読み込み直す
            var reqEnc = GetEncodingFromMessage(reqStr);
            if (reqEnc != Encoding.UTF8)
            {
                reqStr = reqEnc.GetString(messageBytes);
            }

            // ModuleのRequest処理を呼び出す
            var resStr = Module.Request(reqStr);

            // レスポンス文字列をバイト配列に変換
            // (レスポンスの中にCharset指定があれば、そのエンコーディングを使う)
            var resEnc = GetEncodingFromMessage(resStr);
            var resBytes = resEnc.GetBytes(resStr);

            // レスポンス文字列の領域を確保
            var resPtr = Marshal.AllocHGlobal(resBytes.Length);

            // レスポンス文字列をコピー
            Marshal.Copy(resBytes, 0, resPtr, resBytes.Length);

            // responseを返す
            Marshal.WriteInt32(lenPtr, resBytes.Length);
            return resPtr;
        }

        #endregion

        #region その他の関数

        /// <summary>
        /// メッセージ文字列の中からCharsetを探し、そのCharsetヘッダの値を元にエンコーディングを取得。未指定の場合や解釈できないCharset値の場合はUTF-8を返す
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Encoding GetEncodingFromMessage(string message)
        {
            // Charsetヘッダの値を探す
            var matched = CharsetHeaderPattern.Match(message);
            if (matched.Success)
            {
                var charsetValue = matched.Groups[1].Value.TrimEnd();
                try
                {
                    // 解釈可能なCharsetヘッダの値が見つかった場合、対応するエンコーディングを返す
                    return Encoding.GetEncoding(charsetValue);
                }
                catch (ArgumentException)
                {
                    // システムでそのCharsetが使用不可能な場合はスキップ
                }
            }

            // 未指定の場合や解釈できないCharset値の場合
            return Encoding.UTF8;
        }

        #endregion

    }
}
