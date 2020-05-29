using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakuraBridge.Shared
{
    /// <summary>
    /// 実際にリクエストを処理するクラスのインターフェース
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// DLLのロード時に呼び出される処理
        /// </summary>
        /// <param name="dllDirPath">DLLが存在するパス</param>
        void Load(string dllDirPath);

        /// <summary>
        /// DLLのアンロード時に呼び出される処理
        /// </summary>
        void Unload();

        /// <summary>
        /// リクエスト受信
        /// </summary>
        /// <param name="msg">リクエスト文字列</param>
        /// <returns>レスポンス文字列</returns>
        string Request(string msg);
    }
}
