using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SVR;
using UnityEngine.Events;
using Svr;

public class BluetoothUtils
{
    AndroidJavaObject InterfaceObj;

    public BluetoothUtils(SvrNativeInterface svrNI)
    {
        if (svrNI != null)
        {
            InterfaceObj = svrNI.GetUtils(UtilsType.BluetoothUtils);
            SetOnBluetoothEvent();
            LogTool.Log("创建BluetoothUtils");
        }
    }

    public void SetOnBluetoothEvent()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        BluetoothListener listener = new BluetoothListener(this);
        if (InterfaceObj != null)
            InterfaceObj.Call("setListener", listener);
#endif
    }

    public bool IsOpenBluetooth()
    {
        bool isOpen = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            isOpen = InterfaceObj.Call<bool>("isOpen");
#endif
        return isOpen;
    }

    public bool OpenBluetooth()
    {
        bool isOpen = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            isOpen = InterfaceObj.Call<bool>("open");
#endif
        return isOpen;
    }

    public bool CloseBluetooth()
    {
        bool isClose = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            isClose = InterfaceObj.Call<bool>("close");
#endif
        return isClose;
    }

    public bool SearchBluetoothDevices()
    {
        bool isClose = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            isClose = InterfaceObj.Call<bool>("search");
#endif
        return isClose;
    }

    public bool CancelSearchBluetoothDevices()
    {
        bool isCancelSearch = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            isCancelSearch = InterfaceObj.Call<bool>("cancelSearch");
#endif
        return isCancelSearch;
    }

    public bool IsSearching()
    {
        bool isSearch = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            isSearch = InterfaceObj.Call<bool>("isSearching");
#endif
        return isSearch;
    }

    public List<BluetoothDeviceInfo> GetBondedBluetoothDevice()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject bluetoothInfoObj = InterfaceObj.Call<AndroidJavaObject>("getBondedDevices");
        int count = bluetoothInfoObj.Call<int>("size");
        List<BluetoothDeviceInfo> bluetoothInfoObjList = new List<BluetoothDeviceInfo>();
        for (int i = 0; i<count;i++)
        {
            BluetoothDeviceInfo bluetooth = new BluetoothDeviceInfo();
            if (bluetoothInfoObj != null)
            {
                bluetooth = getBluetoothDevice(bluetoothInfoObj.Call<AndroidJavaObject>("get", i));
                Debug.Log("GetBondedBluetoothDevice bluetooth = "+ bluetooth.name +", "+ bluetooth.address + ", "+ bluetooth.type + ", " + bluetooth.bondState);
                bluetoothInfoObjList.Add(bluetooth);
            }
        }
        
        return bluetoothInfoObjList;
#else
        List<BluetoothDeviceInfo> bluetoothInfoObjList = new List<BluetoothDeviceInfo>();
        for (int i = 0; i < 5; i++)
        {
            BluetoothDeviceInfo info = new BluetoothDeviceInfo("A" + i, "A1-A1-A1-A1-A1", 0, i % 3 +10);
                bluetoothInfoObjList.Add(info);
        }
        return bluetoothInfoObjList;
#endif
    }

    public void BondBluetoothDevice(BluetoothDeviceInfo bluetooth)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            InterfaceObj.Call("bond", bluetooth.obj);
#endif
    }

    public void UnBondBluetoothDevice(BluetoothDeviceInfo bluetooth)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            InterfaceObj.Call("unbond", bluetooth.obj);
#endif
    }

    public void Resume()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            InterfaceObj.Call("resume");
#endif
    }

    public void Pause()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            InterfaceObj.Call("pause");
