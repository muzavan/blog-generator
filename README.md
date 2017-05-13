# blog-generator
Blog Generator is blog generation executable which used markov chain based on previous posts on blog, to create new post. This is inspired by my fellow [Calvin] (https://github.com/calvinsadewa/poem_generator).

# Configuration

Please find the configuration for the projects below.
```
    public class AppConstant
    {
        public static string PROJ_PATH = "D:\\Proyek\\blog-generator\\"; // Change it to your project location (ending with\\) 
    }

    public class CrawlingConstant
    {
        public static string TITLE_SELECTOR = "h1.entry-title"; // Change it to the selector of html element contained title
        public static string CONTENT_SELECTOR = "div.entry-content"; // Change it to the selector of html element contained main content
        public static string PREVIOUS_SELECTOR = ".nav-previous > a[rel=\"prev\"]"; // Change it to the selector of html element contained previous post
    }
```
(TODO: Move configuration to external file, not constant file)

# Run
Currently, please clone/download this repo and run it from VisualStudio. (Will update with executable, later)

There are two solutions in this repository.

## Wordpress.Crawler
This solution will help you to crawl content from blog (used to build the word dictionary later), the result can be seen in `<proj>\Data\crawled-posts.xml`

Please configure the Program.cs accordingly, by defining last post url from blog you want to use.
```
    class Program
    {
        static void Main(string[] args)
        {
            // Please configure your project base path in AppConstant.PROJ_PATH, uncomment below config
            // Wordpress.Business.Util.AppConstant.PROJ_PATH = "";
            var url = "http://pandji.com/walk-away";
            var crawler = new Wordpress.Business.Crawling.WebCrawler(url); // you can define crawled limit by adding second parameter in this constructor, if not defined, the default limit is 100
            crawler.PrintProgress = true; // comment this line if you dont want to print the progress
            crawler.Crawl();
        }
    }

```

## Wordpress.Generator
This solution will generate content based on data you crawled from previous solution. 

Please configure the Program.cs accordingly.
```
    class Program
    {
        static void Main(string[] args)
        {
            var dictBuilder = new DictionaryBuilder();
            dictBuilder.BuildDictionary();
            // dictBuilder.ReadDictionary(); // Used this one if you already build the dictionary, which you can check in GeneratingConstant.DICT_PATH xml

            var generator = new Wordpress.Business.Generating.Generator(dictBuilder.Dictionary); // you can set the content size by adding param in this constructor, currently the default content size is 200 word.
            generator.Generate(20); // 20 is the number of generated content

        }
    }
```
(TODO : Adding dll and exe to Repo)

# Example
Currently, in `<proj>\Data` there are some example from my local test. I used [Pandji's blog](http://pandji.com) as my sample. 

# TODO
1. Removing html encoded string from content.
2. Adding stop words, currently generated content dont have proper stop word.
3. Move configuration to external file, not constant file
4. TODO : Adding dll and exe to Repo
5. ?
