using SakuraBridge.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeTest
{
    public class TestModule : IModule
    {
        public virtual void Load(string dllDirPath)
        {
            Debug.WriteLine("[module load]");
        }

        public virtual string Request(string msg)
        {
            Debug.WriteLine("[module request]");
            Debug.WriteLine(msg);

            PluginRequest req;
            try
            {
                req = PluginRequest.Parse(msg);
            }
            catch (BadRequestException ex)
            {
                Debug.WriteLine(ex.ToString());
                return (new PluginResponse(CommonStatusCode.BadRequest)).ToString();
            }

            var res = new PluginResponse(CommonStatusCode.OK);

            if(req.ID == "OnMenuExec")
            {
                res["Script"] = @"\0OnMenuExecイベントが呼び出されました。\e";
            }
            Debug.WriteLine(req.ToString());

            return res.ToString();
        }

        public virtual void Unload()
        {
            Debug.WriteLine("[module unload]");
        }
    }
}
