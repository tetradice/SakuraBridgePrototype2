using SakuraBridge.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeTest
{
    public class TestModule : IModule
    {
        protected string DllDirPath;
        protected AppDomain FormAppDomain;

        public virtual void Load(string dllDirPath)
        {
            Debug.WriteLine("[module load]");
            DllDirPath = dllDirPath;
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
                res.Script = @"\0OnMenuExecイベントが呼び出されました。\e";

                FormAppDomain = AppDomain.CreateDomain("子画面");
                Task.Run(() =>
                {
                    FormAppDomain.ExecuteAssembly(Path.Combine(DllDirPath, "BridgeTestForm.exe"));
                });
            }
            Debug.WriteLine(res.ToString());

            return res.ToString();
        }

        public virtual void Unload()
        {
            Debug.WriteLine("[module unload]");
            AppDomain.Unload(FormAppDomain);
        }
    }
}
