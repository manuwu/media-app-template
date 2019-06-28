using UnityEngine;
using System;
using System.Collections.Generic;

namespace IVR.Language
{

    /// <summary>
    /// 全局本地化语言设置中心
    /// </summary>
    public static class Language
    {
        private static ILanguagePack language_pack;
        private static Dictionary<int, LanguageLocalization> dict_localizations;
        private static int language;

        public delegate void OnLanguageChanged ();

        public static event OnLanguageChanged onLanguageChanged;

        public static Font currentFont;

        public static Font CurrentFont {
            get {
                if (currentFont == null) {
                    SystemLanguage language = Application.systemLanguage;
                    switch (language) {
                    case SystemLanguage.Japanese:
                        //currentFont = Font.CreateDynamicFontFromOSFont ("Noto Sans JP", 14);
                        break;
                    }
                }
                return currentFont;
            }
            set {
                currentFont = value;
            }
        }

        static Language ()
        {
            bool isinited = LanguageManager.Instance.hadInited;
            dict_localizations = new Dictionary<int, LanguageLocalization> ();
        }

        /// <summary>
        /// 判断是否准备好了
        /// </summary>
        /// <returns></returns>
        public static bool CheckOk ()
        {
            return language_pack != null;
        }

        /// <summary>
        /// 配置语言包
        /// </summary>
        /// <param name="_get"></param>
        public static void Config (ILanguagePack _get)
        {
            language_pack = _get;
        }

        public static void ConfigAndSetLanguage (ILanguagePack _get, string text, int _language)
        {
          
            _get.InitXmlFromString (text);
            Language.Config (_get);
            Language.Switch (_language);
        }

        /// <summary>
        /// 返回指定key的语言
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        private static string Get (string _key)
        {
            return language_pack.Get (_key).Replace("\\n", "\n");
        }
        /// <summary>
        /// 根据提供的Key设置对应的文字
        /// </summary>
        /// <param name="text"></param>
        /// <param name="_key"></param>
        public static void SetTextByKey(this UnityEngine.UI.Text text, string _key)
        {
            if(currentFont!=null) text.font = currentFont;
            string curString = Get(_key);
            text.text = curString;
            LanguageManager.Instance.AddListener(text,()=>{

                //Debug.LogFormat("{0},{1}", text, _key);
                if (text.text == curString)
                {
                    curString = Get(_key);
                    text.text = curString;
                }
            });

        }

        /// <summary>
        /// Key是Format类型
        /// </summary>
        /// <param name="text"></param>
        /// <param name="_key"></param>
        /// <param name="args"></param>
        public static void SetTextByKey(this UnityEngine.UI.Text text, string _key,params object[] args)
        {
            if (currentFont != null) text.font = currentFont;
            string curString = string.Format(Get(_key), args);
            text.text = curString;
            //LanguageManager.Instance.OnLanguageChanged.AddListener((UnityEngine.UI.Text _text) =>
            //{
            //    Debug.LogFormat("{0},{1}", _text.name, _key);
            //    _text.text = string.Format(Get(_key), args);
            //}, text);
            LanguageManager.Instance.AddListener(text, () => {

                if (text.text == curString)
                {
                    curString = string.Format(Get(_key), args);
                    text.text = curString;
                }
            });
        }
        public static void SetText(this UnityEngine.UI.Text text, string str)
        {
            if (currentFont != null) text.font = currentFont;
            text.text = str;
        }
        public static string Get(string key, params object[] args)
        {
            string val = language_pack.Get(key);
            return string.Format(val, args); 
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="_ll"></param>
        public static void Register (LanguageLocalization _ll)
        {
            int key = _ll.GetInstanceID ();
            if (dict_localizations.ContainsKey (key)) {
                return;
            }
            dict_localizations.Add (key, _ll);
            //添加过后需要刷新
            //Fresh (_ll);
        }

        /// <summary>
        /// 解除注册
        /// </summary>
        /// <param name="_ll"></param>
        public static void UnRegister (LanguageLocalization _ll)
        {
            int key = _ll.GetInstanceID ();
            dict_localizations.Remove (key);
        }

        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="_language"></param>
        public static void Switch (int _language)
        {
            language = _language;
            //设置语言包中的当前语言环境
            language_pack.SetLanguage (_language);

            //Dictionary<int, LanguageLocalization>.Enumerator er = dict_localizations.GetEnumerator ();
            //while (er.MoveNext ()) {
            //    Fresh (er.Current.Value);
            //}

            //if (onLanguageChanged != null) {
            //    onLanguageChanged ();
            //}
        }

        /// <summary>
        /// 返回当前的语言
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentLanguage ()
        {
            return language;
        }

        /// <summary>
        /// 刷新单个本地化组件
        /// </summary>
        /// <param name="_ll"></param>
        //public static void Fresh (LanguageLocalization _ll)
        //{
        //    string[] keys = _ll.Keys;
        //    if (keys == null || keys.Length == 0) {
        //        return;
        //    }
        //    List<string> languages = new List<string> ();
        //    for (int i = 0, length = keys.Length; i < length; i++) {
        //        languages.Add (language_pack.Get (keys [i]));
        //    }
        //    _ll.SetLocalizationText (languages.ToArray ());
        //}
    }
}