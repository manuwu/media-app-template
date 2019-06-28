/*
 * Author:李传礼
 * DateTime:2018.01.15
 * Description:设备状态面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class DeviceStatusPanel : MonoBehaviour
{
    [SerializeField]
    private Sprite m_HandleConnected;
    [SerializeField]
    private Sprite m_HandleDisConnected;
    [SerializeField]
    private Image m_HandleButton;
    public Text SystemTime;
    public BatteryPowerIcon BPIcon;
    public WifiStrengthIcon WSIcon;
    public Text BatteryText;

    DateTimeUtils DateTimeUtils;

    public static Action OnHandleClick;

    private void Awake()
    {

        GvrControllerInput.OnConterollerChanged += GvrControllerInput_OnStateChanged;

    }

    private void GvrControllerInput_OnStateChanged(SvrControllerState state, SvrControllerState oldState)
    {
#if !SVR_USE_GAZE
        if (state == SvrControllerState.GvrController)
        {
            m_HandleButton.sprite = m_HandleConnected;
            m_HandleButton.SetNativeSize();
        }
        else
        {
            m_HandleButton.sprite = m_HandleDisConnected;
            m_HandleButton.SetNativeSize();
        }
#endif
    }

    void Start()
    {
        DateTimeUtils = SvrNativeInterface.GetInstance().DateTimeUtils;

        WifiManage.Instance.WifiSwitchStatusChangeCallback += WifiSwitchStatusChange;
        WifiManage.Instance.WifiConnectStatusChangeCallback += WifiConnectStatusChange;
        WifiManage.Instance.WifiStrengthChangeCallback += WifiStrengthChange;

        DeviceStatus.Instance.BatteryStatusChangeCallback += BatteryStatusChange;
        DeviceStatus.Instance.BatteryValueChangeCallback += BatteryValueChange;

        //if (!Launcher.Instant.IsResume)
        //{
        //    WifiManage.Instance.WifiUtils.Resume();
        //    Launcher.Instant.IsResume = true;
        //}

        InvokeRepeating("UpdateSystemTime", 0, 1);

        SetBatteryStatus(DeviceStatus.Instance.GetBatteryStatus());
        float batteryvalue = (float)DeviceStatus.Instance.GetCurrentBatteryValue() / DeviceStatus.Instance.GetMaxBatteryValue();
        SetBattery((int)(batteryvalue * 100));
        SetBatteryPower((int)(batteryvalue * 100));

        //WifiStrength wifiStrength = Launcher.Instant.WifiManage.GetWifiStrength();
        //SetWifiStrength(wifiStrength);
    }
    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            WifiManage.Instance.WifiUtils.Resume();
        }
    }
    private void OnEnable()
    {
        CheckWifiBtnStatus();
        GvrControllerInput_OnStateChanged(GvrControllerInput.SvrState, GvrControllerInput.SvrState);

        BatteryValueChange(DeviceStatus.Instance.GetCurrentBatteryValue(), DeviceStatus.Instance.GetBatteryPower(DeviceStatus.Instance.GetCurrentBatteryValue()));
    }

    void CheckWifiBtnStatus()
    {
        bool isWifiOpen = WifiManage.Instance.WifiUtils.GetWifiSwitchStatus();
        Debug.Log("DeviceStatusPanel:isWifiOpen:" + isWifiOpen);
        if (isWifiOpen)
        {
            int curWifiId = WifiManage.Instance.WifiUtils.GetCurrentNetworkId();
            Debug.Log("DeviceStatusPanel:curWifiId:" + curWifiId);
            if (curWifiId < 0)
            {
                WSIcon.Init(isWifiOpen, false);
                SetWifiConnectedStatus(false);
            }
            else
            {
                WSIcon.Init(isWifiOpen, true);
                SetWifiConnectedStatus(true);
            }
        }
        else
        {
            WSIcon.Init(isWifiOpen, false);
        }
    }

    private void BatteryValueChange(int percentNum, BatteryPower batteryPower)
    {
        SetBattery(percentNum);
        SetBatteryPower(percentNum);
    }

    private void BatteryStatusChange(BatteryStatus batteryStatus)
    {
        Svr.SvrLog.Log("BatteryStatusChange");
        SetBatteryStatus(batteryStatus);
    }

    private void WifiStrengthChange(WifiStrength wifiStrength)
    {
        Svr.SvrLog.Log("DeviceStatusPanel,WifiStrengthChange,"+ wifiStrength);
        WSIcon.SetWifiStrength(wifiStrength);
    }

    private void WifiConnectStatusChange(WifiConnectStatus wifiConnectStatus, string wifiName)
    {
        bool isConnected = false;
        bool isWifiOpen = WifiManage.Instance.WifiUtils.GetWifiSwitchStatus();
        if (isWifiOpen)
        {
            int curWifiId = WifiManage.Instance.WifiUtils.GetCurrentNetworkId();
            if (curWifiId < 0)
                isConnected = false;
            else
                isConnected = true;
        }
        else
            isConnected = false;

        SetWifiConnectedStatus(isConnected);
    }

    private void WifiSwitchStatusChange(bool obj)
    {
        SetWifiSwitchStatus(obj);
    }

    private void OnDestroy()
    {
        WifiManage.Instance.WifiSwitchStatusChangeCallback -= WifiSwitchStatusChange;
        WifiManage.Instance.WifiConnectStatusChangeCallback -= WifiConnectStatusChange;
        WifiManage.Instance.WifiStrengthChangeCallback -= WifiStrengthChange;

        DeviceStatus.Instance.BatteryStatusChangeCallback -= BatteryStatusChange;
        DeviceStatus.Instance.BatteryValueChangeCallback -= BatteryValueChange;

        GvrControllerInput.OnConterollerChanged -= GvrControllerInput_OnStateChanged;

        m_HandleConnected = null;
        m_HandleDisConnected = null;
        m_HandleButton = null;
        SystemTime = null;
        BPIcon = null;
        WSIcon = null;
        BatteryText = null;
        DateTimeUtils = null;
    }

    void UpdateSystemTime()
    {
        //SystemTime.text = DateTime.Now.ToString("yyyy.MM.dd  HH:mm");
        DateTime now = DateTimeUtils.GetCurrentDateTime();
        SystemTime.text = now.ToString("HH:mm");
    }

    public void SetBatteryStatus(BatteryStatus batteryStatus)
    {
        BPIcon.SetBatteryStatus(batteryStatus);
    }

    public void SetBatteryPower(int percentNum)
    {
        BPIcon.SetBatteryPower(percentNum);
    }

    public void SetBattery(int percentNum)
    {
        if (BatteryText != null)
            BatteryText.text = percentNum + "%";
    }

    public void SetWifiSwitchStatus(bool isOpen)
    {
        if (isOpen)
        {
            int curWifiId = WifiManage.Instance.WifiUtils.GetCurrentNetworkId();
            if (curWifiId < 0)
                WSIcon.SetWifiSwitchStatus(isOpen, false);
            else
                WSIcon.SetWifiSwitchStatus(isOpen, true);
        }
        else
            WSIcon.SetWifiSwitchStatus(isOpen, false);
    }

    public void SetWifiConnectedStatus(bool isConnected)
    {
        WSIcon.SetWifiConnectedStatus(isConnected);
    }

    public void Event_OnHandleClick()
    {
        if (OnHandleClick != null) OnHandleClick.Invoke();
    }
}
