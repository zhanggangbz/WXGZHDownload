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
                WXGZHArticle.LogUtil.Log("开始创建阿里百川对象");
                ALBC_REST_SDK.ALBCClient _client = new ALBC_REST_SDK.ALBCClient(WXGZHDownload.DeDe.DedeStaticValues.ALBCAK, WXGZHDownload.DeDe.DedeStaticValues.ALBCSK, "image1");

                WXGZHArticle.LogUtil.Log("开始抓取图片");
                HttpClient client = new HttpClient();
                foreach (var item in ImagePtahs)
                {
                    string imageUrl = item.Attributes["data-src"].Value;

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

                    //把图片文件上传到阿里百川
                    string newImagePath = ImagePath + "\\" + imagename;
                    ALBC_REST_SDK.ALBCUnit.ResultType rs = _client.UpLoadFile(new Uri(imageUrl), imagename, "tiankongqi");
                    if (rs.code=="OK")
                    {
                        newImagePath = rs.url;
                    }

                    //给文章设置缩略图
                    if (string.IsNullOrWhiteSpace(info.sltImageUrl) && houzhui.ToUpper()!="GIF")
                    {
                        info.sltImageUrl = newImagePath;
                    }

                    //去掉html中img之前的src属性，替换为新的src属性，同时去掉data-src
                    item.Attributes.Remove("src");
                    item.Attributes.Add("src", newImagePath);
                    item.Attributes.Remove("data-src");
                    item.Attributes.Remove("style");
                    item.Attributes.Add("style", "width: auto !important; height: auto !important; line-height: 24.38px; font-size: 14.44px;max-width:100%");
                }

                WXGZHArticle.LogUtil.Log("抓取图片结束");
            }

            _nodeTemp = docBody.DocumentNode.SelectSingleNode("//div[@id='js_content']");
            if (_nodeTemp != null)
            {
                info.content = _nodeTemp.OuterHtml;
                //正则去掉所有的a标签
                info.content = Regex.Replace(info.content, @"<a\s*[^>]*>", "", RegexOptions.IgnoreCase);
                info.content = Regex.Replace(info.content, @"</a>", "", RegexOptions.IgnoreCase);
            }

            //设置缩略图
            SetSLTInfo(info);

            return info;
        }

        public static void SetSLTInfo(WXArticleInfo info)
        {
            if (string.IsNullOrWhiteSpace(info.sltImageUrl) == false)
            {
                if (info.sltImageUrl.Contains("alimmdn.com"))
                {
                    info.sltImageUrl += "@!tkqslt";
                }
            }
            else
            {
                ALBC_REST_SDK.ALBCClient _client = new ALBC_REST_SDK.ALBCClient(WXGZHDownload.DeDe.DedeStaticValues.ALBCAK, WXGZHDownload.DeDe.DedeStaticValues.ALBCSK, "image1");
                string imagename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Guid.NewGuid().ToString("N")+".jpg";
                ALBC_REST_SDK.ALBCUnit.ResultType rs = _client.UpLoadFile(new Uri("https://bing.ioliu.cn/v1/rand?w=640&h=480"), imagename, "tiankongqi");
                if (rs.code == "OK")
                {
                    info.sltImageUrl = rs.url;
                }
                else
                {
                    info.sltImageUrl = "https://bing.ioliu.cn/v1/rand?w=640&h=480";
                }
            }
        }
    }
}
