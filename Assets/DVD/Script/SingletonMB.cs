/*
 * Description:具有MonoBehaviour特性的单例类
 */
using UnityEngine;

public class SingletonMB<T> : MonoBehaviour where T : Component
{
    private static T Instance;

    public static T GetInstance()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType<T>();
            if(Instance == null)
            {
                GameObject go = new GameObject();
                Instance = go.AddComponent<T>();
                go.name = Instance.ToString();
            }

            //在切换场景时，使其他继承的类的信息能够保存
            //DontDestroyOnLoad(Instance.gameObject);
        }
        return Instance;
    }
}