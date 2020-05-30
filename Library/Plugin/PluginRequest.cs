using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    public class PluginRequest : Request
    {
        public static PluginRequest Parse(string message)
        {
            throw new NotImplementedException();
        }

        public virtual bool Notify { get; set; }

        public override string Command
        {
            get
            {
                return (Notify ? "GET NOTIFY/2.0" : "GET PLUGIN/2.0");
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
