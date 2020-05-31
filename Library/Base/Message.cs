using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakuraBridge.Library
{
    /// <summary>
    /// SHIORI/SAORI/PLUGINのリクエストやレスポンスを表すクラス
    /// </summary>
    public abstract class Message
    {
        protected virtual Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// 指定した名前のヘッダ値を取得/設定 (取得時、指定した名前のヘッダが存在しない場合はnull)
        /// </summary>
        public virtual string this[string name]
        {
            get
            {
                return (Headers.ContainsKey(name) ? Headers[name] : null);
            }

            set
            {
                // 改行文字を含んでいる場合はエラー
                if (value.Contains("\r") || value.Contains("\n"))
                {
                    throw new ArgumentException(string.Format("{0} ヘッダに設定しようとした値に、改行文字が含まれています。"));
                }

                if (value == null)
                {
                    // nullを設定しようとした場合はクリア
                    Headers.Remove(name);
                }
                else
                {
                    Headers[name] = value;
                }
            }
        }

        /// <summary>
        /// 指定した名前の値をヘッダに含んでいるかどうかを判定
        /// </summary>
        public virtual bool Contains(string name)
        {
            return Headers.ContainsKey(name);
        }

        /// <summary>
        /// 指定した名前の値をヘッダから削除
        /// </summary>
        public virtual bool Remove(string name)
        {
            return Headers.ContainsKey(name);
        }

        /// <summary>
        /// Reference0, 1, 2... のリストを取得/設定
        /// </summary>
        public virtual List<string> References
        {
            get { return GetSequentialHeaderValues("Reference"); }
            set { SetSequentialHeaderValues("Reference", value); }
        }

        /// <summary>
        /// Reference0, 1, 2... やArgument0, 1, 2... など、連番方式のヘッダ値をListとして取得
        /// </summary>
        public virtual List<string> GetSequentialHeaderValues(string prefix)
        {
            var values = new List<string>();
            for (var i = 0; true; i++)
            {
                var name = prefix + i.ToString();
                if (Contains(name))
                {
                    values.Add(this[name]);
                }
                else
                {
                    break;
                }
            }

            return values;
        }

        /// <summary>
        /// Reference0, 1, 2... やArgument0, 1, 2... など、連番方式のヘッダ値を設定する。既存の設定値はクリアされる
        /// </summary>
        public virtual void SetSequentialHeaderValues(string prefix, IList<string> values)
        {
            // 既存値をクリア
            foreach (var name in HeaderNames)
            {
                if (name.StartsWith(prefix))
                {
                    Remove(name);
                }
            }

            // 値を設定
            for (var i = 0; i < values.Count; i++)
            {
                var name = prefix + i.ToString();
                this[name] = values[i];
            }
        }

        /// <summary>
        /// ヘッダ名のコレクションを取得
        /// </summary>
        public virtual ICollection<string> HeaderNames { get { return Headers.Keys; } }

        /// <summary>
        /// Charsetヘッダ
        /// </summary>
        public virtual string Charset
        {
            get { return this["Charset"]; }
            set { this["Charset"] = value; }
        }

        /// <summary>
        /// エンコーディング (Charsetで指定された内容と連動)
        /// 取得時、Charsetが未指定の場合や、解釈できない値の場合はnullを返す;
        /// </summary>
        public virtual Encoding Encoding
        {
            get
            {
                return (this.Charset != null ? FindEncodingByCharset(this.Charset) : null);
            }
            set
            {
                this.Charset = (value != null ? ConvertEncoding2Charset(value) : null);
            }
        }

        /// <summary>
        /// Charsetとして指定された値から、対応するEncodingを探す。
        /// エンコーディングが見つからない場合、未サポート時はnullを返す
        /// </summary>
        protected static Encoding FindEncodingByCharset(string charset)
        {
            if (charset == null) throw new ArgumentNullException("charset");

            try
            {
                if (!string.IsNullOrWhiteSpace(charset))
                {
                    // 見つかった
                    return Encoding.GetEncoding(charset);
                }
            }
            catch (ArgumentException)
            {
            }
            catch (NotSupportedException)
            {
            }
            return null;
        }

        /// <summary>
        /// EncodingからCharsetで使用可能な名前を取得 (一般的でないCharsetの場合はWebNameを返す)
        /// </summary>
        protected static string ConvertEncoding2Charset(Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");

            if (encoding == Encoding.UTF8) return "UTF-8";
            if (encoding == Encoding.GetEncoding("Shift_JIS")) return "EUC-JP";
            if (encoding == Encoding.GetEncoding("EUC-JP")) return "EUC-JP";
            if (encoding == Encoding.GetEncoding("ISO-2022-JP")) return "ISO-2022-JP";
            return encoding.WebName;
        }
    }
}
