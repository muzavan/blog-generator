using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wordpress.Business.Generating;

namespace Wordpress.Generator
{
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
}
