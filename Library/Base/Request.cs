using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    /// <summary>
    /// SHIORI/SAORI/PLUGINリクエストの基底クラス
    /// </summary>
    public abstract class Request : Message
    {
        /// <summary>
        /// コマンド文字列 (例: "GET PLUGIN/2.0") 
        /// </summary>
        public abstract string Command { get; }

        /// <summary>
        /// Senderヘッダ
        /// </summary>
        public virtual string Sender
        {
            get { return this["Sender"]; }
            set { this["Sender"] = value; }
        }

        /// <summary>
        /// 文字列化
        /// </summary>
        public override string ToString()
        {
            var lines = new List<string>();
            lines.Add(Command);
            foreach (var name in HeaderNames)
            {
                lines.Add(string.Format("{0}: {1}", name, this[name]));
            }

            return string.Join("\r\n", lines) + "\r\n\r\n";
        }

        #region staticメンバ

        /// <summary>
        /// リクエスト文字列をパースする
        /// </summary>
        /// <param name="message">パース対象のリクエスト文字列</param>
        /// <param name="versionCaption">パース対象のリクエストバージョン表記。例外発生時に表示される</param>
        /// <param name="CommandCheckFunc">コマンド行が正しいかどうかを判定する関数</param>
        /// <param name="RequestSetupFunc">リクエストに必要な初期値をセットする関数。ヘッダのセット後に、コマンド行、リクエストを引数として呼ばれる</param>
        public static TReq Parse<TReq>(
            string message,
            string versionCaption,
            Func<string, bool> CommandCheckFunc,
            Action<string, TReq> RequestSetupFunc = null
        )
            where TReq : Request, new()
        {
            var headerPattern = new Regex(Message.HeaderPattern);

            if (message == null) throw new ArgumentNullException("message");

            // メッセージを改行で分割
            var lines = message.Split(new[] { "\r\n" }, StringSplitOptions.None);

            // 1行目のコマンド行が正しければ有効メッセージ
            if (CommandCheckFunc.Invoke(lines[0]))
            {
                var req = new TReq();

                // 2行目以降の行をパース
                for (var i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];

                    // 空行ならその時点で終了
                    if (line == "") return req;

                    // ヘッダとして解析
                    var matched = headerPattern.Match(line);
                    if (matched.Success)
                    {
                        var name = matched.Groups[1].Value;
                        var value = matched.Groups[2].Value;
                        req[name] = value;
                    }
                    else
                    {
                        throw new BadRequestException(string.Format("リクエストの{0}行目にヘッダではない行が含まれています。", i + 1));
                    }
                }

                // リクエストの初期設定
                if (RequestSetupFunc != null) RequestSetupFunc.Invoke(lines[0], req);

                // 空行が見つからずに終わった場合も結果を返す
                return req;
            }
            else
            {
                throw new BadRequestException(string.Format("有効な {0} リクエストではありません。 (コマンド行: {1})", versionCaption, lines[0]));
            }
        }

        #endregion
    }
}
