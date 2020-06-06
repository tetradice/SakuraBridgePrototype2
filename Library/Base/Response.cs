using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    /// <summary>
    /// SHIORI/SAORI/PLUGINレスポンスの基底クラス
    /// </summary>
    public abstract class Response : Message
    {
        /// <summary>
        /// ステータス文字列 (例: "PLUGIN/2.0 200 OK") 
        /// </summary>
        public string Status
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(StatusExplanation))
                {
                    return string.Format("{0} {1} {2}", Version, StatusCode, StatusExplanation);
                }
                else
                {
                    // 説明句なし
                    return string.Format("{0} {1}", Version, StatusCode);
                }
            }
        }

        /// <summary>
        /// ステータス行に出力するバージョン (例: "PLUGIN/2.0") 
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// ステータスコード (例: 200) 
        /// </summary>
        public virtual int StatusCode { get; set; }

        /// <summary>
        /// ステータス説明句 (例: "OK") 
        /// </summary>
        public virtual string StatusExplanation { get; set; }

        /// <summary>
        /// リクエストが成功したかどうか。200番台のステータスコードの場合true
        /// </summary>
        public virtual bool Success { get { return StatusCode >= 200 && StatusCode < 300; } }

        /// <summary>
        /// クライアントエラーかどうか。400番台のステータスコードの場合true
        /// </summary>
        public virtual bool IsClientError { get { return StatusCode >= 400 && StatusCode < 500; } }

        /// <summary>
        /// サーバーエラーかどうか。500番台のステータスコードの場合true
        /// </summary>
        public virtual bool IsServerError { get { return StatusCode >= 500 && StatusCode < 600; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="statusCode">ステータスコード。ここで指定した値に応じて、ステータス説明文も自動設定される</param>
        public Response(int statusCode)
        {
            StatusCode = statusCode;
            if (CommonStatusCode.ExplanationMap.ContainsKey(statusCode))
            {
                StatusExplanation = CommonStatusCode.ExplanationMap[statusCode];
            }

            // Charsetの初期値はUTF-8
            Encoding = Encoding.UTF8;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var lines = new List<string>();
            lines.Add(Status);
            foreach (var name in HeaderNames)
            {
                lines.Add(string.Format("{0}: {1}", name, this[name]));
            }

            return string.Join("\r\n", lines) + "\r\n\r\n";
        }
    }
}
