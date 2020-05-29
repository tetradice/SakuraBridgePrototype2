using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    public abstract class RequestBase
    {
        protected virtual Dictionary<string, string> Headers { get; set; }

        public abstract string Status { get; }

        public virtual string this[string name]
        {
            get { return (Headers.ContainsKey(name) ? Headers[name] : null); }
            set {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Headers.Remove(name);
                } else
                {
                    Headers[name] = value;
                }
            }
        }

        public virtual string Charset
        {
            get { return this["Charset"]; }
            set { this["Charset"] = value; }
        }
    }
}
