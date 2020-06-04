using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    /// <summary>
    /// SSTPリクエスト
    /// </summary>
    public abstract class SSTPRequest : Request
    {
    }

    /// <summary>
    /// NOTIFY SSTP/1.1 リクエスト
    /// </summary>
    public class NotifySSTP11Request : Request
    {
        /// <summary>
        /// コマンド文字列
        /// </summary>
        public override string Command { get { return "NOTIFY SSTP/1.1"; } }
    }

    /// <summary>
    /// SEND SSTP/1.4 リクエスト
    /// </summary>
    public class SendSSTP14Request : Request
    {
        /// <summary>
        /// コマンド文字列
        /// </summary>
        public override string Command { get { return "SEND SSTP/1.4"; } }
    }
}
