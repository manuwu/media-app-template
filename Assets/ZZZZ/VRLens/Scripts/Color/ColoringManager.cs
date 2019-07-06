using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

namespace SVR.Coloring
{
    public enum ColorTheme
    {
        Green = 0,
        Red = 1,
        Blue = 2
    }

    public class ColoringManager : MonoBehaviour
    {
        [HideInInspector]
        public ColorTheme CurrentColorTheme = ColorTheme.Green;
        private ColorTheme m_previousColorTheme;
        private struct TextActionParmes
        {
            public TextAction action;
            public Text text;
        }
        private struct ImageActionParmes
        {
            public ImageAction action;
            public Image image;
        }

        [Serializable]
        public class LanguageEvent : UnityEvent { }
        [SerializeField]
        private LanguageEvent m_ColoringChanged = new LanguageEvent();

        public LanguageEvent OnColoringChanged
        {
            get { return m_ColoringChanged; }
        }
        public delegate void TextAction();
        public delegate void ImageAction();

        private List<TextActionParmes> textListaction = new List<TextActionParmes>();
        private List<ImageActionParmes> imageListaction = new List<ImageActionParmes>();

        public void AddListener(Text target, TextAction textAction)
        {
            TextActionParmes findparmes = textListaction.Find((TextActionParmes parms) => { return parms.text == target; });
            if (findparmes.text != null)
                textListaction.Remove(findparmes);
            findparmes.text = target;
            findparmes.action = textAction;
            textListaction.Add(new TextActionParmes() { action = textAction, text = target });
        }

        public void AddListener(Image target, ImageAction imageAction)
        {
            ImageActionParmes findparmes = imageListaction.Find((ImageActionParmes parms) => { return parms.image == target; });
            if (findparmes.image != null)
                imageListaction.Remove(findparmes);
            findparmes.image = target;
            findparmes.action = imageAction;
            imageListaction.Add(new ImageActionParmes() { action = imageAction, image = target });
        }

        public void RemoveAllListener(TextAction textAction)
        {
            textListaction.Clear();
            imageListaction.Clear();
        }

        private static ColoringManager mInstance;
        public static ColoringManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject game = new GameObject("[ColoringManager]");
                    mInstance = game.AddComponent<ColoringManager>();
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
            initColoring(new XMLParser());

            OnColoringChanged.AddListener(() =>
            {
                for (int i = 0; i < textListaction.Count; i++)
                {
                    var item = textListaction[i];
                    if (item.text == null)
                        textListaction.Remove(item);
                    else
                    {
                        item.action.Invoke();
                    }
                }

                for (int i = 0; i < imageListaction.Count; i++)
                {
                    var item = imageListaction[i];
                    if (item.image == null)
                        imageListaction.Remove(item);
                    else
                    {
                        item.action.Invoke();
                    }
                }
            });

            m_previousColorTheme = CurrentColorTheme;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.UpArrow))
                CurrentColorTheme = ColorTheme.Green;
            if (Input.GetKeyDown(KeyCode.DownArrow))
                CurrentColorTheme = ColorTheme.Blue;
#endif
            ColorThemeEvent();
        }

        private void ColorThemeEvent()
        {
            if (CurrentColorTheme != m_previousColorTheme)
            {
                setColoring(mXMLParaser/*, CurrentColorTheme*/);
                if (OnColoringChanged != null) OnColoringChanged.Invoke();
                m_previousColorTheme = CurrentColorTheme;
            }
        }

        public void initColoring(ColoringPack coloringPack)
        {
            //ColorTheme color = CurrentColorTheme;
            setColoring(coloringPack/*, color*/);
        }

        private void setColoring(ColoringPack coloringPack, ColorTheme colorTheme)
        {
            CurrentColorTheme = colorTheme;
            setColoring(coloringPack/*, color*/);
        }

        private void setColoring(ColoringPack coloringPack)
        {
            ColorTheme colorTheme = CurrentColorTheme;
            Debug.Log("setColoring:" + colorTheme);
            TextAsset xml;
            switch (colorTheme)
            {
                case ColorTheme.Red:
                case ColorTheme.Blue:
                    xml = Resources.Load<TextAsset>("Theme/Blue");
                    Coloring.ConfigAndSetColor(coloringPack, xml.text, (int)ColorTheme.Blue);
                    break;
                default:
                    xml = Resources.Load<TextAsset>("Theme/Green");
                    Coloring.ConfigAndSetColor(coloringPack, xml.text, (int)ColorTheme.Green);
                    break;
            }
        }
    }
}
