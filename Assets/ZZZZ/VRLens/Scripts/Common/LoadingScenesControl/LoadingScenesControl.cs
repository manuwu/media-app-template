using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScenesControl : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(ToLoad());
    }

    private IEnumerator ToLoad()
    {
        string SceneName = string.Empty;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (Svr.SvrSetting.IsVR9Device)
            SceneName = "Launcher_Environment_VR9";
        else
            SceneName = "Launcher_Environment_Master";
#else
        SceneName = "Launcher_Environment_VR9";
#endif
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneName);
    }
}
