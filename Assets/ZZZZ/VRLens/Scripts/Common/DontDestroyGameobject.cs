using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyGameobject : MonoBehaviour {

    private static DontDestroyGameobject s_Instance;

    public static DontDestroyGameobject Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<DontDestroyGameobject>();
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
