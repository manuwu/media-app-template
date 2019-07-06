using UnityEngine;

namespace SVR.Coloring
{
    /// <summary>
    /// 主题颜色包接口
    /// </summary>
    public interface ColoringPack
    {
        /// <summary>
        /// 根据key返回当前主题的颜色
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        Color Get(string _key);

        /// <summary>
        /// 设置主题类型
        /// </summary>
        /// <param name="_theme"></param>
        void SetColorTheme(int _theme);

        void InitXmlFromResourcesPath(string _fileName);

        void InitXmlFromString(string _xml);
    }
}
