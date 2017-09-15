using DotNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WXGZHDownload.WXPost;

namespace WXGZHDownload.DeDe
{
    public class DeDePublic
    {
        HttpHelper httpClient = new HttpHelper();
        public bool run(WXArticleInfo info)
        {
            bool rs = false;
            rs = login();
            sendarticle(info);
            return rs;
        }
        CookieCollection coolies = new CookieCollection();


        private bool login()
        {
            HttpItem item = new HttpItem()
            {
                URL = "http://dg.ce.kankan.ml/dede/login2.php",//URL     必需项  
                Method = "Get",//URL     可选项 默认为Get  
                Timeout = 100000,//连接超时时间     可选项默认为100000  
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000  
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写  
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0",//用户的浏览器类型，版本，操作系统     可选
                ResultType = ResultType.String,//返回数据类型，是Byte还是String  
                ProxyIp = "127.0.0.1:8888",
            };
            HttpResult result = httpClient.GetHtml(item);
            coolies = result.CookieCollection;
            if (result.Html.Contains("成功登录"))
            {
                HttpItem item2 = new HttpItem()
                {
                    URL = "http://dg.ce.kankan.ml/dede",//URL     必需项  
                    Method = "get",//URL     可选项 默认为Get  
                    Timeout = 100000,//连接超时时间     可选项默认为100000  
                    ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000  
                    IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写  
                    UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0",//用户的浏览器类型，版本，操作系统     
                    Referer = "http://dg.ce.kankan.ml",
                    ResultType = ResultType.String,//返回数据类型，是Byte还是String  
                    ProxyIp = "127.0.0.1:8888",
                };

                HttpResult result2 = httpClient.GetHtml(item2);
                Console.Write(result2.Html);
                return true;
            }
            return false;
        }

        private bool sendarticle(WXArticleInfo info)
        {
            bool rs = false;

            string urlstr = "http://dg.ce.kankan.ml/dede/article_add.php";


            HttpItem item2 = new HttpItem()
            {
                URL = urlstr,//URL     必需项  
                Method = "post",//URL     可选项 默认为Get  
                Timeout = 100000,//连接超时时间     可选项默认为100000  
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000  
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写  
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0",//用户的浏览器类型，版本，操作系统   
                PostDataType = PostDataType.Byte,
                ContentType= "multipart/form-data; boundary=---------------------------7e128826811ec",
                Referer = "http://dg.ce.kankan.ml",
                ResultType = ResultType.String,//返回数据类型，是Byte还是String  
                ProxyIp = "127.0.0.1:8888",
            };

            //0标题1来源2作者3内容4时间
            string strbody = System.IO.File.ReadAllText("postt.txt");

            strbody = string.Format(strbody, "biaoti111", "biaoti2222", "标题2222", "内容2222", DateTime.Now.ToString("yyyy-MM-dd HH:mm:dd"));
            byte[] bytedate = System.Text.Encoding.UTF8.GetBytes(strbody);

            item2.PostdataByte = bytedate;
            HttpResult result2 = httpClient.GetHtml(item2);
            Console.Write(result2.Html);
            return rs;
        }
    }
}
