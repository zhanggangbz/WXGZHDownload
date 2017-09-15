using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXGZHDownload.WXPost
{
    public class WXArticleInfo
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title;
        /// <summary>
        /// 作者
        /// </summary>
        public string author;
        /// <summary>
        /// 来源
        /// </summary>
        public string source;
        /// <summary>
        /// 内容
        /// </summary>
        public string content;

        public WXArticleInfo()
        {
            title = "";
            author = "甜空气";
            source = "甜空气";
            content = "";
        }

        public bool isEmpty()
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                return true;
            }
            return false;
        }
    }
}
