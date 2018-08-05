using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using Parser.Models;

namespace Parser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        List<Link> links = new List<Link>();

        private void Form1_Load(object sender, EventArgs e)
        {
            string url = "https://youtube.com";
            GetUrls(url);
            dgv.DataSource = links;
        }

        private void GetUrls(string url)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            var response = request.GetResponseAsync().Result;
            var streamReader = new StreamReader(response.GetResponseStream());
            string html = streamReader.ReadToEnd();
            Match link = Regex.Match(html, "<\\s*a\\s+href=\"(http?.*?)\"", RegexOptions.Multiline);
            while (link.Success)
            {
                var urlHTML = link.Groups[1].Value.ToString();
                var item = new Link() { URL = urlHTML, Title = GetTitle(urlHTML) };
                if(!links.Exists(x => x.URL == urlHTML))
                {
                    links.Add(item);
                }
                link = link.NextMatch();
            }
            

            streamReader.Close();
        }

        private string GetTitle(string url)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            var response = request.GetResponseAsync().Result;
            var streamReader = new StreamReader(response.GetResponseStream());
            string html = streamReader.ReadToEnd();
            Match title = Regex.Match(html, "<title>(.*?)</title>", RegexOptions.Multiline);
            return title.Groups[1].Value.ToString();
        }
    }
}
