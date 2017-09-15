using CYQ.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CYQ.Data.Table;

namespace WXGZHDownload.WXPost
{
    public class WXArticleDBHelper
    {
        private static string ConnectStr = "Data Source=" + System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "wx\\article.db;failifmissing=false";

        public static bool InseartDB(WXArticleInfo info,string _id)
        {
            bool rs = false;
            try
            {
                using (MAction _action = new MAction(TableNames.TB_WXARTICLE, ConnectStr))
                {
                    _action.Set(TB_WXARTICLE.id, _id);
                    _action.Set(TB_WXARTICLE.author, info.author);
                    _action.Set(TB_WXARTICLE.source, info.source);
                    _action.Set(TB_WXARTICLE.title, info.title);
                    _action.Set(TB_WXARTICLE.content, info.content);
                    _action.Set(TB_WXARTICLE.flag, 0);

                    rs = _action.Insert();
                }
            }
            catch (System.Exception ex)
            {
            	
            }


            return rs;
        }

        public static bool ResetArticleFlag(string id, int flag)
        {
            bool rs = false;

            using (MAction _action = new MAction(TableNames.TB_WXARTICLE, ConnectStr))
            {
                _action.Set(TB_WXARTICLE.flag, flag);

                rs = _action.Update("id='"+id+"'");
            }

            return rs;
        }

        public static List<WXArticleDBModel> GetArticleByFlag(int flag)
        {
            List<WXArticleDBModel> rs = new List<WXArticleDBModel>();

            using (MAction _action = new MAction(TableNames.TB_WXARTICLE, ConnectStr))
            {
                MDataTable dt = _action.Select("flag=" + flag);

                foreach (var item in dt.Rows)
                {
                    WXArticleDBModel obj = new WXArticleDBModel();
                    obj.id = item[TB_WXARTICLE.id].StringValue;
                    obj.author = item[TB_WXARTICLE.author].StringValue;
                    obj.source = item[TB_WXARTICLE.source].StringValue;
                    obj.title = item[TB_WXARTICLE.title].StringValue;
                    obj.content = item[TB_WXARTICLE.content].StringValue;
                    obj.flag = int.Parse(item[TB_WXARTICLE.flag].StringValue);

                    rs.Add(obj);
                }
            }

            return rs;
        }
    }
}
