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
        /// Reference0, 1, 2... のリストを取得
        /// </summary>
        public virtual List<string> References
        {
            get
            {
                var refs = new List<string>();
                for (var i = 0; true; i++)
                {
                    var name = string.Format("Reference{0}", i);
                    if (Contains(name))
                    {
                        refs.Add(this[name]);
                    }
                    else
                    {
                        break;
                    }
                }

                return refs;
            }
        }

        /// <summary>
        /// ヘッダ名のコレクションを取得
        /// </summary>
        public virtual IEnumerable<string> HeaderKeys { get { return Headers.Keys; } }

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
