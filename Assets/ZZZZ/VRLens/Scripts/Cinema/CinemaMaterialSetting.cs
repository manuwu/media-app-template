using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaMaterialSetting : SingletonMB<CinemaMaterialSetting>
{
    [HideInInspector]
    public Material DefaultMat;
    [HideInInspector]
    public Material StarringMat;

    private GameObject SphereSkyboxGo;
    public GameObject ImaxPurple;
    public GameObject DriveSceneBox;

    private SceneModel currentSceneModel;

    private void Awake()
    {
        currentSceneModel = GlobalVariable.GetSceneModel();
        if (currentSceneModel == SceneModel.Default)
            SetDefaultScene();
        else if (currentSceneModel == SceneModel.IMAXTheater)
            SetIMAXTheaterScene();
        else if (currentSceneModel == SceneModel.StarringNight)
            SetStarringNightScene();
        else if (currentSceneModel == SceneModel.Drive)
        {
            DriveSceneModel driveModel = GlobalVariable.GetDriveSceneModel();
            switch (driveModel)
            {
                case DriveSceneModel.Karting:
                    SetDrive_KartingScene();
                    break;
                case DriveSceneModel.King:
                    SetDrive_KingScene();
                    break;
                case DriveSceneModel.Rattletrap:
                    SetDrive_RattletrapScene();
                    break;
                default: //DriveSceneModel.Playboy
                    SetDrive_PlayboyScene();
                    break;
            }
        }
    }

    public void SetIMAXTheaterScene()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Svr.SvrSetting.IsVR9Device)
            ImaxPurple.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("SceneMaterial/vr_9_ImaxSkyMat");
        else
            ImaxPurple.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("SceneMaterial/vr_901_ImaxSkyMat");
#else
        ImaxPurple.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("SceneMaterial/vr_9_ImaxSkyMat");
#endif
    }

    public void SetStarringNightScene()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Svr.SvrSetting.IsVR9Device)
            StarringMat = Resources.Load<Material>("CinemaMaterial/Cinema_VR9_StarringNight");
        else
            StarringMat = Resources.Load<Material>("SceneMaterial/vr_901_SkyboxPurple");
#else
        StarringMat = Resources.Load<Material>("CinemaMaterial/Cinema_VR9_StarringNight");
#endif
        LoadRender(StarringMat);
    }

    public void SetDefaultScene()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Svr.SvrSetting.IsVR9Device)
            DefaultMat = Resources.Load<Material>("CinemaMaterial/Cinema_VR9_Default");
        else
            DefaultMat = Resources.Load<Material>("SceneMaterial/vr_901_SkyMovieDome");
#else
        DefaultMat = Resources.Load<Material>("CinemaMaterial/Cinema_VR9_Default");
#endif
        LoadRender(DefaultMat);
    }

    public void SetDrive_KartingScene()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Svr.SvrSetting.IsVR9Device)
            DriveSceneBox.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("CinemaMaterial/Drive/Cinema_VR9_Drive_Karting");
        else
            DriveSceneBox.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("CinemaMaterial/Drive/Cinema_901_Drive_Karting");
#else
        DriveSceneBox.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("CinemaMaterial/Drive/Cinema_VR9_Drive_Karting");
#endif
    }

    public void SetDrive_KingScene()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Svr.SvrSetting.IsVR9Device)
            DriveSceneBox.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("CinemaMaterial/Drive/Cinema_VR9_Drive_King");
        else
            DriveSceneBox.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("CinemaMaterial/Drive/Cinema_901_Drive_King");
#else
        DriveSceneBox.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("CinemaMaterial/Drive/Cinema_VR9_Drive_King");
#endif
    }

    public void SetDrive_PlayboyScene()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Svr.SvrSetting.IsVR9Device)
            DriveSceneBox.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("CinemaMaterial/Drive/Cinema_VR9_Drive_Playboy");
        else
            DriveSceneBox.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("CinemaMaterial/Drive/Cinema_901_Drive_Playboy");
#else
        DriveSceneBox.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("CinemaMaterial/Drive/Cinema_VR9_Drive_Playboy");
#endif
    }

    public void SetDrive_RattletrapScene()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Svr.SvrSetting.IsVR9Device)
            DriveSceneBox.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("CinemaMaterial/Drive/Cinema_VR9_Drive_Rattletrap");
        else
            DriveSceneBox.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("CinemaMaterial/Drive/Cinema_901_Drive_Rattletrap");
#else
        DriveSceneBox.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("CinemaMaterial/Drive/Cinema_VR9_Drive_Rattletrap");
#endif
    }

    public void LoadRender(Material material)
    {
        RenderSettings.skybox = material;
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        if (GvrViewer.Controller.Eyes.Length == 2)
        {
            foreach (var item in GvrViewer.Controller.Eyes)
            {
                item.cam.clearFlags = CameraClearFlags.Skybox;
            }
        }
    }

    public void UnLoadRender()
    {
        RenderSettings.skybox = null;
        Camera.main.clearFlags = CameraClearFlags.Nothing;
        if (GvrViewer.Controller.Eyes.Length == 2)
        {
            foreach (var item in GvrViewer.Controller.Eyes)
            {
                item.cam.clearFlags = CameraClearFlags.Depth;
            }
        }

    }

    private void OnDestroy()
    {
        DefaultMat = null;
        StarringMat = null;
        SphereSkyboxGo = null;
        ImaxPurple = null;
        DriveSceneBox = null;
    }
}
