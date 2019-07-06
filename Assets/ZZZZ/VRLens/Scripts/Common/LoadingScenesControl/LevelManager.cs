using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    [SerializeField]
    private string SceneName;
    private void Awake()
    {
        StartCoroutine(ToLoad());
    }
    private IEnumerator ToLoad()
    {
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
}
