using System;

namespace IVR.Language
{
    /// <summary>
    /// 语言包接口 主要提供语言文本
    /// </summary>
    public interface ILanguagePack
    {
        /// <summary>
        /// 根据key返回当前语言环境下的语言
        /// </summary>
        /// <param name="_key"></param>
        /// <returns></returns>
        string Get(string _key);

        /// <summary>
        /// 设置语言环境
        /// </summary>
        /// <param name="_language"></param>
        void SetLanguage(int _language);

        void InitXmlFromResourcesPath(string _fileName);

        void InitXmlFromString(string _xml);
    }
}