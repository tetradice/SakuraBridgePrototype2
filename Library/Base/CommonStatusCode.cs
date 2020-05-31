using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    /// <summary>
    /// レスポンスで使用する一般的なステータスを表す定数クラス
    /// </summary>
    public static class CommonStatusCode
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const int OK = 200;

        /// <summary>
        /// 正常/結果なし
        /// </summary>
        public const int NoContent = 204;

        /// <summary>
        /// 不正なリクエスト
        /// </summary>
        public const int BadRequest = 400;

        /// <summary>
        /// サーバが未実装
        /// </summary>
        public const int NotImplemented = 501;

        /// <summary>
        /// サーバがリクエストを受け付けられない状態
        /// </summary>
        public const int ServiceUnavailable = 503;

        #region スタティックコンストラクタ

        /// <summary>
        /// ステータス説明文
        /// </summary>
        public static Dictionary<int, string> ExplanationMap;

        static CommonStatusCode()
        {
            ExplanationMap = new Dictionary<int, string>();
            ExplanationMap.Add(OK, "OK");
            ExplanationMap.Add(NoContent, "NoContent");
            ExplanationMap.Add(BadRequest, "Bad Request");
            ExplanationMap.Add(NotImplemented, "Not Implemented");
            ExplanationMap.Add(ServiceUnavailable, "Service Unavailable");
        }

        #endregion
    }
}
