/*
 * Author:李传礼
 * DateTime:2018.05.02
 * Description:Wifi强度图标
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WifiStrengthIcon : MonoBehaviour
{
    public Sprite WifiOffTexture;//wifi关闭
    public Sprite WifiUnconnectedTexture;//wifi未连接
    public Sprite[] WifiStrengths = new Sprite[5];//wifi强度
    Image Image;
    bool IsWifiOpen;
    bool IsWifiConnected;
    WifiStrength CurWifiStrength;

    public void Init(bool isOpen, bool isConnected)
    {
        if (Image == null) Image = this.GetComponent<Image>();
        IsWifiOpen = isOpen;
        IsWifiConnected = isConnected;

        SetWifiSwitchStatus(IsWifiOpen, IsWifiConnected);

        if (IsWifiConnected && !WifiManage.Instance.IsChangeStrength)
            CurWifiStrength = WifiManage.Instance.GetWifiStrength();
        else if (IsWifiConnected && WifiManage.Instance.IsChangeStrength)
            CurWifiStrength = WifiManage.Instance.ConnectedWifiStrength;
        else
            CurWifiStrength = WifiStrength.Null;

        SetWifiConnectedStatus(isConnected);
    }

    public void SetWifiSwitchStatus(bool isOpen, bool isConnect)
    {
        if (Image == null) Image = this.GetComponent<Image>();
        IsWifiOpen = isOpen;
        IsWifiConnected = isConnect;

        if (!IsWifiOpen)
        {
            Image.sprite = WifiOffTexture;
            Debug.Log("WifiStrengthIcon:WifiOffTexture");
        }
        else if (IsWifiOpen && !isConnect)
        {
            Image.sprite = WifiUnconnectedTexture;
            Debug.Log("WifiStrengthIcon:WifiUnconnectedTexture");
        }
    }

    public void SetWifiConnectedStatus(bool isConnected)
    {
        if (!IsWifiOpen)
            return;

        IsWifiConnected = isConnected;
        if (!IsWifiConnected)
        {
            Image.sprite = WifiUnconnectedTexture;
            Debug.Log("WifiStrengthIcon:WifiUnconnectedTexture");
        }
        else
        {
            if (IsWifiConnected && !WifiManage.Instance.IsChangeStrength)
                CurWifiStrength = WifiManage.Instance.GetWifiStrength();
            else if (IsWifiConnected && WifiManage.Instance.IsChangeStrength)
                CurWifiStrength = WifiManage.Instance.ConnectedWifiStrength;
            else
                CurWifiStrength = WifiStrength.Null;
            SetWifiStrength(CurWifiStrength);
            Debug.Log("WifiStrengthIcon:"+ CurWifiStrength);
        }
    }

    public void SetWifiStrength(WifiStrength wifiStrength)
    {
        if (!IsWifiOpen || !IsWifiConnected)
            return;

        if (Image == null || WifiStrengths == null)
            return;

        CurWifiStrength = wifiStrength;
        WifiManage.Instance.IsChangeStrength = true;
        WifiManage.Instance.ConnectedWifiStrength = wifiStrength;
        int index = (int)CurWifiStrength;
        if (index <= WifiStrengths.Length - 1)
            Image.sprite = WifiStrengths[index];

        Debug.Log("set WifiStrength:" + CurWifiStrength);
    }
}
