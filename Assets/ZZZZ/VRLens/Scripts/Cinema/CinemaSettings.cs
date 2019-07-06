/*
 * 2018-11-14
 * 黄秋燕 Shemi
 * Cinema场景设置控制器（目前主要针对天空盒控制）
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaSettings : SingletonMB<CinemaSettings>
{
    [SerializeField]
    private CinemaPanel CinemaPanel;

    [HideInInspector]
    public Vector3 ImaxQuadScreenPosition = new Vector3(0, 1.5f, 25);
    [HideInInspector]
    public Vector3 ImaxQuadScreenScale = new Vector3(25.90708f, 14.58858f, 1);

    private Vector3 Drive_KartingQuadScreenPosition = new Vector3(22.3f, 9.61f, 50);
    private Vector3 Drive_KartingQuadScreenScale = new Vector3(36, 20.2f, 1);
    private Vector3 Drive_KingQuadScreenPosition = new Vector3(0, 16.1f, 50);
    private Vector3 Drive_KingQuadScreenScale = new Vector3(58.68f, 34f, 1);
    private Vector3 Drive_PlayboyQuadScreenPosition = new Vector3(0, 10.1f, 50);
    private Vector3 Drive_PlayboyQuadScreenScale = new Vector3(36.5f, 21f, 1);
    private Vector3 Drive_RattletrapQuadScreenPosition = new Vector3(-15.15f, 7.45f, 50);
    private Vector3 Drive_RattletrapQuadScreenScale = new Vector3(26.65f, 15, 1);

    private Vector3 HemisphereScreenPosition = Vector3.zero;
    private Vector3 HemisphereScale = new Vector3(1206, 1206, 1206);
    private Vector3 OctahedronSphereScreenPosition = Vector3.zero;
    private Vector3 OctahedronSphereScale = new Vector3(0.012f, 0.012f, 0.012f);

    private void Awake()
    {
        Cinema.GvrHead.trackPosition = false;
        Camera.main.transform.localPosition = Vector3.zero;

        SceneModel sceneModel = GlobalVariable.GetSceneModel();
        bool isInteractable = (sceneModel != SceneModel.IMAXTheater && sceneModel != SceneModel.Drive);
        CinemaPanel.ChangeSceneStyle(isInteractable, false);
        switch(sceneModel)
        {
            case SceneModel.Drive:
                DriveModelQuadScreenTrans();
                break;
            default:
                PlayerGameobjectControl.Instance.QuadScreen.transform.localPosition = ImaxQuadScreenPosition;
                PlayerGameobjectControl.Instance.QuadScreen.transform.localScale = ImaxQuadScreenScale;
                break;
        }
        
        PlayerGameobjectControl.Instance.HemisphereScreen.transform.localPosition = HemisphereScreenPosition;
        PlayerGameobjectControl.Instance.HemisphereScreen.transform.localScale = HemisphereScale;
        PlayerGameobjectControl.Instance.SphereScreen.transform.localPosition = OctahedronSphereScreenPosition;
        PlayerGameobjectControl.Instance.SphereScreen.transform.localScale = OctahedronSphereScale;
        //if (GvrViewer.Controller.Eyes.Length == 2)
        //{
        //    foreach (var item in GvrViewer.Controller.Eyes)
        //    {
        //        item.cam.clearFlags = CameraClearFlags.Depth;
        //    }
        //}
        
        CameraMaskControl.GetInstance().HideMask();
    }

    public void DriveModelQuadScreenTrans()
    {
        DriveSceneModel driveModel = GlobalVariable.GetDriveSceneModel();
        switch (driveModel)
        {
            case DriveSceneModel.Karting:
                PlayerGameobjectControl.Instance.QuadScreen.transform.localPosition = Drive_KartingQuadScreenPosition;
                PlayerGameobjectControl.Instance.QuadScreen.transform.localScale = Drive_KartingQuadScreenScale;
                break;
            case DriveSceneModel.King:
                PlayerGameobjectControl.Instance.QuadScreen.transform.localPosition = Drive_KingQuadScreenPosition;
                PlayerGameobjectControl.Instance.QuadScreen.transform.localScale = Drive_KingQuadScreenScale;
                break;
            case DriveSceneModel.Rattletrap:
                PlayerGameobjectControl.Instance.QuadScreen.transform.localPosition = Drive_RattletrapQuadScreenPosition;
                PlayerGameobjectControl.Instance.QuadScreen.transform.localScale = Drive_RattletrapQuadScreenScale;
                break;
            default: //DriveSceneModel.Playboy
                PlayerGameobjectControl.Instance.QuadScreen.transform.localPosition = Drive_PlayboyQuadScreenPosition;
                PlayerGameobjectControl.Instance.QuadScreen.transform.localScale = Drive_PlayboyQuadScreenScale;
                break;
        }
    }
}
