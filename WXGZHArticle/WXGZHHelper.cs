using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WXGZHArticle
{
    public class WXGZHHelper
    {
        private static Queue<WXTaskInfo> taskQueus = new Queue<WXTaskInfo>();

        private static Thread thread=null;

        public static void AddWXTask(string _url,int _type)
        {
            if (_url.Contains("weixin"))
            {
                WXGZHArticle.LogUtil.Log(_url + "已经加入队列");
                WXTaskInfo info = new WXTaskInfo(_url, _type);
                taskQueus.Enqueue(info);
            }
            if (thread == null && taskQueus.Count == 1)
            {
                thread = new Thread(ThreadWork);
                WXGZHArticle.LogUtil.Log(thread.ManagedThreadId + "马上开始");
                try
                {
                    thread.Start();
                }
                catch(Exception ex)
                {
                    WXGZHArticle.LogUtil.Log(thread.ManagedThreadId + "启动失败");
                    WXGZHArticle.LogUtil.Log(ex.Message);
                    WXGZHArticle.LogUtil.Log(ex.StackTrace);
                    WXGZHArticle.LogUtil.Log(ex.InnerException.Message);
                    WXGZHArticle.LogUtil.Log(ex.InnerException.StackTrace);
                    thread = null;
                }
            }
        }

        private static void ThreadWork()
        {
            WXGZHArticle.LogUtil.Log(thread.ManagedThreadId + "已进入线程函数主体");
            WXGZHArticle.LogUtil.Log(thread.ManagedThreadId + "此时队列任务个数" + taskQueus.Count);
            while (taskQueus.Count > 0)
            {
                WXTaskInfo mes = taskQueus.Peek();
                if (mes.taskflag == 0)
                {
                    mes.Run();

                    WXGZHArticle.LogUtil.Log(mes.url + "已经撤出队列");
                    taskQueus.Dequeue();
                }
            }
            System.Threading.Thread.Sleep(1000);

            WXGZHArticle.LogUtil.Log(thread.ManagedThreadId + "马上结束，队列已空");

            thread = null;
        }
    }
}
