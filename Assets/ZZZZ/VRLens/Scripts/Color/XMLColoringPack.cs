using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

namespace SVR.Coloring
{
    public abstract class XMLColoringPack : ColoringPack
    {
        protected XmlDocument coloringDoc;
        private int colorTheme;

        public XMLColoringPack()
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
            coloringDoc = new XmlDocument();
            coloringDoc.LoadXml(_xml);
            init();
        }

        /// <summary>
        /// 从xml里获取key对应的颜色
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        public Color Get(string _key)
        {
            return getColoringByName(_key);
        }

        public void SetColorTheme(int _colorTheme)
        {
            colorTheme = _colorTheme;
        }

        protected abstract void init();
        public abstract Color getColoringByName(string _languagekey);
    }
}
