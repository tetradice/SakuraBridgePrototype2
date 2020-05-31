using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    /// <summary>
    /// PLUGIN/2.0 レスポンス
    /// </summary>
    public class PluginResponse : Response
    {
        /// <summary>
        /// バージョン
        /// </summary>
        public override string Version { get { return "PLUGIN/2.0"; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="statusCode">ステータスコード。ここで指定した値に応じて、ステータス説明文も自動設定される</param>
        public PluginResponse(int statusCode) : base(statusCode)
        {
        }

        /// <summary>
        /// Scriptヘッダ
        /// </summary>
        public virtual string Script
        {
            get { return this["Script"]; }
            set { this["Script"] = value; }
        }
    }
}
