using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordpress.Business.Util
{
    public class AppConstant
    {
        public static string PROJ_PATH = "D:\\Proyek\\blog-generator\\";
    }

    public class CrawlingConstant
    {
        public static string TITLE_SELECTOR = "h1.entry-title";
        public static string CONTENT_SELECTOR = "div.entry-content";
        public static string PREVIOUS_SELECTOR = ".nav-previous > a[rel=\"prev\"]";

        public static string XML_PATH = string.Format("{0}{1}",AppConstant.PROJ_PATH,"Data\\crawled-posts.xml");
    }

    public class GeneratingConstant
    {
        public static string DICT_PATH = string.Format("{0}{1}", AppConstant.PROJ_PATH, "Data\\words-dictionary.xml");
    }
}
