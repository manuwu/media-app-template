using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace IVR.Language
{
    /// <summary>
    /// 本地化语言 组件 可支持动态的参数和多个语言本地化
    /// </summary>
    //[ExecuteInEditMode]
    //public class LanguageLocalization : MonoBehaviour
    //{
    //    enum Mode
    //    {
    //        /// <summary>
    //        /// 简单的语言 直接替换
    //        /// </summary>
    //        simple,
    //        /// <summary>
    //        /// 复杂的 需要组合
    //        /// </summary>
    //        complex,
    //    }

    //    //供开发时设置的文本 设置格式有多种
    //    public string text = string.Empty;


    //    //对于本地化的key
    //    private string[] keys = null;
    //    public string[] Keys
    //    {
    //        get
    //        {
    //            return keys;
    //        }
    //    }

    //    //设置过本地语言的文本 当设置动态文本时使用
    //    private string localtext = string.Empty;
    //    //最终的文本
    //    private string finaltext = string.Empty;
    //    //当前模式
    //    private Mode mode = Mode.simple;

    //    /// <summary>
    //    /// 最终的文本
    //    /// </summary>
    //    public string FinalText
    //    {
    //        get
    //        {
    //            return finaltext;
    //        }
    //        set
    //        {
    //            finaltext = value;

    //            //3D文本
    //            TextMesh txt_mesh = GetComponent<TextMesh>();
    //            if (txt_mesh != null)
    //            {
    //                txt_mesh.text = finaltext.Replace("\\n", "\n");
    //                if (Language.currentFont != null) txt_mesh.font = Language.currentFont;
    //                return;
    //            }

    //            //2DUI文本
    //            Text text = GetComponent<Text>();
    //            if (text != null)
    //            {
    //                text.text = finaltext.Replace("\\n", "\n");
    //                if (Language.currentFont != null) text.font = Language.currentFont;
    //                return;
    //            }

    //            //NGUI文本 TODO 根据需要打开注释 如果用的是UILabel 则打开此处 因为不是所有项目都导入了NGUI插件 所以先注释掉 
    //            Assembly assembly = Assembly.GetCallingAssembly();
    //            Type labletype = assembly.GetType("UILabel");
    //            if (labletype!=null)
    //            {
    //                var label = GetComponent("UILabel");
    //                labletype.GetProperty("text").SetValue(label, finaltext.Replace("\\n", "\n"), null);
    //                if (Language.currentFont!=null) labletype.GetProperty("ambigiousFont").SetValue(label, Language.currentFont, null);
    //                return;
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 设置本地化语言key 设置后立即刷新
    //    /// </summary>
    //    public string LocalizationKey
    //    {
    //        set
    //        {
    //            text = value;
    //            AnalyzeText();
    //            Language.Fresh(this);
    //        }
    //    }

    //    void Awake()
    //    {
    //        if (Application.isPlaying)
    //        {
    //            //解析文本 
    //            AnalyzeText();
    //            //初始化时 添加注册
    //            //LanguageManager.initLanguage(new XMLParser());//需要在程序开始时初始化
    //            //Language.Register(this);
    //            StartCoroutine("RegisterLL");
    //        }
    //    }
    //    void OnEnable()
    //    {
    //        if (Language.CheckOk())
    //        {
    //            Language.Fresh(this);
    //        }
    //    }

    //    private IEnumerator RegisterLL()
    //    {
    //        while (true)
    //        {
    //            yield return 1;
    //            if (Language.CheckOk())
    //            {
    //                break;
    //            }
    //        }
    //        Language.Register(this);
    //    }

    //    void OnDestory()
    //    {
    //        if (Application.isPlaying)
    //        {
    //            //销毁时 注销注册
    //            Language.UnRegister(this);
    //        }
    //    }

    //    /// <summary>
    //    /// 解析key
    //    /// </summary>
    //    void AnalyzeText()
    //    {
    //        //如果不是动态的 则不包含“{}”符号
    //        if (text.IndexOf("{") < 0 || text.IndexOf("}") < 0)
    //        {
    //            mode = Mode.simple;
    //            if (string.IsNullOrEmpty(text))
    //            {
    //                keys = null;
    //            }
    //            else
    //            {
    //                keys = new string[] { text };
    //            }
    //        }
    //        else
    //        {
    //            mode = Mode.complex;
    //            Regex fillter_regex = new Regex(@"[^\{\}\r\n]+(\{[^\{ \}]+\})");
    //            string fillter_str = fillter_regex.Replace(text, "$1");
    //            string[] str_arr = fillter_str.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
    //            List<string> list_keys = new List<string>();
    //            char first_char;
    //            string temp_str;
    //            for (int i = 0, length = str_arr.Length; i < length; i++)
    //            {
    //                temp_str = str_arr[i];
    //                first_char = temp_str[0];
    //                if (first_char.Equals('@'))
    //                {
    //                    list_keys.Add(temp_str);
    //                }
    //            }
    //            keys = list_keys.ToArray();
    //        }
    //        finaltext = text;
    //        localtext = string.Copy(text);
    //    }


    //    void Update()
    //    {
    //        if (Application.isPlaying)
    //        {
    //            return;
    //        }
    //        FinalText = text;
    //    }

    //    private bool dynamic_has_set = false;
    //    private string[] d_params;
    //    /// <summary>
    //    /// 设置文字中动态的部分
    //    /// </summary>
    //    /// <param name="_value"></param>
    //    public void SetDynamicText(params string[] _value)
    //    {
    //        d_params = _value;
    //        //只有设置过本地化语言 才可以设置动态部分
    //        if (has_set_local_text)
    //        {
    //            if (dynamic_has_set)
    //            {
    //                FinalText = string.Format(localtext, _value);
    //            }
    //            else
    //            {
    //                FinalText = string.Format(finaltext, _value);
    //            }
    //            dynamic_has_set = true;
    //        }
    //    }

    //    private bool has_set_local_text = false;
    //    /// <summary>
    //    /// 设置本地化语言
    //    /// </summary>
    //    /// <param name="_value"></param>
    //    public void SetLocalizationText(params string[] _value)
    //    {
    //        //如果没有找到本地化语言 则显示原始
    //        if (string.IsNullOrEmpty(_value[0]))
    //        {
    //            FinalText = text;
    //            return;
    //        }

    //        has_set_local_text = true;
    //        switch (mode)
    //        {
    //            case Mode.simple:
    //                localtext = _value[0];
    //                break;
    //            case Mode.complex:
    //                localtext = ReplaceLocalText(string.Copy(text), _value);
    //                break;
    //        }
    //        //恢复动态部分
    //        if (d_params != null && d_params.Length > 0)
    //        {
    //            FinalText = string.Format(localtext, d_params);
    //        }
    //        else
    //        {
    //            FinalText = localtext;
    //        }
    //    }
    //    /// <summary>
    //    /// 替换本地语言key 到value
    //    /// </summary>
    //    /// <param name="_txt"></param>
    //    /// <param name="_value"></param>
    //    /// <returns></returns>
    //    private string ReplaceLocalText(string _txt, params string[] _value)
    //    {
    //        string old_string = string.Empty;
    //        string new_string = string.Empty;
    //        for (int i = 0, length = _value.Length; i < length; i++)
    //        {
    //            old_string = "{" + keys[i] + "}";
    //            new_string = _value[i];
    //            _txt = _txt.Replace(old_string, new_string);
    //        }
    //        return _txt;
    //    }
    //}


    /// <summary>
    /// 本地化语言 组件 可支持动态的参数和多个语言本地化
    /// </summary>
    public class LanguageLocalization : MonoBehaviour
    {


        //供开发时设置的文本 设置格式有多种
        [SerializeField]
        private string text = string.Empty;
        private string finaltext = string.Empty;
        string curString = string.Empty;
        /// <summary>
        /// 最终的文本
        /// </summary>
        public string FinalText
        {
            get
            {
                return finaltext;
            }
            set
            {
                finaltext = value;
                curString = finaltext.Replace("\\n", "\n");
                //3D文本
                TextMesh txt_mesh = GetComponent<TextMesh>();
                if (txt_mesh != null)
                {
                    txt_mesh.text = curString;
                    if (Language.currentFont != null) txt_mesh.font = Language.currentFont;
                    return;
                }

                //2DUI文本
                Text text = GetComponent<Text>();
                if (text != null)
                {
                    text.text = curString;
                    if (Language.currentFont != null) text.font = Language.currentFont;
                    return;
                }

                //NGUI文本 TODO 根据需要打开注释 如果用的是UILabel 则打开此处 因为不是所有项目都导入了NGUI插件 所以先注释掉 
                Assembly assembly = Assembly.GetCallingAssembly();
                Type labletype = assembly.GetType("UILabel");
                if (labletype != null)
                {
                    var label = GetComponent("UILabel");
                    labletype.GetProperty("text").SetValue(label, curString, null);
                    if (Language.currentFont != null) labletype.GetProperty("ambigiousFont").SetValue(label, Language.currentFont, null);
                    return;
                }
            }
        }

        private void OnEnable()
        {
            FinalText = Language.Get(text);
            LanguageManager.Instance.OnLanguageChanged.AddListener(Language_Changed);
        }

        private void Language_Changed()
        {
            //3D文本
            TextMesh txt_mesh = GetComponent<TextMesh>();
            if (txt_mesh != null)
            {
                if (txt_mesh.text == curString)
                    FinalText = Language.Get(text);
                return;
            }
            //2DUI文本
            Text text2D = GetComponent<Text>();
            if (text2D != null)
            {
                if (text2D.text == curString)
                    FinalText = Language.Get(text);
                return;
            }

            //NGUI文本 TODO 根据需要打开注释 如果用的是UILabel 则打开此处 因为不是所有项目都导入了NGUI插件 所以先注释掉 
            Assembly assembly = Assembly.GetCallingAssembly();
            Type labletype = assembly.GetType("UILabel");
            if (labletype != null)
            {
                var label = GetComponent("UILabel");
                string str = (string)labletype.GetProperty("text").GetValue(label, null);
                if (str == curString)
                    FinalText = Language.Get(text);
                return;
            }
        }

        private void OnDisable()
        {
            LanguageManager.Instance.OnLanguageChanged.RemoveListener(Language_Changed);
        }

    }

}