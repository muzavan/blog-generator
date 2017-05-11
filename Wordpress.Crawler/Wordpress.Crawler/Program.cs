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
            // Please configure your project base path in AppConstant.PROJ_PATH, uncomment below config
            // Wordpress.Business.Util.AppConstant.PROJ_PATH = "";
            var url = "http://pandji.com/walk-away";
            var crawler = new Wordpress.Business.Crawling.WebCrawler(url);
            crawler.PrintProgress = true; // comment this line if you dont want to print the progress
            crawler.Crawl();
        }
    }
}
