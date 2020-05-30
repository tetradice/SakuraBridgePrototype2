using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    /// <summary>
    /// SHIORI/SAORI/PLUGINレスポンス
    /// </summary>
    public abstract class Response : Message
    {
        /// <summary>
        /// ステータス文字列 (例: "200 OK") 
        /// </summary>
        public abstract string Status { get; }
    }
}
