using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    /// <summary>
    /// SSTPレスポンス
    /// </summary>
    /// <remarks>SHIORI/SAORI/PLUGINのレスポンスとはフォーマットが異なるため、Responseクラスを継承していないことに注意してください。</remarks>
    public class SSTPResponse : Message
    {
        /// <summary>
        /// レスポンス文字列をパースする
        /// </summary>
        public static SSTPResponse Parse(string message)
        {
            var headerPattern = new Regex(Message.HeaderPattern);

            if (message == null) throw new ArgumentNullException("message");

            // メッセージを改行で分割
            var lines = message.Split(new[] { "\r\n" }, StringSplitOptions.None);

            // 1行目のステータス行が正しければ有効なSSTPレスポンス
            var matched = Regex.Match(lines[0], @"^(SSTP/[0-9.]+)\s+(\d{1,7})(?:\s+(.+))?"); // 仕様上ステータスコードの桁数には記載がないため、一応7桁まで受け付ける
            if (matched.Success)
            {
                var version = matched.Groups[1].Value;
                var statusCode = int.Parse(matched.Groups[2].Value);
                string explanation = null;
                if (matched.Groups[3].Success)
                {
                    explanation = matched.Groups[3].Value;
                }

                var res = new SSTPResponse();
                res.Version = version;
                res.StatusCode = statusCode;
                res.StatusExplanation = explanation;

                // 2行目以降の行をパース
                var additionalLines = new List<string>();
                for (var i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];

                    // 空行は読み飛ばす
                    if (line == "") continue;

                    // 追加情報として格納
                    additionalLines.Add(line);
                }

                // 追加情報があればセット
                if (additionalLines.Any())
                {
                    res.AdditionalData = string.Join("\r\n", additionalLines);
                }

                return res;
            }
            else
            {
                throw new BadRequestException(string.Format("有効なSSTPレスポンスではありません。 (コマンド行: {1})", lines[0]));
            }
        }

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
        public string Version { get; set; }

        /// <summary>
        /// ステータスコード (例: 200) 
        /// </summary>
        public virtual int StatusCode { get; set; }

        /// <summary>
        /// ステータス説明句 (例: "OK") 
        /// </summary>
        public virtual string StatusExplanation { get; set; }

        /// <summary>
        /// EXECUTE/1.3 リクエストなどで取得可能な追加情報。存在しない場合はnull
        /// </summary>
        public virtual string AdditionalData { get; set; }

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
        public SSTPResponse()
        {
            AdditionalData = null;

            // Charsetの初期値はUTF-8
            Encoding = Encoding.UTF8;
        }
    }
}