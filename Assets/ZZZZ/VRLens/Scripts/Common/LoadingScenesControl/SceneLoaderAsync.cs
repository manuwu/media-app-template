using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderAsync : SingletonMB<SceneLoaderAsync>
{

    // Loading Progress: private setter, public getter
    private float _loadingProgress;
    public float LoadingProgress { get { return _loadingProgress; } }

    //private void SceneManager_sceneUnloaded(Scene arg0)
    //{
    //    Debug.Log("SceneManager_sceneUnloaded:" + arg0.name);
    //}

    //private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    //{
    //    Debug.Log("SceneManager_sceneLoaded:" + arg0.name + "," + arg1);
    //}

    public void LoadScene(string name)
    {
        // kick-off the one co-routine to rule them all
        StartCoroutine(LoadScenesInOrder(name));
    }

    private IEnumerator LoadScenesInOrder(string name)
    {
        // LoadSceneAsync() returns an AsyncOperation, 
        // so will only continue past this point when the Operation has finished
        CameraMaskControl.GetInstance().ShowMask();
        GameObject.FindGameObjectWithTag("CinemaUI").SetActive(false);
        yield return SceneManager.LoadSceneAsync(name);

        // as soon as we've finished loading the loading screen, start loading the game scene
        //yield return StartCoroutine(LoadSceneMain("Cinema"));
    }

    private IEnumerator LoadSceneMain(string sceneName)
    {
        var asyncScene = SceneManager.LoadSceneAsync(sceneName);
        yield return asyncScene;
        // this value stops the scene from displaying when it's finished loading
        //asyncScene.allowSceneActivation = false;

        //while (!asyncScene.isDone)
        //{
        //    // loading bar progress
        //    _loadingProgress = Mathf.Clamp01(asyncScene.progress / 0.9f) * 100;

        //    // scene has loaded as much as possible, the last 10% can't be multi-threaded
        //    if (asyncScene.progress >= 0.9f)
        //    {
        //        // we finally show the scene
        //        asyncScene.allowSceneActivation = true;
        //    }

        //    yield return null;
        //}
    }
}
