using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace SVR.Coloring
{
    public class ColoringLocalization : MonoBehaviour
    {
        [SerializeField]
        private string text = string.Empty;
        private Color finalcolor = Color.white;

        public Color FinalColor
        {
            get
            {
                return finalcolor;
            }
            set
            {
                finalcolor = value;

                //2DUI文本
                Text text = GetComponent<Text>();
                if (text != null)
                    text.color = finalcolor;

                Image image = GetComponent<Image>();
                if (image != null)
                    image.color = finalcolor;
            }
        }

        private void OnEnable()
        {
            FinalColor = Coloring.Get(text);
            ColoringManager.Instance.OnColoringChanged.AddListener(ColoringChanged);
        }

        private void ColoringChanged()
        {
            FinalColor = Coloring.Get(text);
        }

        private void OnDisable()
        {
            ColoringManager.Instance.OnColoringChanged.RemoveListener(ColoringChanged);
        }
    }
}
