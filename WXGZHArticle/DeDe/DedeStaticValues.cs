using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXGZHDownload.DeDe
{
    public class DedeStaticValues
    {
        public static int SelectedrticleType = 1;

        public static Dictionary<int, string> DicArticleType = new Dictionary<int, string>();

        public static string DedeLoginUrl = "";
        public static string DedeAddarticleUrl = "";
        public static string DedeSitemapUrl = "";

        public static string ALBCAK = "阿里百川ak";
        public static string ALBCSK = "阿里百川sk";

        static DedeStaticValues()
        {
            DicArticleType.Add(1, "资讯");
            DicArticleType.Add(2, "生活");

            DedeLoginUrl = "http://localhost/dede/login2.php";
            DedeAddarticleUrl = "http://localhost/dede/article_add.php";
            DedeSitemapUrl = "http://localhost/dede/makehtml_map.php?dopost=site";
        }

        public static void SetCurType(string _value)
        {
            foreach(var item in DicArticleType)
            {
                if (_value == item.Value)
                {
                    SelectedrticleType = item.Key;
                    break;
                }
            }
        }
    }
}
