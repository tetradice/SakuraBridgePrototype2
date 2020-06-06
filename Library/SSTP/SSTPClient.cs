using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    /// <summary>
    /// SSTP送信クライアント
    /// </summary>
    public class SSTPClient
    {
        public const int DefaultToPort = 9801;

        /// <summary>
        /// 送信先ホスト (初期値: 127.0.0.1)
        /// </summary>
        public virtual string ToHost { get; set; }

        /// <summary>
        /// 送信先ポート (初期値: 9801)
        /// </summary>
        public virtual int ToPort { get; set; }

        /// <summary>
        /// リクエスト、レスポンスの内容などをDebug.WriteLineで出力するかどうか
        /// </summary>
        public virtual bool DebugLogging { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SSTPClient()
        {
            ToHost = "127.0.0.1";
            ToPort = DefaultToPort;
            DebugLogging = false;
        }

        /// <summary>
        /// SSTPリクエストを送信し、レスポンスを得る
        /// </summary>
        /// <param name="req">リクエストオブジェクト</param>
        /// <remarks>
        /// khskさんの作成されたソースを参考にしています。
        /// https://qiita.com/khsk/items/177741a6c573790a9379
        /// </remarks>
        /// <returns>成功した場合はResponseオブジェクト、失敗した場合はnull</returns>
        public virtual SSTPResponse SendRequest(SSTPRequest req)
        {
            // Charset未指定のリクエストには対応しない
            if (req.Encoding == null)
            {
                new ArgumentException(string.Format("{0} でSSTPリクエストを送信する場合は、リクエストに有効なCharsetを指定する必要があります。", MethodBase.GetCurrentMethod().Name));
            }

            if (DebugLogging)
            {
                Debug.WriteLine("[SSTP Request]");
                Debug.WriteLine(req.ToString());
            }

            var data = req.Encoding.GetBytes(req.ToString());

            using (var client = new TcpClient(ToHost, ToPort))
            {
                using (var ns = client.GetStream())
                {
                    ns.ReadTimeout = 10 * 1000; // 読み込みタイムアウト10秒
                    ns.WriteTimeout = 10 * 1000; // 書き込みタイムアウト10秒

                    // リクエストを送信する
                    ns.Write(data, 0, data.Length); // リクエスト

                    //サーバーから送られたデータを受信する
                    using (var ms = new System.IO.MemoryStream())
                    {
                        var resBytes = new byte[256];
                        var resSize = 0;
                        do
                        {
                            //データの一部を受信する
                            resSize = ns.Read(resBytes, 0, resBytes.Length);

                            //Readが0を返した時はサーバーが切断したと判断
                            if (resSize == 0)
                            {
                                break;
                            }

                            //受信したデータを蓄積する
                            ms.Write(resBytes, 0, resSize);

                            //まだ読み取れるデータがあるか、データの最後が\nでない時は、受信を続ける
                        } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');

                        // 受信したデータを文字列に変換
                        var resMsg = req.Encoding.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                        // 末尾の\0, \nを削除
                        resMsg = resMsg.TrimEnd('\0').TrimEnd();

                        if (DebugLogging)
                        {
                            Debug.WriteLine("[SSTP Response]");
                            Debug.WriteLine(resMsg);
                        }

                        return SSTPResponse.Parse(resMsg);
                    }
                }
            }
        }
    }

}
