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

        private void CreatePath(HtmlAgilityPack.HtmlDocument doc)
        {
            string user = doc.DocumentNode.SelectSingleNode("//*[@id='post-user']").InnerText;
            if(string.IsNullOrWhiteSpace(user))
            {
                user = Guid.NewGuid().ToString("N");
            }

            strtitle = doc.DocumentNode.SelectSingleNode("//title").InnerText;
            if (string.IsNullOrWhiteSpace(strtitle))
            {
                strtitle = Guid.NewGuid().ToString("N");
            }

            strhtmlpath = user + "\\" + strtitle;

            strimagepath = strhtmlpath + "\\images";

            createdir(strimagepath);
        }

        private void createdir(string filefullpath)
        {
            if (!File.Exists(filefullpath))
            {
                string[] pathes = filefullpath.Split('\\');
                if (pathes.Length > 1)
                {
                    string path = pathes[0];
                    for (int i = 1; i < pathes.Length; i++)
                    {
                        path += "\\" + pathes[i];
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox2.Text = "";

            string urlStr = this.textBox1.Text;

            HttpClient client = new HttpClient();

            string rs = client.GetStringAsync(urlStr).Result;

            HtmlAgilityPack.HtmlDocument doccc = new HtmlAgilityPack.HtmlDocument();

            doccc.LoadHtml(rs);

            CreatePath(doccc);

            HtmlAgilityPack.HtmlNodeCollection ImagePtahs = doccc.DocumentNode.SelectNodes("//img[@data-src]");

            foreach(var item in ImagePtahs)
            {
                string imageUrl = item.Attributes["data-src"].Value;

                this.textBox2.Text += imageUrl + "\r\n";

                Stream inStream = client.GetStreamAsync(imageUrl).Result;

                string[] lists = imageUrl.Split(new char[] { '?','&'});
                string houzhui = "";
                foreach(string itempit in lists)
                {
                    if(itempit.Contains("wx_fmt="))
                    {
                        houzhui = itempit.Replace("wx_fmt=", "");
                        break;
                    }
                }

                string imagename = Guid.NewGuid().ToString("N");
                if (string.IsNullOrWhiteSpace(houzhui)==false)
                {
                    imagename = imagename + "." + houzhui;
                }

                item.Attributes.Add("src", "images\\"+imagename);

                byte[] buffer = new byte[1024];
                Stream outStream = System.IO.File.Create(strimagepath + "\\"+ imagename);
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
            }

            doccc.Save(strhtmlpath + "\\" + strtitle + ".html");

            this.textBox2.Text += "下载完成！";

            this.textBox2.Focus();
            this.textBox2.Select(this.textBox2.TextLength, 0);
            this.textBox2.ScrollToCaret();
        }

        private void CreateDir(string titlestr)
        {
            System.IO.Directory.CreateDirectory(titlestr);
        }
    }
}
