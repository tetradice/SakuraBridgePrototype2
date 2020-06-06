using SakuraBridge.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    /// <summary>
    /// プラグインモジュール (主に継承して使用する)
    /// </summary>
    public abstract class PluginModule : ModuleBase
    {
        /// <summary>
        /// DLLが置かれているディレクトリのパス。Load時にセットされる
        /// </summary>
        protected string DLLDirPath;

        /// <summary>
        /// Load処理
        /// </summary>
        /// <param name="dllDirPath">DLLが置かれているパス</param>
        public override void Load(string dllDirPath)
        {
            Debug.WriteLine("[plugin module load]");
            Debug.WriteLine(string.Format("dllDirPath = {0}", dllDirPath));
            DLLDirPath = dllDirPath;
        }

        /// <summary>
        /// リクエスト
        /// </summary>
        public override string Request(string msg)
        {
            Debug.WriteLine("[plugin module request]");
            Debug.WriteLine(msg);

            // リクエストのパース
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

            // レスポンスの生成
            var res = MakeResponse(req);

            Debug.WriteLine(res.ToString());

            return res.ToString();
        }

        /// <summary>
        /// リクエストIDに対応するResponseを作成する
        /// </summary>
        public virtual PluginResponse MakeResponse(PluginRequest req)
        {
            if (req.ID == "version")
            {
                var res = PluginResponse.OK();
                res["Value"] = this.Version;
                return res;
            }
            else if (req.ID == "OnMenuExec")
            {
                return OnMenuExec(req);
            }
            else
            {
                return new PluginResponse(CommonStatusCode.NotImplemented);
            }
        }

        /// <summary>
        /// Unload処理
        /// </summary>
        public override void Unload()
        {
            Debug.WriteLine("[plugin module unload]");
        }

        /// <summary>
        /// バージョン文字列 (オーバーライドしない場合はアセンブリ名とアセンブリのバージョンから自動生成される)
        /// </summary>
        public virtual string Version {
            get {
                var asm = Assembly.GetAssembly(this.GetType());
                return string.Format("{0}-{1}", asm.GetName().Name, asm.GetName().Version);
            }
        }

        /// <summary>
        /// メニューからのプラグイン選択時に呼び出される処理
        /// </summary>
        public virtual PluginResponse OnMenuExec(PluginRequest req)
        {
            return PluginResponse.OK();
        }
    }
}
