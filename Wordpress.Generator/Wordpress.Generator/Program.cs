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
            dictBuilder.ReadDictionary();

            var generator = new Wordpress.Business.Generating.Generator(dictBuilder.Dictionary);
            generator.Generate(20);

        }
    }
}
