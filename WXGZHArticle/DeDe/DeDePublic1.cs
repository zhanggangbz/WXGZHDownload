using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using WXGZHDownload.WXPost;
using System.Net;

namespace WXGZHDownload.DeDe
{
    class DeDePublic1
    {
        HttpClient httpClient;
        HttpClientHandler handler = new HttpClientHandler();
        public DeDePublic1()
        {
            handler.CookieContainer = new CookieContainer();
            httpClient = new HttpClient(handler);
        }
        public bool run(WXArticleInfo info,int type1)
        {
            bool rs = false;
            rs = login();
            if (rs)
            {
                rs = sendarticle(info, type1);
            }
            return rs;
        }

        private bool sendarticle(WXArticleInfo info, int type1)
        {
            bool rs = false;

            MultipartFormDataContent mm = new MultipartFormDataContent();
            AddFlagIntoParm(mm);
            mm.Add(new StringContent("1"), "channelid");
            mm.Add(new StringContent("save"), "dopost");
            mm.Add(new StringContent(info.title), "title");
            mm.Add(new StringContent(""), "shorttitle");
            mm.Add(new StringContent(""), "redirecturl");
            mm.Add(new StringContent(""), "tags");
            mm.Add(new StringContent("2"), "weight");
            mm.Add(new StringContent(info.sltImageUrl), "picname");
            mm.Add(new StringContent(info.source), "source");
            mm.Add(new StringContent(info.author), "writer");
            mm.Add(new StringContent(type1.ToString()), "typeid");
            mm.Add(new StringContent(""), "typeid2");
            mm.Add(new StringContent(""), "keywords");
            mm.Add(new StringContent("1"), "autokey");
            mm.Add(new StringContent(""), "description");
            mm.Add(new StringContent(""), "dede_addonfields");
            mm.Add(new StringContent("0"), "remote");
            mm.Add(new StringContent("1"), "autolitpic");
            mm.Add(new StringContent("1"), "needwatermark");
            mm.Add(new StringContent("hand"), "sptype");
            mm.Add(new StringContent("5"), "spsize");
            mm.Add(new StringContent(info.content), "body");
            mm.Add(new StringContent(""), "voteid");
            mm.Add(new StringContent("0"), "notpost");
            mm.Add(new StringContent("112"), "click");
            mm.Add(new StringContent("0"), "sortup");
            mm.Add(new StringContent(""), "color");
            mm.Add(new StringContent("0"), "arcrank");
            mm.Add(new StringContent("0"), "money");
            mm.Add(new StringContent(DateTime.Now.ToString("yyyy-MM-dd HH:mm")), "pubdate");
            mm.Add(new StringContent("0"), "ishtml");
            mm.Add(new StringContent(""), "filename");
            mm.Add(new StringContent(""), "templet");
            mm.Add(new StringContent("28"), "imageField.x");
            mm.Add(new StringContent("11"), "imageField.y");
            var response1 = httpClient.PostAsync(new Uri(DedeStaticValues.DedeAddarticleUrl), mm).Result;
            string result = response1.Content.ReadAsStringAsync().Result;
            if (result.Contains("成功发布文章"))
            {
                result = httpClient.GetStringAsync(new Uri(DedeStaticValues.DedeSitemapUrl)).Result;
                rs = true;
            }
            return rs;

        }

        private bool login()
        {
            httpClient = new HttpClient(handler);

            var response = httpClient.GetAsync(new Uri(DedeStaticValues.DedeLoginUrl)).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            if (result.Contains("常用操作"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 给提交的数据增加标记
        /// </summary>
        /// <param name="mm"></param>
        private void AddFlagIntoParm(MultipartFormDataContent mm)
        {
            Random rd = new Random((int)DateTime.Now.Ticks);
            if (rd.Next(1, 6) == 1)//头条
            {
                mm.Add(new StringContent("h"), "flags[]");
            }
            if (rd.Next(1, 6) == 2)//推荐
            {
                mm.Add(new StringContent("c"), "flags[]");
            }
            if (rd.Next(1, 6) == 3)//幻灯
            {
                mm.Add(new StringContent("f"), "flags[]");
            }
            if (rd.Next(1, 6) == 4)//特荐
            {
                mm.Add(new StringContent("a"), "flags[]");
            }
            if (rd.Next(1, 6) == 5)//滚动
            {
                mm.Add(new StringContent("s"), "flags[]");
            }
        }
    }
}
