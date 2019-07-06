/*
 * 2018-8-2
 * 黄秋燕 Shemi
 * video名字截取，多余字符使用省略号替换
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StripIconNameLengthWithSuffix : MonoBehaviour
{
    Text VideNameText;
    string NameString;
    int SuffixWidth;

    const string Suffix = "..";
    public int MaxNameLenth;

    public void Init(string name)
    {
        VideNameText = this.gameObject.GetComponent<Text>();
        if (VideNameText == null) return;

        if (name == null || name == string.Empty)
        {
            VideNameText.text = string.Empty;
            return;
        }

        NameString = name;
        //计算后缀的长度
        SuffixWidth = CalculateLengthOfText(Suffix);

        VideNameText.text = StripLengthWithSuffix(name, MaxNameLenth);
    }

    string StripLengthWithSuffix(string input, int maxWidth)
    {
        int len = CalculateLengthOfText(input);
        //截断text的长度，如果总长度大于限制的最大长度，
        //那么先根据最大长度减去后缀长度的值拿到字符串，在拼接上后缀
        if (len > maxWidth)
        {
            string stringTemp = StripLength(input, maxWidth - SuffixWidth);
            string s = stringTemp.Substring(stringTemp.Length - 1, 1);
            if (s.Equals("."))
                stringTemp = stringTemp.Substring(0, stringTemp.Length - 1);

            return stringTemp + Suffix;
        }
        else
            return input;
    }

    /// <summary>
    /// 根据maxWidth来截断input拿到子字符串
    /// </summary>
    /// <param name="input"></param>
    /// <param name="maxWidth"></param>
    /// <returns></returns>
    string StripLength(string input, int maxWidth)
    {
        int totalLength = 0;
        Font myFont = VideNameText.font;  //chatText is my Text component
        myFont.RequestCharactersInTexture(input, VideNameText.fontSize, VideNameText.fontStyle);
        CharacterInfo characterInfo = new CharacterInfo();
        char[] arr = input.ToCharArray();
        int i = 0;
        foreach (char c in arr)
        {
            myFont.GetCharacterInfo(c, out characterInfo, VideNameText.fontSize);
            int newLength = totalLength + characterInfo.advance;
            if (newLength > maxWidth)
            {
                if (Mathf.Abs(newLength - maxWidth) > Mathf.Abs(maxWidth - totalLength))
                    break;
                else
                {
                    totalLength = newLength;
                    i++;
                    break;
                }
            }

            totalLength += characterInfo.advance;
            i++;
        }

        return input.Substring(0, i);
    }

    /// <summary>
    /// 计算字符串在指定text控件中的长度
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    int CalculateLengthOfText(string name)
    {
        int totalLength = 0;
        Font myFont = VideNameText.font;  //chatText is my Text component
        myFont.RequestCharactersInTexture(name, VideNameText.fontSize, VideNameText.fontStyle);
        CharacterInfo characterInfo = new CharacterInfo();
        char[] arr = name.ToCharArray();

        foreach (char c in arr)
        {
            myFont.GetCharacterInfo(c, out characterInfo, VideNameText.fontSize);
            totalLength += characterInfo.advance;
        }

        return totalLength;
    }
}
