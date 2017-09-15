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

        static DedeStaticValues()
        {
            DicArticleType.Add(1, "资讯");
            DicArticleType.Add(2, "生活");

            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            DedeLoginUrl = config.AppSettings.Settings["DedeLoginUrl"].Value;
            DedeAddarticleUrl = config.AppSettings.Settings["DedeAddFileUrl"].Value;
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
