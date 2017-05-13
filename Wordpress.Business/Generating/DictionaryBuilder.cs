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
        private Dictionary<string, Dictionary<string, int>> _wordDictionary;
        #endregion

        #region Public Method
        public Dictionary<string, Dictionary<string, int>> Dictionary{
            get{
                return _wordDictionary;
            }
        }
        public DictionaryBuilder()
        {
            _xmlDoc = new XmlDocument();
            _xmlDoc.Load(CrawlingConstant.XML_PATH);

            _wordDictionary = new Dictionary<string, Dictionary<string, int>>();
        }

        public void BuildDictionary(bool persist = true)
        {
            _wordDictionary.Clear();
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
            _wordDictionary.Clear();
            _ReadDictionary();
        }
        #endregion

        #region Private Method
        private void _BuildDictionary()
        {
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
                    if(!_wordDictionary.ContainsKey(prevWord))
                    {
                        _wordDictionary[prevWord] = new Dictionary<string, int>();
                    }
                    if (!_wordDictionary[prevWord].ContainsKey(word))
                    {
                        _wordDictionary[prevWord][word] = 0;
                    }

                    // Increment the table
                    _wordDictionary[prevWord][word] = _wordDictionary[prevWord][word] + 1;
                    prevWord = word;
                }
            }
        }

        private void _WriteToXml()
        {
            var _dictXmlWord = new XmlDocument();

            var wordsNode = _dictXmlWord.CreateElement("words");

            foreach(var pair in _wordDictionary)
            {
                var wordNode = _dictXmlWord.CreateElement("word");
                wordNode.InnerText = pair.Key;

                foreach(var nextPair in pair.Value)
                {
                    var nextNode = _dictXmlWord.CreateElement("next");
                    nextNode.SetAttribute("word",nextPair.Key);
                    nextNode.SetAttribute("count", nextPair.Value.ToString());

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
                _wordDictionary[word1] = new Dictionary<string, int>();

                var nextNodes = wordNode.SelectNodes("next");
                for (var cidx = 0; cidx < nextNodes.Count; cidx++)
                {
                    var nextNode = nextNodes.Item(cidx) as XmlElement;
                    var word2 = nextNode.GetAttribute("word");
                    var count = Convert.ToInt32(nextNode.GetAttribute("count"));
                    _wordDictionary[word1][word2] = count;
                }
            }
        }
        #endregion
    }
}
