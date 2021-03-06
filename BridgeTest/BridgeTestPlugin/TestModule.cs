﻿using SakuraBridge.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeTest
{
    public class TestModule : PluginModule
    {
        protected AppDomain FormAppDomain;

        public override PluginResponse OnMenuExec(PluginRequest req)
        {
            var res = PluginResponse.OK();
            res.Script = @"\0OnMenuExecイベントが呼び出されました。\e";

            FormAppDomain = AppDomain.CreateDomain("子画面");
            Task.Run(() =>
            {
                FormAppDomain.ExecuteAssembly(Path.Combine(DLLDirPath, "BridgeTestForm.exe"));
            });

            return res;
        }

        public override void Unload()
        {
            base.Unload();
            if (FormAppDomain != null)
            {
                AppDomain.Unload(FormAppDomain);
            }
        }
    }
}
