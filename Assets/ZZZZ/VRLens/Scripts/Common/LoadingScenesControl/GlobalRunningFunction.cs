using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class GlobalRunningFunction : MonoBehaviour
{
    public const string dllName = "svr_plugin_media_scan";
    [DllImport(dllName)]
    public static extern void InitEnvironment();
    [DllImport(dllName)]
    public static extern void ReleaseEnvironment();

    public GameObject NoloLeftController;
    public GameObject NoloRightController;
    public GameObject GvrControllerPointer;
    public MeshRenderer SvrReticleRayLine;

    public LoadImageAndAnalyze LoadImageAndAnalyze;
    public InputOperate InputOperate;

    public SubtitleCanvasControl Subtitle;
   
    public static GlobalRunningFunction Instance { get; private set; }

    private void Awake()
    {
        // 确保只会创建一次该对象
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != Instance)
        {
            Destroy(gameObject);
        }

        InitEnvironment();
    }

    public void ShowControllerRayLine()
    {
        //if ((GvrControllerInput.SvrState & (SvrControllerState.NoloLeftContoller | SvrControllerState.NoloRightContoller)) != 0)
        //{
        //    if ((GvrControllerInput.SvrState & SvrControllerState.NoloLeftContoller) != 0 && NoloLeftController != null)
        //        NoloLeftController.transform.localScale = Vector3.one;

        //    if ((GvrControllerInput.SvrState & SvrControllerState.NoloRightContoller) != 0 && NoloRightController != null)
        //        NoloRightController.transform.localScale = Vector3.one;
        //}
        //else if (GvrControllerInput.SvrState == SvrControllerState.None && SvrReticleRayLine != null)
        //{
        //    if (Svr.SvrSetting.IsVR9Device)
        //        SVR.AtwAPI.ShowDualSurface(true);
        //    else
        //        SvrReticleRayLine.enabled = true;
        //}
        //else if (GvrControllerInput.SvrState == SvrControllerState.GvrController && GvrControllerPointer != null)
        //    GvrControllerPointer.transform.localScale = Vector3.one;
        GvrControllerInput.GvrPointerEnable = true;
    }

    public void HideControllerRayLine()
    {
        GvrControllerInput.GvrPointerEnable = false;
        //if ((GvrControllerInput.SvrState & (SvrControllerState.NoloLeftContoller | SvrControllerState.NoloRightContoller)) != 0)
        //{
        //    if ((GvrControllerInput.SvrState & SvrControllerState.NoloLeftContoller) != 0 && NoloLeftController != null)
        //        NoloLeftController.transform.localScale = Vector3.zero;

        //    if ((GvrControllerInput.SvrState & SvrControllerState.NoloRightContoller) != 0 && NoloRightController != null)
        //        NoloRightController.transform.localScale = Vector3.zero;
        //}
        //else if (GvrControllerInput.SvrState == SvrControllerState.None && SvrReticleRayLine != null)
        //{
        //    if (Svr.SvrSetting.IsVR9Device)
        //        SVR.AtwAPI.ShowDualSurface(false);
        //    else
        //        SvrReticleRayLine.enabled = false;
        //}
        //else if (GvrControllerInput.SvrState == SvrControllerState.GvrController && GvrControllerPointer != null)
        //    GvrControllerPointer.transform.localScale = Vector3.zero;
    }

    private void OnApplicationQuit()
    {
        ReleaseEnvironment();
    }
}
