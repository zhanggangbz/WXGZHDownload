using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WXGZHWCeblient.Control
{
    /// <summary>
    /// ComHandler 的摘要说明
    /// </summary>
    public class ComHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            StringBuilder _strContent = new StringBuilder();
            string _strAction = context.Request.Params["action"];
            if (string.IsNullOrEmpty(_strAction))
            {
                _strContent.Append("{\"msg\": \"0\", \"msgbox\": \"禁止访问！\",\"rows\": []}");
            }
            else
            {
                switch (_strAction.Trim().ToLower())
                {
                    case "sendwxurl":
                        SendWXUrl(context, _strContent);
                        break;
                    default:
                        break;
                }
            }
            context.Response.ContentType = "application/json";
            context.Response.Write(_strContent.ToString());
        }

        private void SendWXUrl(HttpContext context, StringBuilder _strContent)
        {
            try
            {
                string urlstr = context.Request.Form["url"];
                int typeid = int.Parse(context.Request.Form["type"]);

                WXGZHArticle.LogUtil.Log("即将增加" + urlstr + "到队列");
                WXGZHArticle.WXGZHHelper.AddWXTask(urlstr, typeid);

            }
            catch (System.Exception ex)
            {
            }
            _strContent.Append("{\"msg\": \"1\", \"msgbox\": \"已提交后台发布！\"}");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}