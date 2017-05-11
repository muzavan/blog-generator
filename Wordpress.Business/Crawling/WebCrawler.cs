using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Wordpress.Business.Util;

namespace Wordpress.Business.Crawling
{
    public class WebCrawler
    {
        #region Private Propertys
        private string _latestPostUrl;
        private int _postLimit;
        private XmlDocument _xmlDoc;
        #endregion

        #region Public Method
        public WebCrawler(string latestPostUrl, int postLimit = 100)
        {
            _latestPostUrl = latestPostUrl;
            _postLimit = postLimit;
            _xmlDoc = new XmlDocument();
            _xmlDoc.Load(CrawlingConstant.XML_PATH);
        }

        public int Crawl()
        {
            int count = 0;
            string nextUrl = _latestPostUrl;
            
            while(!string.IsNullOrWhiteSpace(nextUrl) && count < _postLimit)
            {
                nextUrl = _ScrapeUrl(nextUrl);
                count++;
            }
            return count;
        }
        #endregion

        #region Private Method
        private string _ScrapeUrl(string url)
        {
            string nextUrl = "";
            string htmlText = _GetContent(url);

            var document = new HtmlDocument();
            document.LoadHtml(htmlText);

            // Finding Title
            var titleNodes = document.QuerySelectorAll(CrawlingConstant.TITLE_SELECTOR);
            var titleString = new StringBuilder();
            foreach(var titleNode in titleNodes){                
                titleString.Append(titleNode.InnerText + Environment.NewLine);
            }

            // Finding Content
            var contentNodes = document.QuerySelectorAll(CrawlingConstant.CONTENT_SELECTOR);
            var contentString = new StringBuilder();
            foreach (var contentNode in contentNodes)
            {
                contentString.Append(contentNode.InnerText + Environment.NewLine);
            }

            // Finding NextUrl
            var prevNode = document.QuerySelector(CrawlingConstant.PREVIOUS_SELECTOR);
            if(prevNode != null)
            {
                nextUrl = prevNode.GetAttributeValue("href","");
            }

            // Write Title and Content to XML
            _WritePost(titleString.ToString(),contentString.ToString());

            return nextUrl;
        }

        private string _GetContent(string url)
        {
            var htmlText = "";

            // Make Request, Get Content as Html
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.UserAgent = "Crawling Agent";
            var response = request.GetResponse();
            var stream = response.GetResponseStream();
            var reader = new StreamReader(stream);
            htmlText = reader.ReadToEnd();
            return htmlText;
        }

        private void _WritePost(string title, string content)
        {
            if(_xmlDoc.FirstChild == null)
            {
                var nposts = _xmlDoc.CreateElement("posts");
                _xmlDoc.AppendChild(nposts);
            }

            var posts = _xmlDoc.FirstChild;
            var post = _xmlDoc.CreateElement("post");
            var titleNode = _xmlDoc.CreateElement("title");
            titleNode.InnerText = title;
            var contentNode = _xmlDoc.CreateElement("content");
            contentNode.InnerText = content;

            post.AppendChild(titleNode);
            post.AppendChild(contentNode);

            posts.AppendChild(post);
            _xmlDoc.Save(CrawlingConstant.XML_PATH);
        }
        #endregion
    }
}
