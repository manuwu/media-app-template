using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVR.Coloring
{
    public static class Coloring
    {
        private static ColoringPack color_pack;
        private static Dictionary<int, ColoringLocalization> dict_localizations;
        private static int color;

        public delegate void OnColoringChanged();
        public static event OnColoringChanged onColoringChanged;

        static Coloring()
        {
            bool isinited = ColoringManager.Instance.hadInited;
            dict_localizations = new Dictionary<int, ColoringLocalization>();
        }

        /// <summary>
        /// 判断是否准备好了
        /// </summary>
        /// <returns></returns>
        public static bool CheckOk()
        {
            return color_pack != null;
        }

        /// <summary>
        /// 配置主题颜色包
        /// </summary>
        /// <param name="_get"></param>
        public static void Config(ColoringPack _get)
        {
            color_pack = _get;
        }

        public static void ConfigAndSetColor(ColoringPack _get, string text, int _theme)
        {
            _get.InitXmlFromString(text);
            Coloring.Config(_get);
            Coloring.Switch(_theme);
        }

        /// <summary>
        /// 返回指定key的语言
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        public static Color Get(string _key)
        {
            return color_pack.Get(_key);
        }

        /// <summary>
        /// 根据提供的Key设置对应文字的颜色
        /// </summary>
        /// <param name="text"></param>
        /// <param name="_key"></param>
        public static void SetTextColorByKey(this UnityEngine.UI.Text text, string _key)
        {
            if (_key == null || _key == "") return;

            text.color = Get(_key);

            ColoringManager.Instance.AddListener(text, () => {
                text.color = Get(_key);
            });
        }

        /// <summary>
        /// 根据提供的Key设置对应图片的颜色
        /// </summary>
        /// <param name="image"></param>
        /// <param name="_key"></param>
        public static void SetImageColorByKey(this UnityEngine.UI.Image image, string _key)
        {
            if (_key == null || _key == "") return;

            image.color = Get(_key);

            ColoringManager.Instance.AddListener(image, () => {
                image.color = Get(_key);
            });
        }

        public static void SetTextColor(this UnityEngine.UI.Text text, Color color)
        {
            text.color = color;
        }

        public static void SetImageColor(this UnityEngine.UI.Image image, Color color)
        {
            image.color = color;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="_ll"></param>
        public static void Register(ColoringLocalization _ll)
        {
            int key = _ll.GetInstanceID();
            if (dict_localizations.ContainsKey(key))
            {
                return;
            }
            dict_localizations.Add(key, _ll);
            //添加过后需要刷新
            //Fresh (_ll);
        }

        /// <summary>
        /// 解除注册
        /// </summary>
        /// <param name="_ll"></param>
        public static void UnRegister(ColoringLocalization _ll)
        {
            int key = _ll.GetInstanceID();
            dict_localizations.Remove(key);
        }

        /// <summary>
        /// 切换主题
        /// </summary>
        /// <param name="_theme"></param>
        public static void Switch(int _theme)
        {
            color = _theme;
            //设置主题颜色包中的当前主题环境
            color_pack.SetColorTheme(_theme);
        }

        /// <summary>
        /// 返回当前的主题
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentColorTheme()
        {
            return color;
        }
    }
}
