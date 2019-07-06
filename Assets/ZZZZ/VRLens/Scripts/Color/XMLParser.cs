using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

namespace SVR.Coloring
{
    public class XMLParser : XMLColoringPack
    {
        private Dictionary<string, string> stringDict;

        public override Color getColoringByName(string _coloringkey)
        {
            if (string.IsNullOrEmpty(_coloringkey))
            {
                return Color.white;
            }
            if (_coloringkey[0].Equals('@'))
            {
                _coloringkey = _coloringkey.Substring(1, _coloringkey.Length - 1);
            }
            if (stringDict.ContainsKey(_coloringkey))
            {
                string colorStr = stringDict[_coloringkey];
                string[] colorStrs = colorStr.Split(',');
                //r,g,b,a
                float r = float.Parse(colorStrs[0]) / 255.0f;
                float g = float.Parse(colorStrs[1]) / 255.0f;
                float b = float.Parse(colorStrs[2]) / 255.0f;
                float a = float.Parse(colorStrs[3]) / 255.0f;
                Color color = new Color(r, g, b, a);
                return color;
            }
            return Color.white;
        }

        protected override void init()
        {
            stringDict = new Dictionary<string, string>();
            XmlNodeList list = coloringDoc.SelectNodes("/resources/string");
            foreach (XmlNode node in list)
            {
                stringDict[node.Attributes["name"].Value] = node.FirstChild.InnerText.Trim();
            }
        }
    }
}
