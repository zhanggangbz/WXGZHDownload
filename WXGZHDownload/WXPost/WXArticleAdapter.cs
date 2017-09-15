using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WXGZHDownload.WXPost
{
    public class WXArticleAdapter
    {
        public static string ImagePath = "images";
        public static WXArticleInfo GetArticle(string htmlstr)
        {
            WXArticleInfo info = new WXArticleInfo();

            HtmlAgilityPack.HtmlDocument docBody = new HtmlAgilityPack.HtmlDocument();

            docBody.LoadHtml(htmlstr);

            HtmlAgilityPack.HtmlNode _nodeTemp = docBody.DocumentNode.SelectSingleNode("//*[@id='post-user']");
            if (_nodeTemp != null)
            {
                info.source = _nodeTemp.InnerText.Trim();
            }

            _nodeTemp = docBody.DocumentNode.SelectSingleNode("//em[@class='rich_media_meta rich_media_meta_text' and not(@id)]");
            if (_nodeTemp != null)
            {
                info.author = _nodeTemp.InnerText.Trim();
            }

            _nodeTemp = docBody.DocumentNode.SelectSingleNode("//*[@id='activity-name']");
            if (_nodeTemp != null)
            {
                info.title = _nodeTemp.InnerText.Trim();
            }

            HtmlAgilityPack.HtmlNodeCollection videoPtahs = docBody.DocumentNode.SelectNodes("//iframe[@class='video_iframe']");
            if (videoPtahs != null)
            {
                foreach (var item in videoPtahs)
                {
                    string imageUrl = item.Attributes["data-src"].Value;
                    item.Attributes.Remove("src");
                    item.Attributes.Remove("style");
                    item.Attributes.Add("style", "width: 670px ; height: 376.875px ;");
                    item.Attributes.Add("src", imageUrl);
                    item.Attributes.Remove("data-src");
                }
            }

            HtmlAgilityPack.HtmlNodeCollection ImagePtahs = docBody.DocumentNode.SelectNodes("//img[@data-src]");
            if (ImagePtahs != null)
            {
                Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                //根据Key读取<add>元素的Value
                string ak = config.AppSettings.Settings["ALBCAK"].Value;
                string sk = config.AppSettings.Settings["ALBCSK"].Value;

                ALBC_REST_SDK.ALBCClient _client = new ALBC_REST_SDK.ALBCClient(ak, sk, "image1");
                
                HttpClient client = new HttpClient();
                foreach (var item in ImagePtahs)
                {
                    string imageUrl = item.Attributes["data-src"].Value;

                    Stream inStream = client.GetStreamAsync(imageUrl).Result;

                    string[] lists = imageUrl.Split(new char[] { '?', '&' });
                    string houzhui = "";
                    foreach (string itempit in lists)
                    {
                        if (itempit.Contains("wx_fmt="))
                        {
                            houzhui = itempit.Replace("wx_fmt=", "");
                            break;
                        }
                    }

                    //按照时间+uuid来命名
                    string imagename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Guid.NewGuid().ToString("N");
                    if (string.IsNullOrWhiteSpace(houzhui) == false)
                    {
                        imagename = imagename + "." + houzhui;
                    }

                    //把文件保存到本地
                    byte[] buffer = new byte[1024];
                    Stream outStream = System.IO.File.Create(ImagePath + "\\" + imagename);
                    int l;
                    do
                    {
                        l = inStream.Read(buffer, 0, buffer.Length);
                        if (l > 0)
                            outStream.Write(buffer, 0, l);
                    }
                    while (l > 0);

                    outStream.Close();
                    inStream.Close();

                    //把图片文件上传到阿里百川
                    string newImagePath = ImagePath + "\\" + imagename;
                    ALBC_REST_SDK.ALBCUnit.ResultType rs =_client.UpLoadFile(ImagePath + "\\" + imagename, imagename, "tiankongqi");
                    if (rs.code=="OK")
                    {
                        newImagePath = rs.url;
                    }
                    //去掉html中img之前的src属性，替换为新的src属性，同时去掉data-src
                    item.Attributes.Remove("src");
                    item.Attributes.Add("src", newImagePath);
                    item.Attributes.Remove("data-src");
                }
            }

            _nodeTemp = docBody.DocumentNode.SelectSingleNode("//div[@id='js_content']");
            if (_nodeTemp != null)
            {
                info.content = _nodeTemp.OuterHtml;
                //正则去掉所有的a标签
                info.content = Regex.Replace(info.content, @"<a\s*[^>]*>", "", RegexOptions.IgnoreCase);
                info.content = Regex.Replace(info.content, @"</a>", "", RegexOptions.IgnoreCase);
            }

            return info;
        }
    }
}
