using System;
using System.Xml;
using UnityEngine;

namespace IVR.Language
{
    /// <summary>
    /// Xml格式的语言包 封装 
    /// </summary>
    public abstract class XMLLanguagePack : ILanguagePack
    {

        protected XmlDocument languageDoc;
        private int language;


        public XMLLanguagePack()
        {

        }


        /// <summary>
        /// 从Resources路径初始化xml
        /// </summary>
        /// <param name="_fileName"></param>
        public void InitXmlFromResourcesPath(string _fileName)
        {
            TextAsset xmlFile = (TextAsset)Resources.Load(_fileName, typeof(TextAsset));
            InitXmlFromString(xmlFile.text);
        }

        /// <summary>
        /// 从字符初始化xml
        /// </summary>
        /// <param name="_xml"></param>
        public void InitXmlFromString(string _xml)
        {
            languageDoc = new XmlDocument();
            languageDoc.LoadXml(_xml);
            init();
        }

        

        public string Get(string _key)
        {
            return getLanguageByName(_key);
        }

        public void SetLanguage(int _language)
        {
            language = _language;
        }

        protected abstract void init();
        public abstract string getLanguageByName(string _languagekey);
    }
}
