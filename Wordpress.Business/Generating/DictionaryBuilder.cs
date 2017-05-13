using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Wordpress.Business.Util;

namespace Wordpress.Business.Generating
{
    public class DictionaryBuilder
    {
        #region Private Property
        private XmlDocument _xmlDoc;
        private Dictionary<string, Dictionary<string, double>> _doubleWordDictionary;
        #endregion

        #region Public Method
        public Dictionary<string, Dictionary<string, double>> Dictionary{
            get{
                return _doubleWordDictionary;
            }
        }
        public DictionaryBuilder()
        {
            _xmlDoc = new XmlDocument();
            _xmlDoc.Load(CrawlingConstant.XML_PATH);

            _doubleWordDictionary = new Dictionary<string, Dictionary<string, double>>();
        }

        public void BuildDictionary(bool persist = true)
        {
            _doubleWordDictionary.Clear();
            _BuildDictionary();

            if(persist)
            {
                _WriteToXml();
            }
        }

        /// <summary>
        /// Call this method to initialize Word Dictionary by reading xml. Make sure you have the words-dictionary.xml filled
        /// </summary>
        public void ReadDictionary()
        {
            _doubleWordDictionary.Clear();
            _ReadDictionary();
        }
        #endregion

        #region Private Method
        private void _BuildDictionary()
        {
            var intWordDictionary = new Dictionary<string, Dictionary<string, int>>();
            var postNodes = _xmlDoc.GetElementsByTagName("post");
            if(postNodes == null)
            {
                return;
            }

            // Regex Evaluator => Remove non-alphabet and multiple whitespaces
            var nonAlphaRegex = new Regex(@"[^a-z]");
            var whitespaceRegex = new Regex(@"\s+");

            for (var idx = 0; idx < postNodes.Count; idx++)
            {
                var postNode = postNodes.Item(idx) as XmlElement;
                var contentNode = postNode.LastChild as XmlElement;

                // Remove non-alphabet and whitespaces, then trim it
                var contentStr = whitespaceRegex.Replace(nonAlphaRegex.Replace(contentNode.InnerText.ToLower()," ")," ").Trim();
                var words = contentStr.Split(' ');
                
                var prevWord = "";
                for (var conIdx = 0; conIdx < words.Length; conIdx++ )
                {
                    var word = words[conIdx];
                    if(!intWordDictionary.ContainsKey(prevWord))
                    {
                        intWordDictionary[prevWord] = new Dictionary<string, int>();
                    }
                    if (!intWordDictionary[prevWord].ContainsKey(word))
                    {
                        intWordDictionary[prevWord][word] = 0;
                    }

                    // Increment the table
                    intWordDictionary[prevWord][word] = intWordDictionary[prevWord][word] + 1;
                    prevWord = word;
                }
            }

            _ToDoubleWordDictionary(intWordDictionary);
        }

        private void _WriteToXml()
        {
            var _dictXmlWord = new XmlDocument();

            var wordsNode = _dictXmlWord.CreateElement("words");

            foreach(var pair in _doubleWordDictionary)
            {
                var wordNode = _dictXmlWord.CreateElement("word");
                wordNode.InnerText = pair.Key;

                foreach(var nextPair in pair.Value)
                {
                    var nextNode = _dictXmlWord.CreateElement("next");
                    nextNode.SetAttribute("word",nextPair.Key);
                    nextNode.SetAttribute("prob", nextPair.Value.ToString());

                    wordNode.AppendChild(nextNode);
                }

                wordsNode.AppendChild(wordNode);
            }

            _dictXmlWord.AppendChild(wordsNode);
            _dictXmlWord.Save(GeneratingConstant.DICT_PATH);
        }

        private void _ReadDictionary()
        {
            var _wordDoc = new XmlDocument();
            _wordDoc.Load(GeneratingConstant.DICT_PATH);

            var wordNodes = _wordDoc.SelectNodes("words/word");
            
            for (var idx = 0; idx < wordNodes.Count; idx++)
            {
                var wordNode = wordNodes.Item(idx);
                var word1 = wordNode.InnerText;
                _doubleWordDictionary[word1] = new Dictionary<string, double>();

                var nextNodes = wordNode.SelectNodes("next");
                for (var cidx = 0; cidx < nextNodes.Count; cidx++)
                {
                    var nextNode = nextNodes.Item(cidx) as XmlElement;
                    var word2 = nextNode.GetAttribute("word");
                    var count = Convert.ToDouble(nextNode.GetAttribute("prob"));
                    _doubleWordDictionary[word1][word2] = count;
                }
            }
        }

        private void _ToDoubleWordDictionary(Dictionary<string, Dictionary<string, int>> intWordDictionary)
        {
            foreach (var pair in intWordDictionary)
            {
                _doubleWordDictionary[pair.Key] = new Dictionary<string, double>();
                var sum = Convert.ToDouble(pair.Value.Sum(x => x.Value));
                
                foreach (var nextPair in pair.Value)
                {
                    _doubleWordDictionary[pair.Key][nextPair.Key] = nextPair.Value / sum;
                }
            }
        }
        #endregion
    }
}
