using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using WXGZHDownload.DeDe;
using WXGZHDownload.WXPost;

namespace WXGZHDownload
{
    public partial class Form1 : Form
    {
        string strhtmlpath = "";
        string strimagepath = "";
        string strtitle = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string urlStr = this.textBox1.Text;
            this.webBrowser1.Url = new System.Uri(urlStr);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string _allhtmlinfo = this.webBrowser1.DocumentText;
            string _bodyhtmlinfo = this.webBrowser1.Document.Body.InnerHtml;

            this.webBrowser1.Stop();

            WXArticleInfo rs = WXArticleAdapter.GetArticle(_bodyhtmlinfo);

            string articleid = Guid.NewGuid().ToString("N");

            bool dbrs = WXArticleDBHelper.InseartDB(rs, articleid);

            if (dbrs)
            {
                this.textBox2.Text += "操作完成已写入数据库！";
            }
            else
            {
                this.textBox2.Text += "操作失败未写入数据库！";
            }

            WXGZHDownload.DeDe.DeDePublic1 dede = new WXGZHDownload.DeDe.DeDePublic1();
            if (dede.run(rs))
            {
                this.textBox2.Text += "已发布到网站！";

                if (WXArticleDBHelper.ResetArticleFlag(articleid,1))
                {
                    this.textBox2.Text += "数据库更新成功！";
                }
                else
                {
                    this.textBox2.Text += "数据库更新失败！";
                }
            }
            else
            {
                this.textBox2.Text += "未发布到网站！";
            }
            this.textBox2.Text += "采集及发布过程结束！\r\n";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (var item in DedeStaticValues.DicArticleType.Values)
            {
                this.comboBox1.Items.Add(item);
            }
            if (this.comboBox1.Items.Count > 0)
            {
                this.comboBox1.SelectedIndex = 0;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DedeStaticValues.SetCurType(this.comboBox1.Text);
        }
    }
}
