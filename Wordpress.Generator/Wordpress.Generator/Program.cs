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
            var generator = new DictionaryBuilder();
            generator.BuildDictionary();


        }
    }
}
