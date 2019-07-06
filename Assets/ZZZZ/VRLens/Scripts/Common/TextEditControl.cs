/*
 * 2018-8-20
 * 黄秋燕 Shemi
 * 文字颜色状态改变面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEditControl : MonoBehaviour
{
    public Color NorColor;
    public Color HoverColor;
    public Color SelectColor;

	public void HoverState()
    {
        if(this.gameObject.GetComponent<Text>() != null)
            this.gameObject.GetComponent<Text>().color = HoverColor;
    }

    public void SelectState()
    {
        if(this.gameObject.GetComponent<Text>() != null)
            this.gameObject.GetComponent<Text>().color = SelectColor;
    }

    public void NormalState()
    {
        if(this.gameObject.GetComponent<Text>() != null)
            this.gameObject.GetComponent<Text>().color = NorColor;
    }

    public void FontStyleNormal()
    {
        if(this.gameObject.GetComponent<Text>() != null)
            this.gameObject.GetComponent<Text>().fontStyle = FontStyle.Normal;
    }

    public void FontStyleBold()
    {
        if(this.gameObject.GetComponent<Text>() != null)
            this.gameObject.GetComponent<Text>().fontStyle = FontStyle.Bold;
    }
}
