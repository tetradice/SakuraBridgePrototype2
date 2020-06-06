using SakuraBridge.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    /// <summary>
    /// モジュールの基底クラス
    /// </summary>
    public abstract class ModuleBase : MarshalByRefObject, IModule
    {
        /// <inheritdoc />
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            // 起動後しばらく経っても開放されないように、生存期間を無期限とする
            // 参考: <https://stackoverflow.com/questions/2410221/appdomain-and-marshalbyrefobject-life-time-how-to-avoid-remotingexception>
            return null;
        }

        /// <inheritdoc />
        public abstract Encoding Encoding { get; }

        /// <inheritdoc />
        public abstract void Load(string dllDirPath);

        /// <inheritdoc />
        public abstract string Request(string msg);

        /// <inheritdoc />
        public abstract void Unload();
    }
}
