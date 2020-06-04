using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    /// <summary>
    /// PLUGIN/2.0 リクエスト
    /// </summary>
    public class PluginRequest : Request
    {
        #region 定数

        public const string NotifyCommand = "NOTIFY PLUGIN/2.0";
        public const string GetCommand = "GET PLUGIN/2.0";

        #endregion

        /// <summary>
        /// リクエスト文字列をパースする
        /// </summary>
        public static PluginRequest Parse(string message)
        {
            // Request.Parseを使ってパース
            return Request.Parse<PluginRequest>(
                message,
                "PLUGIN/2.0",
                (command) => (command == GetCommand || command == NotifyCommand),
                (command, req) =>
                {
                    req.IsNotify = (command == NotifyCommand);
                }
            );
        }

        /// <summary>
        /// NOTIFYリクエストフラグ
        /// </summary>
        public virtual bool IsNotify { get; set; }

        /// <summary>
        /// コマンド文字列
        /// </summary>
        public override string Command
        {
            get
            {
                return (IsNotify ? NotifyCommand : GetCommand);
            }
        }

        /// <summary>
        /// IDヘッダ
        /// </summary>
        public virtual string ID {
            get { return this["ID"]; }
            set { this["ID"] = value; }
        }
    }
}
