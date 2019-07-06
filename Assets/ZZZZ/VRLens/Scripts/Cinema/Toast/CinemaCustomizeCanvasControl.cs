using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 自定义浮窗（事件消失）
/// </summary>
public class CinemaCustomizeCanvasControl : SingletonMB<CinemaCustomizeCanvasControl>
{
    public GameObject LoadingPanel;
    public DownloadSpeedPanel DownloadSpeedPanel;

    public void SetDownloadSpeed(string str)
    {
        //DownloadSpeedPanel.DownloadSpeedText.text = str;
    }
}
