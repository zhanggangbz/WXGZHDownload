using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WXGZHDownload.WXPost;

namespace WXGZHArticle
{
    class WXTaskInfo
    {
        public string id;
        public string url;
        public int sendtype;

        /// <summary>
        /// 任务标记，0等待运行，1正在运行,2运行结束
        /// </summary>
        public int taskflag;
        /// <summary>
        /// 任务开始执行的时间
        /// </summary>
        public DateTime taskStartTime;

        public WXTaskInfo(string _url,int _type)
        {
            id = Guid.NewGuid().ToString();
            url = _url;
            sendtype = _type;
        }

        public override bool Equals(object obj)
        {
            WXTaskInfo rs = obj as WXTaskInfo;
            if (rs != null && rs.id == this.id)
            {
                return true;
            }
            return false;
        }

        public void Run()
        {
            //开始运行
            taskflag = 1;
            taskStartTime = DateTime.Now;
            WXGZHArticle.LogUtil.Log(url + "准备开始获取网页信息");
            FinalHtml html = new FinalHtml();
            if (html.Run(url))
            {
                WXGZHArticle.LogUtil.Log(url + "网页信息获取成功");
                try
                {

                    WXArticleInfo rs = WXArticleAdapter.GetArticle(html.HtmlString);

                    string articleid = Guid.NewGuid().ToString("N");

                    bool dbrs = WXArticleDBHelper.InseartDB(rs, articleid);

                    if (dbrs)
                    {
                        WXGZHArticle.LogUtil.Log(url + "操作完成已写入数据库！");
                    }
                    else
                    {
                        WXGZHArticle.LogUtil.Log(url + "操作失败未写入数据库！");
                    }

                    WXGZHDownload.DeDe.DeDePublic1 dede = new WXGZHDownload.DeDe.DeDePublic1();
                    if (dede.run(rs, sendtype))
                    {
                        WXGZHArticle.LogUtil.Log(url + "已发布到网站！");

                        if (WXArticleDBHelper.ResetArticleFlag(articleid, 1))
                        {
                            WXGZHArticle.LogUtil.Log(url + "数据库更新成功！");
                        }
                        else
                        {
                            WXGZHArticle.LogUtil.Log(url + "数据库更新失败！");
                        }
                    }
                    else
                    {
                        WXGZHArticle.LogUtil.Log(url + "未发布到网站！");
                    }
                    WXGZHArticle.LogUtil.Log(url + "采集及发布过程结束！");
                }
                catch (System.Exception ex)
                {
                    WXGZHArticle.LogUtil.Log(url + "采集过程出现异常！"+ex.Message);
                    WXGZHArticle.LogUtil.Log(url + ex.StackTrace);
                }

            }
            //运行结束
            taskflag = 2;
        }
    }
}
