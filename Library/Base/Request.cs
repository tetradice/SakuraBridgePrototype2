using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    /// <summary>
    /// SHIORI/SAORI/PLUGINリクエスト
    /// </summary>
    public abstract class Request : Message
    {
        /// <summary>
        /// コマンド文字列 (例: "GET PLUGIN/2.0") 
        /// </summary>
        public abstract string Command { get; }
    }
}
