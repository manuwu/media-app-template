/*
 * Author:李传礼
 * DateTime:2018.4.23
 * Description:Wifi管理
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum WifiStatus { Enabled, Saved, Using, Connecting }
public enum WifiStrength { Null, Low, Middle, High, Full }
public enum WifiConnectStatus
{
    Idle, Disconnected, Connecting,
    Connected, PasswordError, NotSupport,
    Forget, StartConnect, ForgetFail, ConnectFail
}

[Serializable]
public class JWifiInfo
{
    public string SSID;//网络名
    public string BSSID;//mac地址
    public string capabilities;//加密信息
    public int level;//信号强度等级
    public int networkID;//已保存wifi的 网络ID
    public int wifiStatus;//wifi状态
    public bool needPassword;//是否需要密码连接
    public bool is5G; //是否是5G信号
}

public class WifiInfo
{
    public int NetworkId;
    public string Name;
    public string MacAddress;
    public string Capabilities;
    public WifiStrength WifiStrength;
    public WifiStatus WifiStatus;
    public bool NeedPassword;
    public bool Is5G; //是否是5G信号

    public WifiInfo(JWifiInfo jWifiInfo)
    {
        NetworkId = jWifiInfo.networkID;
        Name = jWifiInfo.SSID;
        MacAddress = jWifiInfo.BSSID;
        Capabilities = jWifiInfo.capabilities;
        WifiStrength = (WifiStrength)jWifiInfo.level;
        WifiStatus = (WifiStatus)jWifiInfo.wifiStatus;
        NeedPassword = jWifiInfo.needPassword;
        Is5G = jWifiInfo.is5G;
    }

    public WifiInfo(int networkId, string name, string macAddress, string capabilities, WifiStrength wifiStrength, WifiStatus wifiStatus, bool needPassword, bool is5G)
    {
        NetworkId = networkId;
        Name = name;
        MacAddress = macAddress;
        Capabilities = capabilities;
        WifiStrength = wifiStrength;
        WifiStatus = wifiStatus;
        NeedPassword = needPassword;
        Is5G = is5G;
    }
}

public class WifiManage : MonoBehaviour
{
    public WifiUtils WifiUtils;

    public Action<bool> WifiSwitchStatusChangeCallback;
    public Action<List<WifiInfo>> ScanWifiInfoListCallback;
    public Action<WifiConnectStatus, string> WifiConnectStatusChangeCallback;
    public Action<WifiStrength> WifiStrengthChangeCallback;

    private static WifiManage mwifiManager;
    public static WifiManage Instance
    {
        get
        {
            if (mwifiManager == null)
            {
                GameObject wifogo = new GameObject("WifiManage");
                mwifiManager = wifogo.AddComponent<WifiManage>();
            }
            return mwifiManager;
        }
    }

    public bool IsChangeStrength { get; internal set; }
    public WifiStrength ConnectedWifiStrength { get; internal set; }

    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        WifiUtils = SvrNativeInterface.GetInstance().WifiUtils;

        WifiUtils.SetOnWifiSwitchStatusChangeEvent(this.name, "OnWifiSwitchStatusChangeEvent");
        WifiUtils.SetOnScanWifiListEvent(this.name, "OnScanWifiListEvent");
        WifiUtils.SetOnWifiConnectStatusChangeEvent(this.name, "OnWifiConnectStatusChangeEvent");
        WifiUtils.SetOnLevelChangeEvent(this.name, "OnLevelChangeEvent");
    }
    private void OnDestroy()
    {
        mwifiManager = null;
    }
    /// <summary>
    /// OpenWifi成功后才回调
    /// </summary>
    /// <param name="isOpenStr">open status</param>
    void OnWifiSwitchStatusChangeEvent(string isOpenStr)
    {
        MainThreadQueue.ExecuteQueue.Enqueue(()=>
        {
            Debug.Log("wifi:: OnWifiSwitchStatusChangeEvent:" + isOpenStr);
            if (isOpenStr == null || isOpenStr == string.Empty) return;
            bool isOpen = bool.Parse(isOpenStr);

            if (WifiSwitchStatusChangeCallback != null)
                WifiSwitchStatusChangeCallback(isOpen);
        });

    }

    void OnScanWifiListEvent(string jWifiInfoStrs)
    {
        MainThreadQueue.ExecuteQueue.Enqueue(() =>
        {
            if (jWifiInfoStrs == null || jWifiInfoStrs == string.Empty) return;
            JWifiInfo[] jWifiInfos = JsonArray<JWifiInfo>.GetJsonArray(jWifiInfoStrs);

            List<WifiInfo> wifiInfoList = new List<WifiInfo>();
            if (jWifiInfos != null)
            {
                foreach (JWifiInfo jWifiInfo in jWifiInfos)
                {
                    WifiInfo wifiInfo = new WifiInfo(jWifiInfo);
                    wifiInfo.WifiStrength = wifiInfo.WifiStrength + 1;
                    wifiInfoList.Add(wifiInfo);
                }
            }

            //for (int i = 0; i < wifiInfoList.Count; i++)
            //{
            //    Debug.Log("wifi:: wifiInfoList[" + i + "].name = " + wifiInfoList[i].Name +
            //        ", id = " + wifiInfoList[i].NetworkId + ", status = " + wifiInfoList[i].WifiStatus
            //        + ", level = " + (int)wifiInfoList[i].WifiStrength
            //        + ", strength = " + wifiInfoList[i].WifiStrength
            //        + ", is5G = " + wifiInfoList[i].Is5G);
            //}

            if (ScanWifiInfoListCallback != null)
                ScanWifiInfoListCallback(wifiInfoList);
        });
    }

    void OnWifiConnectStatusChangeEvent(string wifiConnectStatusStr)
    {
        MainThreadQueue.ExecuteQueue.Enqueue(() => 
        {
            if (wifiConnectStatusStr == null || wifiConnectStatusStr == string.Empty) return;
            Debug.Log("wifi:: OnWifiConnectStatusChangeEvent:" + wifiConnectStatusStr);
            string[] str = wifiConnectStatusStr.Split(';');

            int wifiConnectStatus = int.Parse(str[0]);

            if (str[1].Contains("\""))
                str[1] = str[1].Replace("\"", "");

            if (WifiConnectStatusChangeCallback != null)
                WifiConnectStatusChangeCallback((WifiConnectStatus)wifiConnectStatus, str[1]);
            if ((WifiConnectStatus)wifiConnectStatus == WifiConnectStatus.Connected)
            {
                if (IsInvoking("ChangeNetState"))
                    CancelInvoke("ChangeNetState");
                InvokeRepeating("ChangeNetState", 0.5f, 0.02f);
            }
            else
            {
                //  弹出断网提示信息；
            }
        });

    }

    void ChangeNetState()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            CancelInvoke("ChangeNetState");
            EventCenter.Broadcast(EGameEvent.eGameEvent_ReConnectSuccess);
        }
    }

    void OnLevelChangeEvent(string levelStr)
    {
        MainThreadQueue.ExecuteQueue.Enqueue(() =>
        {
            if (levelStr == null || levelStr == string.Empty) return;

            int level = int.Parse(levelStr);

            if (WifiStrengthChangeCallback != null)
                WifiStrengthChangeCallback((WifiStrength)level+1);
        });
    }

    public WifiStrength GetWifiStrength()
    {
        if (WifiUtils != null)
        {
            int level = WifiUtils.GetWifiLevel();
            return (WifiStrength)level+1;
        }
        else
            return WifiStrength.Null;
    }
}
