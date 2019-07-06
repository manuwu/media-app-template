using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameobjectControl : MonoBehaviour
{
    public Renderer QuadScreen;
    public Renderer HemisphereScreen;
    public Renderer SphereScreen;

    private static PlayerGameobjectControl s_Instance;
    public static PlayerGameobjectControl Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<PlayerGameobjectControl>();
                DontDestroyOnLoad(s_Instance.gameObject);
            }

            return s_Instance;
        }
    }

    private void Awake()
    {
        // 确保只会创建一次该对象
        if (s_Instance == null)
        {
            s_Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != s_Instance)
        {
            Destroy(gameObject);
        }
    }
}
