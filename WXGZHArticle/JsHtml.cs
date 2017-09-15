using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WXGZHArticle
{
    public class FinalHtml
    {
        private String htmlString;
        public System.String HtmlString
        {
            get { return htmlString; }
            set { htmlString = value; }
        }
        private String url;
        public System.String Url
        {
            get { return url; }
            set { url = value; }
        }

        private bool success;
        public bool Success
        {
            get { return success; }
            set { success = value; }
        }

        public FinalHtml()
        {
            url = "";
            HtmlString = "";
            success = false;
        }
        public bool Run(String _url, int timeOut = 10000)
        {
            url = _url;
            Thread newThread = new Thread(NewThread);
            WXGZHArticle.LogUtil.Log(newThread.ManagedThreadId + "线程创建成功");
            newThread.SetApartmentState(ApartmentState.STA);/// 为了创建WebBrowser类的实例 必须将对应线程设为单线程单元
            newThread.Start();
            WXGZHArticle.LogUtil.Log(newThread.ManagedThreadId + "开始等待运行中");
            //监督子线程运行时间
            while (newThread.IsAlive && timeOut > 0)
            {
                Thread.Sleep(100);
                timeOut -= 100;
            }
            WXGZHArticle.LogUtil.Log(newThread.ManagedThreadId + "开始进行超时处理");
            // 超时处理
            if (newThread.IsAlive)
            {
                if (success)
                    return true;
                newThread.Abort();
                return false;
            }
            return true;
        }

        private void NewThread()
        {
            WXGZHArticle.LogUtil.Log("FinalHtmlPerThread对象马上开始创建");
            new FinalHtmlPerThread(this);
            WXGZHArticle.LogUtil.Log("FinalHtmlPerThread对象创建完毕");
            Application.Run();// 循环等待webBrowser 加载完毕 调用 DocumentCompleted 事件
            WXGZHArticle.LogUtil.Log("Application.Run()运行完毕");
        }
        /// <summary>
        ///  用于处理一个url的核心类
        /// </summary>
        class FinalHtmlPerThread : IDisposable
        {
            FinalHtml master;
            System.Windows.Forms.WebBrowser web;

            public FinalHtmlPerThread(FinalHtml master)
            {
                this.master = master;
                DealWithUrl();
            }
            private void DealWithUrl()
            {
                String url = master.Url;
                web = new System.Windows.Forms.WebBrowser();
                bool success = false;
                try
                {
                    WXGZHArticle.LogUtil.Log("WebBrowser空间创建成功，设置URL");
                    web.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(web_DocumentCompleted); // 对事件加委托
                    web.Url = new Uri(url);
                    success = true;
                }
                finally
                {
                    if (!success)
                        Dispose();
                }

            }
            public void Dispose()
            {
                if (!web.IsDisposed)
                    web.Dispose();
            }

            private void web_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
            {
                WXGZHArticle.LogUtil.Log("WebBrowserDocumentCompleted回调成功");
                web.Stop();
                System.Threading.Thread.Sleep(500);
                //微软官方回答 一个网页有多个Ifram元素就有可能触发多次此事件， 并且提到了
                // vb 和 C++ 的解决方案， C# 没有提及， 经本人尝试，发现下面的语句可以判断成功
                // 如果未完全加载 web.ReadyState = WebBrowserReadyState.Interactive
                master.HtmlString = web.Document.Body.InnerHtml;
                master.Success = true;
                WXGZHArticle.LogUtil.Log("获取网页html成功");
                try
                {
                    //Thread.CurrentThread.Abort();
                    Application.Exit();
                    WXGZHArticle.LogUtil.Log("线程终止成功");
                }
                catch(Exception ex)
                {
                    WXGZHArticle.LogUtil.Log(Thread.CurrentThread.ManagedThreadId + "终止失败");
                    WXGZHArticle.LogUtil.Log(ex.Message);
                    WXGZHArticle.LogUtil.Log(ex.StackTrace);
                    WXGZHArticle.LogUtil.Log(ex.InnerException.Message);
                    WXGZHArticle.LogUtil.Log(ex.InnerException.StackTrace);
                    WXGZHArticle.LogUtil.Log("异常打印结束");
                }
            }
        }
    }
}
