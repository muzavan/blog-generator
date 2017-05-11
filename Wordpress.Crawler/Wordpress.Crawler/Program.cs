using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordpress.Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://pandji.com/walk-away";
            var crawler = new Wordpress.Business.Crawling.WebCrawler(url);
            crawler.Crawl();
        }
    }
}
