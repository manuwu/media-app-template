/*
 * 2018-8-9
 * 黄秋燕 Shemi
 * 返回按钮的统一控制，需要在退出时点击此按钮，并且退出前重新设置BackBtn的状态
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour {
    public Image BackBtnChildImage;
    public Text BackBtnChildText;
    public Sprite BackBtnChildNormalSprite;

    public void ResetBackBtnStatus()
    {
        BackBtnChildImage.sprite = BackBtnChildNormalSprite;
        if (BackBtnChildText != null)
            BackBtnChildText.gameObject.SetActive(false);
    }
}
