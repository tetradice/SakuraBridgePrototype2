using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    /// <summary>
    /// プラグインモジュール (主に継承して使用する)
    /// </summary>
    public abstract class PluginModule : IModule
    {
        /// <summary>
        /// DLLが置かれているディレクトリのパス。Load時にセットされる
        /// </summary>
        protected string DLLDirPath;

        /// <summary>
        /// Load処理
        /// </summary>
        /// <param name="dllDirPath">DLLが置かれているパス</param>
        public virtual void Load(string dllDirPath)
        {
            Debug.WriteLine("[plugin module load]");
            Debug.WriteLine(string.Format("dllDirPath = {0}", dllDirPath));
            DLLDirPath = dllDirPath;
        }

        /// <summary>
        /// リクエスト
        /// </summary>
        public virtual string Request(string msg)
        {
            Debug.WriteLine("[pluigin module request]");
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
        public virtual void Unload()
        {
            Debug.WriteLine("[pluigin module unload]");
        }

        public abstract string Version { get; }

        /// <summary>
        /// メニューからのプラグイン選択時に呼び出される処理
        /// </summary>
        public virtual PluginResponse OnMenuExec(PluginRequest req)
        {
            return PluginResponse.OK();
        }

        /// <summary>
        /// リクエスト/レスポンス時に使用するエンコーディング
        /// </summary>
        public virtual Encoding Encoding { get { return Encoding.UTF8; } }
    }
}
