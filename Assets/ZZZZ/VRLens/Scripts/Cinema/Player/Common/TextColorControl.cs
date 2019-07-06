/*
 * 2018-8-20
 * 黄秋燕 Shemi
 * 文字颜色状态改变面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextColorControl : MonoBehaviour {

    public Color NorColor;
    public Color HoverColor;
    public Color SelectColor;

	public void HoverState()
    {
        this.gameObject.GetComponent<Text>().color = HoverColor;
    }

    public void SelectState()
    {
        this.gameObject.GetComponent<Text>().color = SelectColor;
    }

    public void NormalState()
    {
        this.gameObject.GetComponent<Text>().color = NorColor;
    }
}
