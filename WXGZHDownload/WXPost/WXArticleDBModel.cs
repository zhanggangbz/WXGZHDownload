using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXGZHDownload.WXPost
{
    public enum TableNames { TB_WXARTICLE }

    #region 枚举
    public enum TB_WXARTICLE { id, title, author, source, content, flag }
    #endregion
    public class WXArticleDBModel
    {
        private string _id;
        public string id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        private string _title;
        public string title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }
        private string _author;
        public string author
        {
            get
            {
                return _author;
            }
            set
            {
                _author = value;
            }
        }
        private string _source;
        public string source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }
        private string _content;
        public string content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }
        private int _flag;
        public int flag
        {
            get
            {
                return _flag;
            }
            set
            {
                _flag = value;
            }
        }
    }
}
