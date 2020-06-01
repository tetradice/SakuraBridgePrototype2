using iCal;
using SakuraBridge.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotei
{
    public class MainModule : PluginModule
    {
        /// <summary>
        /// プラグインのバージョン (versionリクエストに対して返す値)
        /// </summary>
        public override string Version
        {
            get { return "Yotei-1.0"; }
        }

        public override PluginResponse OnMenuExec(PluginRequest req)
        {
            var res = PluginResponse.OK();
            res.Event = "OnYoteiGet";
            res.Script = @"\0予定を取得しています.\w8\w8.\w8\w8.\w8\w8\e";

            Task.Run(() =>
            {
                var calendar = iCalendar.LoadFromUri(new Uri(@"https://calendar.google.com/xxxxx/basic.ics"));
                Debug.WriteLine("calendar get complete");
            });

            return res;
        }
    }
}
