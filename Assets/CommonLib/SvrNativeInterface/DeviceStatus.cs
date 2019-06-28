/*
 * Author:李传礼
 * DateTime:2018.03.13
 * Description:设备状态
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BatteryStatus { NoCharging, ChargerCharging, UsbCharging }
public enum BatteryPower { One = 1, Two, Three, Four, Five }//Five表示电量满格

public class DeviceStatus : MonoBehaviour
{
    BatteryUtils BatteryUtils;
    int MaxBatteryValue;
    int BatteryPowerLevel;
    int BPLevelUnit;//电量等级单位
    int OldBatteryValue;
    BatteryStatus OldBatterySts;

    public Action<BatteryStatus> BatteryStatusChangeCallback;
    public event Action<int, BatteryPower> BatteryValueChangeCallback;

    private static DeviceStatus mDeviceStatus;
    public static DeviceStatus Instance
    {
        get
        {
            if (mDeviceStatus == null)
            {
                GameObject go = new GameObject("DeviceStatus");
                mDeviceStatus = go.AddComponent<DeviceStatus>();
            }
            return mDeviceStatus;
        }
    }
    private void Awake()
    {
        Init();
    }
    void Start ()
    {
		
	}
	
	void Update ()
    {
        CheckDeviceStatus();
    }

    private void Init()
    {
        BatteryUtils = SvrNativeInterface.GetInstance().BatteryUtils;
        MaxBatteryValue = GetMaxBatteryValue();
        BatteryPowerLevel = 5;
        BPLevelUnit = 10 / BatteryPowerLevel;
        OldBatteryValue = 0;
        OldBatterySts = BatteryStatus.NoCharging;

        //InvokeRepeating("CheckDeviceStatus", 0, 1);
    }

    public int GetMaxBatteryValue()
    {
#if UNITY_EDITOR
        return 100;
#else
        if (BatteryUtils != null)
            return BatteryUtils.GetMaxBattery();
        else
            return 100;
#endif
    }

    public int GetCurrentBatteryValue()
    {
#if UNITY_EDITOR
        return 10;
#else
        if (BatteryUtils != null)
            return BatteryUtils.GetCurrentBattery();
        else
            return 0;
#endif
    }

    public BatteryStatus GetBatteryStatus()
    {
        if (BatteryUtils != null)
        {
            int batteryStatus = BatteryUtils.GetBatteryStatus();
            return (BatteryStatus)batteryStatus;
        }
        else
            return BatteryStatus.NoCharging;
    }

    void CheckDeviceStatus()
    {
        CheckBatteryPower();
        CheckBatteryStatus();
    }

    public BatteryPower GetBatteryPower(int batteryValue)
    {
        float percent = (float)batteryValue / MaxBatteryValue;
        int level = Mathf.CeilToInt(percent * 10 / BPLevelUnit);
        if (level == 0)
            level = 1;

        return (BatteryPower)level;
    }

    void CheckBatteryPower()
    {
        int curBatteryValue = GetCurrentBatteryValue();

        if(curBatteryValue != OldBatteryValue)
        {
            int percentNum = Mathf.CeilToInt(((float)curBatteryValue / MaxBatteryValue) * 100);
            BatteryPower curBatteryPower = GetBatteryPower(curBatteryValue);

            if (BatteryValueChangeCallback != null)
                BatteryValueChangeCallback(percentNum, curBatteryPower);

            OldBatteryValue = curBatteryValue;
        }
    }

    void CheckBatteryStatus()
    {
        BatteryStatus curBatterySts = GetBatteryStatus();
        if (curBatterySts != OldBatterySts)
        {
            if (BatteryStatusChangeCallback != null)
                BatteryStatusChangeCallback(curBatterySts);

            OldBatterySts = curBatterySts;
        }
    }
}
