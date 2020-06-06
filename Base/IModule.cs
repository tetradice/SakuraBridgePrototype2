using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakuraBridge.Base
{
    /// <summary>
    /// 実際にリクエストを処理するモジュールクラスのインターフェース
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// DLLのロード時に呼び出される処理です。
        /// </summary>
        /// <param name="dllDirPath">bridge.dll が存在するフォルダのパス</param>
        void Load(string dllDirPath);

        /// <summary>
        /// DLLのアンロード時に呼び出される処理です。
        /// </summary>
        void Unload();

        /// <summary>
        /// リクエスト受信時に呼び出される処理です。
        /// </summary>
        /// <param name="msg">リクエスト文字列</param>
        /// <returns>レスポンス文字列</returns>
        string Request(string msg);
    }
}
