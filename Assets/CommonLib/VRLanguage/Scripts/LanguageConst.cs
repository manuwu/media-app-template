using System;
using UnityEngine;

namespace IVR.Language
{
    /// <summary>
    /// 需要常量
    /// </summary>
    public static class LanguageConst
    {
        //中文
        public const int CHINEISE = 0;
        //英文
        public const int ENGLISH = 1;
        public const int JAPANESE = 2;
        public const int KOREAN = 3;
    }

    public static class SystemCurrentLanguage
    {
#if UNITY_EDITOR
        public static SystemLanguage m_systemLanguage = SystemLanguage.Chinese;
#endif
        public static SystemLanguage CurrentLanguage
        {
            get
            {
#if UNITY_EDITOR
                return m_systemLanguage;
#else
                SystemLanguage systemLanguage = SystemLanguage.Chinese;
                AndroidJavaClass localclass = new AndroidJavaClass("java.util.Locale");
                AndroidJavaObject localobject = localclass.CallStatic<AndroidJavaObject>("getDefault");
                string language = localobject.Call<string>("getLanguage");
                string country = localobject.Call<string>("getCountry");

                if (language == "zh" && country == "CN")
                {
                    systemLanguage = SystemLanguage.Chinese;
                }
                else if(language == "ja" && country == "JP")
                {
                    systemLanguage = SystemLanguage.Japanese;
                }
                else if (language == "ko" && country == "KR")
                {
                    systemLanguage = SystemLanguage.Korean;
                }
                else
                {
                    systemLanguage = SystemLanguage.English;
                }
                return systemLanguage;
#endif
            }
        }

    }
}