#endif
    }

    public class BluetoothDeviceInfo
    {
        public string name;
        public string address;
        public int type;
        public int bondState;
        public AndroidJavaObject obj;

        public BluetoothDeviceInfo()
        {
            this.name = "";
            this.address = "";
            this.type = 0;
            this.bondState = 10;
            this.obj = null;
        }

        public BluetoothDeviceInfo(string name, string address, int type, int bondState)
        {
            this.name = name;
            this.address = address;
            this.type = type;
            this.bondState = bondState;
        }
    }

    private List<BluetoothDeviceInfo> _BluetoothInfo;
    public List<BluetoothDeviceInfo> BluetoothInfo
    {
        get
        {
            if (_BluetoothInfo == null)
            {
                _BluetoothInfo = GetBondedBluetoothDevice();
            }

            return _BluetoothInfo;
        }
    }

    public BluetoothDeviceInfo getBluetoothDevice(AndroidJavaObject bluetoothInfoObj)
    {
        BluetoothDeviceInfo bluetoothInfo = new BluetoothDeviceInfo()
        {
            name = bluetoothInfoObj.Call<string>("getName"),
            address = bluetoothInfoObj.Call<string>("getAddress"),
            type = bluetoothInfoObj.Call<int>("getType"),
            bondState = bluetoothInfoObj.Call<int>("getBondState"),
            obj = bluetoothInfoObj,
        };
        return bluetoothInfo;
    }

    public enum BondError
    {
        BOND_ERROR = 0,
        SYSTEM_PERMISSIONS_ERROR = 1
    }

    public enum MessageType
    {
        None,
        DeviceFound,
        BondChanged,
        ScanStart,
        ScanFinish,
        NoSupport,
        Error,
        isOpen,
        isConnect
    }

    public UnityAction<BluetoothDeviceInfo> OnDeviceFound;
    public UnityAction<BluetoothDeviceInfo> OnBondChanged;
    public UnityAction OnScanStart;
    public UnityAction OnScanFinish;
    public UnityAction OnNoSupport;
    public UnityAction<BondError> OnBondError;
    public UnityAction<bool> OnOpenedCallback;
    public UnityAction<bool> OnConnectedCallback;

    private Queue<Message> otgMessageQueue = new Queue<Message>();
    public void LoopMessage()
    {
//#if UNITY_ANDROID && !UNITY_EDITOR
//        if (otgMessageQueue == null)
//            otgMessageQueue = new Queue<Message>();
//        if (otgMessageQueue.Count == 0) return;
//        Message message = otgMessageQueue.Peek();
//        switch ((MessageType)message.messageType)
//        {
//            case MessageType.DeviceFound:
//                if (OnDeviceFound != null)
//                {
//                    AndroidJavaObject bluetoothInfoObj = (AndroidJavaObject)message.GetObject("BluetoothDevice");
//                    if (bluetoothInfoObj != null)
//                        OnDeviceFound(getBluetoothDevice(bluetoothInfoObj));
//                    otgMessageQueue.Dequeue();
//                }
//                break;
//            case MessageType.BondChanged:
//                if (OnBondChanged != null)
//                {
//                    AndroidJavaObject bluetoothInfoObj = (AndroidJavaObject)message.GetObject("BluetoothDevice");
//                    if (bluetoothInfoObj != null)
//                        OnBondChanged(getBluetoothDevice(bluetoothInfoObj));
//                    otgMessageQueue.Dequeue();
//                }

//                break;
//            case MessageType.ScanStart:
//                if (OnScanStart != null)
//                {
//                    OnScanStart();
//                    otgMessageQueue.Dequeue();
//                }
//                break;
//            case MessageType.ScanFinish:
//                if (OnScanFinish != null)
//                {
//                    OnScanFinish();
//                    otgMessageQueue.Dequeue();
//                }
//                break;
//            case MessageType.NoSupport:
//                if (OnNoSupport != null)
//                {
//                    OnNoSupport();
//                    otgMessageQueue.Dequeue();
//                }
//                break;
//            case MessageType.Error:
//                BondError bondError = (BondError)message.GetInt("BondError");
//                if (OnBondError != null)
//                {
//                    OnBondError(bondError);
//                    otgMessageQueue.Dequeue();
//                }
//                break;
//            case MessageType.isOpen:
//                bool isOpen = message.GetBool("isOpen");
//                if (OnOpenedCallback != null)
//                {
//                    OnOpenedCallback(isOpen);
//                    otgMessageQueue.Dequeue();
//                }
//                break;
//            case MessageType.isConnect:
//                bool isConnect = message.GetBool("isConnect");
//                if (OnConnectedCallback != null)
//                {
//                    OnConnectedCallback(isConnect);
//                    otgMessageQueue.Dequeue();
//                }
//                break;
//            default:
//                break;
//        }
//#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    public sealed class BluetoothListener : AndroidJavaProxy
    {
        private BluetoothUtils oTGContent;
        public BluetoothListener(BluetoothUtils oTG) : base("com.ssnwt.vr.androidmanager.bluetooth.BluetoothUtils$BluetoothListener")
        {
            oTGContent = oTG;
        }

        public void onDeviceFound(AndroidJavaObject bluetoothInfoObj)
        {
            Message message = new Message((int)MessageType.DeviceFound);
            message.SetObject("BluetoothDevice", bluetoothInfoObj);
            oTGContent.otgMessageQueue.Enqueue(message);
        }

        public void onBondChanged(AndroidJavaObject bluetoothInfoObj)
        {

            Message message = new Message((int)MessageType.BondChanged);
            message.SetObject("BluetoothDevice", bluetoothInfoObj);
            oTGContent.otgMessageQueue.Enqueue(message);
        }

        public void onScanStart()
        {
            Message message = new Message((int)MessageType.ScanStart);
            oTGContent.otgMessageQueue.Enqueue(message);
        }

        public void onScanFinish()
        {
            Message message = new Message((int)MessageType.ScanFinish);
            oTGContent.otgMessageQueue.Enqueue(message);
        }

        public void onNoSupportBluetooth()
        {
            Message message = new Message((int)MessageType.NoSupport);
            oTGContent.otgMessageQueue.Enqueue(message);
        }

        public void onBondError(int code)
        {
            Message message = new Message((int)MessageType.Error);
            message.SetInt("BondError", code);
            oTGContent.otgMessageQueue.Enqueue(message);
        }

        public void onOpened(bool isOpen)
        {
            Message message = new Message((int)MessageType.isOpen);
            message.SetBool("isOpen", isOpen);
            oTGContent.otgMessageQueue.Enqueue(message);
        }

        public void onConnected(bool isConnect)
        {
            Message message = new Message((int)MessageType.isConnect);
            message.SetBool("isConnect", isConnect);
            oTGContent.otgMessageQueue.Enqueue(message);
        }
    }
#endif
}
