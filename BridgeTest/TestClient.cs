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
        public void Load(string dllDirPath)
        {
            Debug.Write("module load");
        }

        public string Request(string msg)
        {
            Debug.Write("module request");
            return "PLUGIN/2.0 200 OK\r\n\r\n";
        }

        public void Unload()
        {
            Debug.Write("module unload");
        }
    }
}
