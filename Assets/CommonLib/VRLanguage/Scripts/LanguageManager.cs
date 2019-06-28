using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace IVR.Language
{

    /// <summary>
    /// 全局本地化语言设置中心
    /// </summary>
    public class LanguageManager : MonoBehaviour
    {

        //public static SystemLanguage CurrentLanguage
        //{
        //    get
        //    {
        //        return MainCommon.LauncherApp.GetLanguage();
        //    }
        //}
        private SystemLanguage m_previousLanguage;
        private struct TextActionParmes
        {
            public TestAction action;
            public Text text;
        }
        //public UnityEvent OnLanguageChanged = new UnityEvent;
        [Serializable]
        public class LanguageEvent : UnityEvent { }
        [SerializeField]
        private LanguageEvent m_LanguageChanged = new LanguageEvent();

        public LanguageEvent OnLanguageChanged
        {
            get { return m_LanguageChanged; }
        }

        public delegate void TestAction();

        private List<TextActionParmes> listaction = new List<TextActionParmes>();

        public void AddListener(Text target, TestAction testAction)
        {
            TextActionParmes findparmes = listaction.Find((TextActionParmes parms) => { return parms.text == target; });
            if (findparmes.text != null)
                listaction.Remove(findparmes);
            findparmes.text = target;
            findparmes.action = testAction;
            listaction.Add(new TextActionParmes() { action = testAction, text = target });

        }
        public void RemoveAllListener(TestAction testAction)
        {
            listaction.Clear();
        }

        private static LanguageManager mInstance;
        public static LanguageManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject game = new GameObject("[LanguageManager]");
                    mInstance = game.AddComponent<LanguageManager>();
                }
                return mInstance;

            }
        }

        public bool hadInited { get { return mInstance != null; } }
        private XMLParser mXMLParaser;
        public void Awake()
        {
            mInstance = this;
            mXMLParaser = new XMLParser();
            initLanguage(new XMLParser());
            OnLanguageChanged.AddListener(() =>
            {
                for (int i = 0; i < listaction.Count; i++)
                {
                    var item = listaction[i];
                    if (item.text == null)
                        listaction.Remove(item);
                    else
                    {
                        item.action.Invoke();
                    }

                }
            });

            //m_previousLanguage = Application.systemLanguage;
            m_previousLanguage = SystemCurrentLanguage.CurrentLanguage;

        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SystemCurrentLanguage.m_systemLanguage = SystemLanguage.English;
                setLanguage(mXMLParaser, SystemLanguage.English);
                if (OnLanguageChanged != null) OnLanguageChanged.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SystemCurrentLanguage.m_systemLanguage = SystemLanguage.Chinese;
                setLanguage(mXMLParaser, SystemLanguage.Chinese);
                if (OnLanguageChanged != null) OnLanguageChanged.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SystemCurrentLanguage.m_systemLanguage = SystemLanguage.Japanese;
                setLanguage(mXMLParaser, SystemLanguage.Japanese);
                if (OnLanguageChanged != null) OnLanguageChanged.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SystemCurrentLanguage.m_systemLanguage = SystemLanguage.Korean;
                setLanguage(mXMLParaser, SystemLanguage.Korean);
                if (OnLanguageChanged != null) OnLanguageChanged.Invoke();
            }
#endif
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                //MainCommon.LauncherApp.GetLanguage();
                //Debug.Log(Application.systemLanguage+",Test");
                Debug.Log("language:" + SystemCurrentLanguage.CurrentLanguage + "," + m_previousLanguage);
                if (SystemCurrentLanguage.CurrentLanguage != m_previousLanguage)
                {
                    setLanguage(mXMLParaser, SystemCurrentLanguage.CurrentLanguage);
                    if (OnLanguageChanged != null) OnLanguageChanged.Invoke();
                    m_previousLanguage = SystemCurrentLanguage.CurrentLanguage;
                }

            }
        }

        public void initLanguage(ILanguagePack languagePack)
        {
            SystemLanguage language = SystemCurrentLanguage.CurrentLanguage;
            setLanguage(languagePack, language);
        }

        private void setLanguage(ILanguagePack languagePack, SystemLanguage language)
        {
            Debug.Log("setLanguage:" + language);
            TextAsset xml;
            switch (language)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    xml = Resources.Load<TextAsset>("Local/chinese");
                    Language.ConfigAndSetLanguage(languagePack, xml.text, LanguageConst.CHINEISE);
                    //Language.currentFont = Font.CreateDynamicFontFromOSFont("Droid Sans Mono", 14);
                    break;
                case SystemLanguage.Japanese:
                    xml = Resources.Load<TextAsset>("Local/japanese");
                    Language.ConfigAndSetLanguage(languagePack, xml.text, LanguageConst.JAPANESE);
                    //Language.CurrentFont = Font.CreateDynamicFontFromOSFont("Noto Sans JP", 14);
                    break;
                case SystemLanguage.Korean:
                    xml = Resources.Load<TextAsset>("Local/korean");
                    Language.ConfigAndSetLanguage(languagePack, xml.text, LanguageConst.KOREAN);
                    break;
                default:
                    xml = Resources.Load<TextAsset>("Local/english");
                    Language.ConfigAndSetLanguage(languagePack, xml.text, LanguageConst.ENGLISH);
                    //Language.currentFont = Font.CreateDynamicFontFromOSFont("Droid Sans Mono", 14);
                    break;
            }
        }

        [Obsolete]
        public void initLanguageOld(ILanguagePack languagePack)
        {
            SystemLanguage language = Application.systemLanguage;
            TextAsset xml = Resources.Load<TextAsset>("Local/language");
            switch (language)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    Language.ConfigAndSetLanguage(languagePack, xml.text, LanguageConst.CHINEISE);
                    break;
                default:
                    Language.ConfigAndSetLanguage(languagePack, xml.text, LanguageConst.ENGLISH);
                    break;
            }
        }

    }
}