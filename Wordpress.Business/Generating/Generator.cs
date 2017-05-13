using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Wordpress.Business.Util;

namespace Wordpress.Business.Generating
{
    public class Generator
    {
        #region Private Property
        private int _textSize;
        private Dictionary<string, Dictionary<string, double>> _wordDictionary;
        private Random _random;
        #endregion

        #region Public Method
        public Generator(Dictionary<string, Dictionary<string, double>> wordDictionary, int textSize = 200)
        {
            _wordDictionary = wordDictionary;
            _textSize = textSize;
            _random = new Random();
        }

        /// <summary>
        /// Generate content based on dictionary defined in constructor
        /// </summary>
        /// <param name="number">Number of content generated</param>
        public void Generate(int number)
        {
            var generated = new List<string>();
            
            while(generated.Count < number)
            {
                generated.Add(_GenerateContent());
            }

            _WriteGenerated(generated);
        }
        
        #endregion

        #region Private Method
        /// <summary>
        /// If you defined textSize in Generate, will override the default textSize (defined in constructor) in this current generation
        /// </summary>
        /// <param name="textSize"></param>
        /// <returns></returns>
        private string _GenerateContent(int textSize = 0)
        {
            var content = new StringBuilder();
            var maxsize = textSize != 0 ? textSize : _textSize;

            var prevWord = "";
            var size = 1;
            var keys = _wordDictionary.Keys;

            while (size <= maxsize)
            {
                var doub = _random.NextDouble();
                var word = string.Empty;

                if (!_wordDictionary.ContainsKey(prevWord))
                {
                    // Pick a random word
                    var randomIdx = _random.Next(keys.Count);
                    word = keys.ElementAt(randomIdx);
                }
                else
                {
                    foreach (var pair in _wordDictionary[prevWord])
                    {
                        doub -= pair.Value;

                        if (doub <= 0)
                        {
                            word = pair.Key;
                            break;
                        }
                    }
                }

                content.AppendFormat("{0} ", word);
                prevWord = word;
                size++;
            }

            return content.ToString();
        }

        private void _WriteGenerated(List<string> contents)
        {
            var genDoc = new XmlDocument();
            var postsNode = genDoc.CreateElement("posts");

            foreach(var content in contents)
            {
                var postNode = genDoc.CreateElement("post");
                postNode.InnerText = content;

                postsNode.AppendChild(postNode);
            }

            genDoc.AppendChild(postsNode);
            genDoc.Save(GeneratingConstant.GEN_PATH);
        }
        #endregion
    }
}
