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

        int deep = 5;
        List<Link> links = new List<Link>();

        private void Form1_Load(object sender, EventArgs e)
        {
            string url = "https://yandex.ru";
            TreeNode linksNode = new TreeNode();
            GetTree(url, linksNode, deep);
            treeView1.Nodes.Add(linksNode);
            links.GroupBy(x => x.URL).Select(y => y.First());
            dgv.DataSource = links;
        }

        private void GetTree(string url, TreeNode node, int cntr)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            var response = request.GetResponseAsync().Result;
            var streamReader = new StreamReader(response.GetResponseStream());
            string html = streamReader.ReadToEnd();
            
            Match link = Regex.Match(html, "<\\s*a\\s+href=\"(http?.*?)\"", RegexOptions.Multiline);
            var urls = new List<string>();
            while (link.Success)
            {
                var urlHTML = link.Groups[1].Value.ToString();
                if (!urls.Contains(urlHTML))
                {
                    urls.Add(urlHTML);
                    links.Add(new Link { URL = urlHTML, Title = GetTitle(urlHTML) });
                }
                link = link.NextMatch();
            }

            foreach (var u in urls)
                node.Nodes.Add(u);

            if (cntr > 0)
            {
                foreach (TreeNode n in node.Nodes)
                {
                    GetTree(n.Text, n, cntr - 1);
                }
            }
            streamReader.Close();
        }

        public string GetTitle(string url)
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
