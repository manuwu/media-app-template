using System;
using System.Xml;
using UnityEngine;
using System.Collections.Generic;

namespace IVR.Language
{
    /// <summary>
    /// Xml格式的语言包 封装 
    /// </summary>
    public class XMLParser : XMLLanguagePack
    {

        private Dictionary<string, string> stringDict;

        public override string getLanguageByName(string _languagekey)
        {
            if (string.IsNullOrEmpty(_languagekey))
            {
                return string.Empty;
            }
            if (_languagekey[0].Equals('@'))
            {
                _languagekey = _languagekey.Substring(1, _languagekey.Length - 1);
            }
            if (stringDict.ContainsKey(_languagekey))
            {
                return stringDict[_languagekey];
            }
            return string.Empty;
        }

        protected override void init()
        {
            stringDict = new Dictionary<string, string>();
            XmlNodeList list = languageDoc.SelectNodes("/resources/string");
            foreach (XmlNode node in list)
            {
                stringDict[node.Attributes["name"].Value] = node.FirstChild.InnerText.Trim();
            }
        }
    }
}
