/*
 * Author:李传礼
 * DateTime:2017.4.18
 * Description:预定义类型及公共函数
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine.UI;
#if UNITY_UWP
using Windows.Networking.Connectivity;
#endif

//按钮类型 默认 输入字符 输入数字 JOG操作
public enum BtnType { DEFAULT, INPUTCHAR, INPUTNUM, JOG }

class VirtualKeyStateStr//虚拟按键状态
{
    public bool m_isPress;//是否按下
    public bool m_isChange;//是否修改值

    public VirtualKeyStateStr()
    {
        m_isPress = false;
        m_isChange = false;
    }

    public bool IsPressDown()
    {
        return m_isChange && m_isPress;
    }

    public bool IsPressUp()
    {
        return m_isChange && !m_isPress;
    }
}

public class VirtualKey
{
    static bool m_isCreateDic = false;//是否创建字典
    static Dictionary<string, VirtualKeyStateStr> m_virKeyDownDic;

    static void SetUpVirKeyDic()
    {
        m_virKeyDownDic = new Dictionary<string, VirtualKeyStateStr>();
        m_virKeyDownDic.Add("+X", new VirtualKeyStateStr());
        m_virKeyDownDic.Add("-X", new VirtualKeyStateStr());
        m_virKeyDownDic.Add("+Y", new VirtualKeyStateStr());
        m_virKeyDownDic.Add("-Y", new VirtualKeyStateStr());
        m_virKeyDownDic.Add("+Z", new VirtualKeyStateStr());
        m_virKeyDownDic.Add("-Z", new VirtualKeyStateStr());

        m_virKeyDownDic.Add("+A", new VirtualKeyStateStr());
        m_virKeyDownDic.Add("-A", new VirtualKeyStateStr());
        m_virKeyDownDic.Add("+B", new VirtualKeyStateStr());
        m_virKeyDownDic.Add("-B", new VirtualKeyStateStr());
        m_virKeyDownDic.Add("+C", new VirtualKeyStateStr());
        m_virKeyDownDic.Add("-C", new VirtualKeyStateStr());
    }

    public static bool GetVirKeyDown(string virKey)
    {
        if (!m_isCreateDic) //创造虚拟按键状态
        {
            VirtualKey.SetUpVirKeyDic();
            m_isCreateDic = true;
        }

        if (m_virKeyDownDic.ContainsKey(virKey))
        {
            if (m_virKeyDownDic[virKey].IsPressDown())
            {
                m_virKeyDownDic[virKey].m_isChange = false;
                return m_virKeyDownDic[virKey].m_isPress;
            }
            else
                return false;
        }
        else
        {
            Debug.LogError(virKey + "不存在");
            return false;
        }
    }

    public static bool GetVirKeyUp(string virKey)
    {
        if (!m_isCreateDic) //创造虚拟按键状态
        {
            VirtualKey.SetUpVirKeyDic();
            m_isCreateDic = true;
        }

        if (m_virKeyDownDic.ContainsKey(virKey))
        {
            if (m_virKeyDownDic[virKey].IsPressUp())
            {
                m_virKeyDownDic[virKey].m_isChange = false;
                return !m_virKeyDownDic[virKey].m_isPress;
            }
            else
                return false;
        }
        else
        {
            Debug.LogError(virKey + "不存在");
            return false;
        }
    }

    public static void SetVirKeyState(string virKey, bool isPress)
    {
        if (m_virKeyDownDic.ContainsKey(virKey))
        {
            m_virKeyDownDic[virKey].m_isPress = isPress;
            m_virKeyDownDic[virKey].m_isChange = true;
        }
    }
}

public enum SpaceMode { Screen, World, Self, Other }//空间模式 屏幕 世界 自身 其他（自定义）
public enum GestureDirection { Up, Down, Left, Right, NA }

//消息体
public class WhoseMsg
{
    public string m_from;//来源方 ip:port
    public string m_msg;//消息内容
    public DateTime m_recvTime;//接收时消息

    public WhoseMsg()
    {
        m_from = "";
        m_msg = "";
        m_recvTime = DateTime.Now;
    }
}

[Serializable]
public class JsonBase
{
    public string GetJson()
    {
        return JsonUtility.ToJson(this);
    }

    public int GetLength()
    {
        return GetJson().Length;
    }
}

//Json返回信息
public class JsonReturnInfo : JsonBase
{
    public int code;
    public string message;
}

//Json控制
public class JsonControl
{
    public static void SplitObjAndJson<T>(string str, out T t, out string json) where T : JsonBase
    {
        t = JsonUtility.FromJson<T>(str);
        if(t == null)
        {
            json = "";
            return;
        }

        int index = t.GetLength();
        json = str.Substring(index);
        json = json.Substring(json.IndexOf(":") + 1);
        json = json.Remove(json.Length - 1);
    }

    public static string CombineObjectAndJson<T>(T t, string dataJsonKey, string dataJsonValue) where T : JsonBase
    {
        string str = JsonUtility.ToJson(t);
        string str1 = string.Format(",\"{0}\":{1}", dataJsonKey, dataJsonValue);
        return str.Insert(str.Length - 1, str1);
    }
}

[Serializable]
public class JsonArray<T>
{
    public T[] array;

    public JsonArray(T[] array)
    {
        this.array = array;
    }

    public static T[] GetJsonArray(string json)
    {
        string newJson = "{\"array\":" + json + "}";
        JsonArray<T> jsonArray = JsonUtility.FromJson<JsonArray<T>>(newJson);
        return jsonArray.array;
    }

    public static string ConvertToJson(T[] array)
    {
        JsonArray<T> jsonArray = new JsonArray<T>(array);
        string json = JsonUtility.ToJson(jsonArray);

        string sign = "{\"array\":";
        int index = json.IndexOf(sign);
        if (index != -1)
        {
            json = json.Substring(index + sign.Length);
            json = json.Substring(0, json.Length - 1);
            return json;
        }
        else
            return "";
    }
}

public class MatrixInfo
{
    public float[] TRS;

    public MatrixInfo()
    {
        TRS = new float[9];
    }

    public MatrixInfo(Vector3 t, Vector3 r, Vector3 s)
    {
        TRS = new float[9];
        SetT(t);
        SetR(r);
        SetS(s);
    }

    public MatrixInfo(Transform t, Space space)
    {
        TRS = new float[9];

        if (t != null)
        {
            if (space == Space.World)
            {
                SetT(t.position);
                SetR(t.eulerAngles);
                SetS(t.lossyScale);
            }
            else
            {
                SetT(t.localPosition);
                SetR(t.localEulerAngles);
                SetS(t.localScale);
            }
        }
    }

    public void SetT(Vector3 t)
    {
        TRS[0] = t.x;
        TRS[1] = t.y;
        TRS[2] = t.z;
    }

    public void SetR(Vector3 r)
    {
        TRS[3] = r.x;
        TRS[4] = r.y;
        TRS[5] = r.z;
    }

    public void SetS(Vector3 s)
    {
        TRS[6] = s.x;
        TRS[7] = s.y;
        TRS[8] = s.z;
    }

    public void GetTRS(out Vector3 t, out Vector3 r, out Vector3 s)
    {
        t = new Vector3(TRS[0], TRS[1], TRS[2]);
        r = new Vector3(TRS[3], TRS[4], TRS[5]);
        s = new Vector3(TRS[6], TRS[7], TRS[8]);
    }

    public override bool Equals(object obj)
    {
        MatrixInfo mi = (MatrixInfo)obj;

        if (mi == null || mi.TRS == null)
            return false;

        for(int i = 0; i < 9; i++)
        {
            if (TRS[i] != mi.TRS[i])
                return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

//多点运动单位
public class MotionUnit
{
    Transform MotionGO;
    Transform[] MotionPointers;
    List<Transform> FilterMotionPointerList;
    List<float> PointerFactorList;

    public MotionUnit(Transform go, Transform[] motionPointers)
    {
        MotionGO = go;
        MotionPointers = motionPointers;
        FilterMotionPointerList = new List<Transform>();
        PointerFactorList = new List<float>();
        if (motionPointers == null)
            return;

        int pointCount = 0;
        float totalDis = 0;
        List<float> disList = new List<float>();
        for (int i = 0; i < motionPointers.Length; i++)
        {
            if (motionPointers[i] != null)
            {
                FilterMotionPointerList.Add(motionPointers[i]);
                pointCount++;

                if (pointCount > 1)
                {
                    float dis = Vector3.Distance(FilterMotionPointerList[pointCount - 1].position, FilterMotionPointerList[pointCount - 2].position);
                    totalDis += dis;
                    disList.Add(dis);
                }
            }
        }

        PointerFactorList.Add(0);
        float lens = 0;
        for (int i = 0; i < disList.Count; i++)
        {
            lens += disList[i];
            float t = lens / totalDis;

            PointerFactorList.Add(t);
        }
    }

    public void Motion(float t, Space space = Space.World)
    {
        if (MotionGO == null || MotionPointers == null)
            return;

        if (FilterMotionPointerList.Count == 0 || FilterMotionPointerList.Count == 1)
            return;

        if (t <= 0)//MotionPointerList头两位
        {
            t = t / PointerFactorList[1];
            SetMotionGo(0, 1, t, space);
        }
        else if (t >= 1)//MotionPointerList末两位
        {
            t = (t - PointerFactorList[FilterMotionPointerList.Count - 2]) / (PointerFactorList[FilterMotionPointerList.Count - 1] - PointerFactorList[FilterMotionPointerList.Count - 2]);
            SetMotionGo(FilterMotionPointerList.Count - 2, FilterMotionPointerList.Count - 1, t, space);
        }
        else
        {
            for (int i = PointerFactorList.Count - 1; i >= 0; i--)
            {
                if (PointerFactorList[i] <= t)
                {
                    t = (t - PointerFactorList[i]) / (PointerFactorList[i] - PointerFactorList[i - 1]);
                    SetMotionGo(i - 1, i, t, space);
                }
            }
        }
    }

    void SetMotionGo(int fromIndex, int toIndex, float t, Space space)
    {
        if (space == Space.World)
        {
            MotionGO.position = Vector3.Lerp(FilterMotionPointerList[fromIndex].position, FilterMotionPointerList[toIndex].position, t);
            MotionGO.eulerAngles = Vector3.Lerp(FilterMotionPointerList[fromIndex].eulerAngles, FilterMotionPointerList[toIndex].eulerAngles, t);
            Vector3 lossyScale = Vector3.Lerp(FilterMotionPointerList[fromIndex].lossyScale, FilterMotionPointerList[toIndex].lossyScale, t);
            PreDefScrp.SetGOLossyScale(MotionGO, lossyScale);
        }
        else
        {
            MotionGO.localPosition = Vector3.Lerp(FilterMotionPointerList[fromIndex].localPosition, FilterMotionPointerList[toIndex].localPosition, t);
            MotionGO.localEulerAngles = Vector3.Lerp(FilterMotionPointerList[fromIndex].localEulerAngles, FilterMotionPointerList[toIndex].localEulerAngles, t);
            MotionGO.localScale = Vector3.Lerp(FilterMotionPointerList[fromIndex].localScale, FilterMotionPointerList[toIndex].localScale, t);
        }
    }
}

public class PreDefScrp
{
    public static T GetMax<T>(List<T> list) where T : IComparable
	{
        T max = list[0];
        foreach (T t in list)
        {
            if (t.CompareTo(max) > 0)
                max = t;
        }

        return max;
	}

    public static T GetMin<T>(List<T> list) where T : IComparable
    {
        T min = list[0];
        foreach (T t in list)
        {
            if (t.CompareTo(min) < 0)
                min = t;
        }

        return min;
    }

    public static T GetMax<T>(T[] array) where T : IComparable
    {
        T max = array[0];
        foreach (T t in array)
        {
            if (t.CompareTo(max) > 0)
                max = t;
        }
        return max;
    }

    public static T GetMin<T>(T[] array) where T : IComparable
    {
        T min = array[0];
        foreach (T t in array)
        {
            if (t.CompareTo(min) < 0)
                min = t;
        }
        return min;
    }

    public static T GetMax<T>(T t, T t1) where T : IComparable
    {
        if (t.CompareTo(t1) > 0)
            return t;
        else
            return t1;
    }

    public static T GetMin<T>(T t, T t1) where T : IComparable
    {
        if (t.CompareTo(t1) < 0)
            return t;
        else
            return t1;
    }

    //找出删除和新增的
    public static void ComputeDeleteAddList<T>(List<T> oldList, List<T> newList,
        out List<T> deleteList, out List<T> addList) where T : class
    {
        deleteList = new List<T>();//空
        addList = new List<T>();//满

        //复制
        for (int i = 0; i < newList.Count; i++)
        {
            addList.Add(newList[i]);
        }

        //筛选
        bool isFind = false;
        for (int i = 0; i < oldList.Count; i++)//旧
        {
            foreach (T t in newList)//新
            {
                if (t.Equals(oldList[i]))//重合部分
                {
                    addList.Remove(t);
                    isFind = true;
                    break;
                }
            }

            if (!isFind)//没找到
            {
                deleteList.Add(oldList[i]);
            }

            isFind = false;
        }
    }

    /*
     * 轮盘序号
     * 参数：当前序号，偏移多少，轮盘位数
     * 返回：新的序号
     */
    public static int Roulette(int x, int offset, int loopCount)
    {
        offset %= loopCount;
        int loopIndex = x + offset;
        if (loopIndex < 0)
            loopIndex += loopCount;
        if (loopIndex > loopCount - 1)
            loopIndex -= loopCount;

        return loopIndex;
    }

    //切换Sprite图片
    //public static void ChangeSpritePic(UISprite curSprite, string spriteName, bool isPixelPerfect = true)
    //{
    //    if (curSprite.atlas == null || curSprite.atlas.GetSprite(spriteName) == null) return;

    //    curSprite.spriteName = spriteName;
    //    if (isPixelPerfect)
    //        curSprite.MakePixelPerfect();
    //}

    ////像素大小转换为屏幕坐标系长度(参照物，像素大小)
    //public static float PixedSizeToScreenSize(GameObject go, float pixedSize)
    //{
    //    return pixedSize / UIRoot.GetPixelSizeAdjustment(go);//像素大小除以参照物的像素长度比
    //}

    ////屏幕坐标系长度转换为像素大小(参照物，屏幕长度)
    //public static float ScreenSizeToPixedSize(GameObject go, float screenSize)
    //{
    //    return screenSize * UIRoot.GetPixelSizeAdjustment(go);
    //}

    ////加载外部图片
    //public static IEnumerator LoadPic(UITexture tex, string filePath, bool KeepWH = true)
    //{
    //    Texture2D tex2D = new Texture2D(0, 0);
    //    byte[] datas = File.ReadAllBytes(filePath);
    //    yield return datas;
    //    tex2D.LoadImage(datas);
    //    if (KeepWH)
    //    {
    //        tex.width = tex2D.width;
    //        tex.height = tex2D.height;
    //    }
    //    tex.mainTexture = tex2D;

    //    BoxCollider texCollider = tex.GetComponent<BoxCollider>();
    //    if (texCollider)
    //        texCollider.size = new Vector3(tex.width, tex.height, 1);
    //}

    //public static IEnumerator LoadPicByWWW(UITexture tex, string filePath)
    //{
    //    WWW www = new WWW(filePath);
    //    yield return www;
    //    if (www.isDone)
    //    {
    //        tex.width = www.texture.width;
    //        tex.height = www.texture.height;
    //        tex.mainTexture = www.texture;
    //    }
    //}

    /// <summary>
    /// IO加载，只能加载本地文件，无法加载网络文件
    /// </summary>
    /// <typeparam name="T">加载完成后需要返回的类型</typeparam>
    /// <param name="name">名称</param>
    /// <param name="fullFilePath">全路径</param>
    /// <param name="loadCompleteCallback">加载完成(成功失败， 名称， 对象)</param>
    /// <returns></returns>
    public static void LoadByIO<T>(string name, string fullFilePath, Action<bool, string, T> loadCompleteCallback) where T : class
    {
        if (!File.Exists(fullFilePath))
        {
            loadCompleteCallback(false, name, null);
            return;
        }
        FileStream fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        if (bytes != null)
        {
            if (typeof(T) == typeof(AudioClip))
                ;//loadCompleteCallback(name, www.GetAudioClip() as T);
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            else if (typeof(T) == typeof(MovieTexture))
                ;//loadCompleteCallback(name, www.GetMovieTexture() as T);
#endif
            else if (typeof(T) == typeof(Texture2D) || typeof(T) == typeof(Texture))
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                loadCompleteCallback(true, name, texture as T);
            }
            else if (typeof(T) == typeof(string))
            {
                loadCompleteCallback(true, name, System.Text.Encoding.ASCII.GetString(bytes) as T);
            }
            else if (typeof(T) == typeof(byte[]))
                loadCompleteCallback(true, name, bytes as T);
            else
                loadCompleteCallback(true, name, null);
        }
        else
        {
            loadCompleteCallback(false, name, null);
        }
    }

    //WWW加载 名称 全路径 加载百分比 加载完成(成功失败， 名称， 对象)
    public static IEnumerator LoadByWWW<T>(string name, string fullFilePath, Action<string, float> loadProcessCallback, Action<bool, string, T> loadCompleteCallback, bool isLocal = true) where T : class
    {
        string path = "";
        if (isLocal)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WSAPlayerX86:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerARM:
                    if (fullFilePath.Contains("file:///"))
                        path = fullFilePath;
                    else
                        path = "file:///" + fullFilePath;
                    break;
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                    if (fullFilePath.Contains("file://"))
                        path = fullFilePath;
                    else
                        path = "file://" + fullFilePath;
                    break;
            }
        }
        else
        {
            if (!fullFilePath.Contains("http://"))
                path = "http://" + fullFilePath;
            else
                path = fullFilePath;
        }

        WWW www = new WWW(path);
        yield return www;
        while (!www.isDone)
        {
            if (loadProcessCallback != null)
                loadProcessCallback(name, www.progress);
            yield return null;
        }

        if (www.isDone)
        {
            if (www.error == null)
            {
                if (loadCompleteCallback != null)
                {
                    if (typeof(T) == typeof(AudioClip))
                        ;//loadCompleteCallback(name, www.GetAudioClip() as T);
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                    else if (typeof(T) == typeof(MovieTexture))
                        ;//loadCompleteCallback(name, www.GetMovieTexture() as T);
#endif
                    else if (typeof(T) == typeof(Texture2D) || typeof(T) == typeof(Texture))
                    {
                        Texture2D tempTexture = new Texture2D(4, 4);
                        www.LoadImageIntoTexture(tempTexture);
                        tempTexture.wrapMode = TextureWrapMode.Clamp;
                        loadCompleteCallback(true, name, tempTexture as T);
                        tempTexture = null;
                    }
                    else if (typeof(T) == typeof(string))
                        loadCompleteCallback(true, name, www.text as T);
                    else if (typeof(T) == typeof(AssetBundle))
                    {
                        loadCompleteCallback(true, name, www.assetBundle as T);
                        www.assetBundle.Unload(false);
                    }
                    else if (typeof(T) == typeof(byte[]))
                        loadCompleteCallback(true, name, www.bytes as T);
                    else
                        loadCompleteCallback(true, name, null);
                }
            }
            else
            {
                Debug.Log(www.error);
                loadCompleteCallback(false, name, null);
            }
            www.Dispose();
            www = null;
        }  
    }

    public static IEnumerator LoadTextureByWWW(string name, string fullFilePath, Action<bool, string, Texture2D> loadCompleteCallback, bool isLocal = true)
    {
        string path = "";
        if (isLocal)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WSAPlayerX86:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerARM:
                    if (fullFilePath.Contains("file:///"))
                        path = fullFilePath;
                    else
                        path = "file:///" + fullFilePath;
                    break;
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                    if (fullFilePath.Contains("file://"))
                        path = fullFilePath;
                    else
                        path = "file://" + fullFilePath;
                    break;
            }
        }
        else
        {
            if (!fullFilePath.Contains("http://"))
                path = "http://" + fullFilePath;
            else
                path = fullFilePath;
        }

        WWW www = new WWW(path);
        yield return www;

        if (www.isDone)
        {
            if (www.error == null)
            {
                if (loadCompleteCallback != null)
                {
                    Texture2D tempTexture = new Texture2D(4, 4);
                    www.LoadImageIntoTexture(tempTexture);
                    loadCompleteCallback(true, name, tempTexture);
                    tempTexture = null;
                }
            }
            else
            {
                Debug.Log(www.error);
                loadCompleteCallback(false, name, null);
            }
        }
        www.Dispose();
        www = null;
    }

    public static int LayerMaskToLayerIndex(LayerMask layerMask)
    {
        int layerIndex = (int)Mathf.Log(layerMask.value, 2);
        return layerIndex;
    }

    //修改层
    public static void ChangeLayer(GameObject go, int layer)
    {
        Transform[] transArray = go.GetComponentsInChildren<Transform>();
        foreach (Transform t in transArray)
        {
            t.gameObject.layer = layer;
        }
    }

    //修改碰撞状态
    public static void EnableCollider(GameObject go, bool enable)
    {
        Collider[] colliderArray = go.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliderArray)
        {
            c.enabled = enable;
        }
    }

    //设置透明
    public static void SetTransparentMaterial(GameObject go)
    {
        Renderer[] materialArray = go.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < materialArray.Length; i++)
        {
            foreach (Material m in materialArray[i].sharedMaterials)
            {
                m.shader = Shader.Find("Transparent/Bumped Diffuse");
                Color c = m.color;
                m.color -= new Color(0, 0, 0, c.a);
            }
        }
    }

    //设置物体材质 物体，材质路径
    public static void SetGoMaterial(GameObject go, string materialPath)
    {
        Renderer[] materialArray = go.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < materialArray.Length; i++)
        {
            foreach (Material m in materialArray[i].sharedMaterials)
            {
                m.shader = Shader.Find(materialPath);
                Color c = m.color;
                m.color -= new Color(0, 0, 0, c.a);
            }
        }
    }

    //恢复材质
    public static void RecoverMaterial(GameObject from, GameObject to)
    {
        Renderer[] fromMaterialArray = from.GetComponentsInChildren<Renderer>();
        Renderer[] toMaterialArray = to.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < fromMaterialArray.Length; i++)
        {
            for (int j = 0; j < toMaterialArray[i].materials.Length; j++)
            {
                toMaterialArray[i].materials[j].shader = GameObject.Instantiate(fromMaterialArray[i].materials[j].shader) as Shader;
                toMaterialArray[i].materials[j].color = fromMaterialArray[i].materials[j].color;
                toMaterialArray[i].materials[j].mainTexture = fromMaterialArray[i].materials[j].mainTexture;
            }
        }
    }

    /************************运算公式*************************/
    public static Vector3 ComputeProjection(Vector3 normal, Vector3 v)//投影向量 参数：平面的法向量，向量本身
    {
        float angle = Vector3.Angle(normal, v);//度数
        float length = v.magnitude * Mathf.Cos(angle * Mathf.Deg2Rad);//垂直向量的长度
        Vector3 nVector = normal.normalized * length;

        return v - nVector;
    }

    /*
     修改：因为unity中绕某一轴（例如X轴，X轴指向自己）顺时针旋转为正角度，逆时针旋转为负角度，
     所以改为verticalV = Vector3.Cross(axis, projV).normalized
     其中要指出的是Vector3.Cross(a, b)相当于百度百科中叉乘的bxa
     */
    //向量绕着某个平面的法线旋转角度 axis可以看作某个平面的法线
    public static Vector3 RotateAroundAxis(Vector3 v, Vector3 axis, float angle)
    {
        Vector3 projV = PreDefScrp.ComputeProjection(axis, v);//投影向量
        Vector3 verticalV = Vector3.Cross(axis, projV).normalized;//新得投影向量垂直于旧投影向量的向量，方向指向新向量
        verticalV *= projV.magnitude * Mathf.Sin(angle * Mathf.Deg2Rad);
        Vector3 sectionProjV = projV.normalized * projV.magnitude * Mathf.Cos(angle * Mathf.Deg2Rad);//旧投影向量的一截
        Vector3 newProjV = verticalV + sectionProjV;

        return newProjV + (v - projV);
    }

    //物体旋转方向 同一平面的起始向量，结束向量，平面的X轴 -1顺时针 1逆时针 0任意方向
    public static int RotateDir(Vector3 startV, Vector3 endV, Vector3 XAxis, Vector3 YAxis)
    {
        startV = startV.normalized;
        endV = endV.normalized;

        Vector3 vec1 = Vector3.zero;
        Vector3 vec2 = Vector3.zero;

        float xValue = Vector3.Project(startV, XAxis).magnitude;
        float yValue = Vector3.Project(startV, YAxis).magnitude;
        if (Vector3.Dot(startV, XAxis) < 0)
            xValue *= -1;
        if(Vector3.Dot(startV, YAxis) < 0)
            yValue *= -1;
        vec1.x = xValue;
        vec1.y = yValue;

        xValue = Vector3.Project(endV, XAxis).magnitude;
        yValue = Vector3.Project(endV, YAxis).magnitude;
        if (Vector3.Dot(endV, XAxis) < 0)
            xValue *= -1;
        if (Vector3.Dot(endV, YAxis) < 0)
            yValue *= -1;
        vec2.x = xValue;
        vec2.y = yValue;

        int onLineRes = OntheLine(Vector3.zero, vec1, vec2);
        float dirResX = Vector3.Dot(endV - startV, XAxis);
		dirResX = (int)(dirResX * 100) / 100.0f;//降低精确度

        if (dirResX > 0)
        {
            if (onLineRes == 1)//方向向右，而且原点在向量上方
                return 1;
            else if (onLineRes == -1)
                return -1;
            else
                return -1;
        }
        else if (dirResX < 0)
        {
            if (onLineRes == -1)//方向向左，而且原点在向量下方
                return 1;
            else if (onLineRes == 1)
                return -1;
            else
                return 1;
        }
        else//随机任意方向
        {
            return 0;
        }
    }

    //比较点与直线的位置
    public static int OntheLine(Vector3 point, Vector3 vec1, Vector3 vec2)
    {
        double computeY;//根据直线方程式计算出的Y值
        computeY = ((point.x - vec1.x) * (vec2.y - vec1.y)) / (vec2.x - vec1.x) + vec1.y;
        //123
        computeY = (int)(computeY * 100) / 100.0f;//降低精确度

        if (computeY < point.y)//点在线段上面
        {
            return 1;
        }
        else if (computeY > point.y)//点在线段下面
        {
            return -1;
        }
        else
            return 0;//线段上
    }

    //判断向量在平面上方向处于什么象限 如果在X,Y,-X,-Y上则返回  -1-2-3-4
    public static int GetQuadrant(Vector3 v, Vector3 otherXAxis, Vector3 otherYAxis)
    {
        float res1 = Vector3.Dot(v, otherXAxis);
        float res2 = Vector3.Dot(v, -otherXAxis);
        float res3 = Vector3.Dot(v, otherYAxis);
        float res4 = Vector3.Dot(v, -otherYAxis);

        if (res1 == 0)
            return -1;
        if (res2 == 0)
            return -3;
        if (res3 == 0)
            return -2;
        if (res4 == 0)
            return -4;

        if (res1 > 0)
        {
            if (res3 > 0)
                return 1;
            else
                return 4;
        }
        else
        {
            if (res3 > 0)
                return 2;
            else
                return 3;
        }
    }

    //得到投影长度
    public static float GetProjLen(Vector3 v, Vector3 onNormal)
    {
        float len = Vector3.Project(v, onNormal).magnitude;
        if (Vector3.Dot(v, onNormal) < 0)
            len *= -1;
        return len;
    }
	
	//误差检测 比较数据 标准数据 误差比率（0-1）
    public static bool ErrorDetection(float data, float standData, float ratio)
    {
        if(ratio <0 || ratio > 1)
            return false;
        if (Mathf.Abs(data - standData) / standData <= ratio)
            return true;
        else
            return false;
    }

    ////动态加载Resources里的文件
    //public static IEnumerator LoadPicByResources(UITexture tex, string filePath)
    //{
    //    ResourceRequest rr =  Resources.LoadAsync<Texture2D>(filePath);
    //    while (!rr.isDone)
    //        yield return null;
    //    tex.mainTexture = rr.asset as Texture2D;
    //}

    //物体绕着某一点的特定轴旋转 物体transform 绕点 轴 角度
    public static void RotateAround(Transform trans, Vector3 point, Vector3 axis, float angle)
    {
        Vector3 v = trans.position - point;//计算物体与绕点的向量
        v = RotateAroundAxis(v, axis, angle);//旋转后的向量
        trans.position = v + point;//计算出物体最新坐标
        trans.Rotate(axis, angle);//物体再自身旋转
    }

    ////第二次使用会摧毁之前的脚本
    //public static TweenPosition TweenMoving(GameObject go, Vector3 from, Vector3 to, float spendTime, AnimationCurve animCurve, float delayTime = 0)
    //{
    //    TweenPosition tp = go.GetComponent<TweenPosition>();
    //    if (tp != null)
    //        GameObject.DestroyImmediate(tp);
    //    tp = go.AddComponent<TweenPosition>();

    //    tp.style = UITweener.Style.Once;
    //    tp.worldSpace = true;

    //    tp.from = from;
    //    tp.to = to;
    //    tp.duration = spendTime;
    //    tp.animationCurve = animCurve;
    //    tp.delay = delayTime;
    //    tp.PlayForward();

    //    return tp;
    //}

    //获取24小时制时分秒
    public static string Get24HMS(DateTime time)
    {
        return time.ToString("HH:mm:ss");
    }

    public static string SecondsToHMS(int seconds)
    {
        TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(seconds));
        int hour = ts.Hours;
        int minute = ts.Minutes;
        int second = ts.Seconds;

        return string.Format("{0:#00}:{1:#00}:{2:#00}", hour, minute, second);
    }

#if UNITY_STANDALONE_WIN || UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
    //关闭线程
    public static void CloseThread(ref Thread thd)
    {
        if (thd != null)
        {
            thd.Abort();
            thd = null;
        }
    }
#endif

    //颜色转16进制
    public static string ColorToHex(Color c)
    {
        int r = (int)(c.r * 255);
        int g = (int)(c.g * 255);
        int b = (int)(c.b * 255);
        int a = (int)(c.a * 255);
        string hex = string.Format("{0:x2}{1:x2}{2:x2}{3:x2}", r, g, b, a);//保持十六进制不够两位补够两位 1-01
        return hex;
    }

    //16进制转颜色
    public static Color HexToColor(string hex)
    {
        if (hex.Length < 6 || hex.Length > 9)
            return Color.black;

        if(hex.Length == 7 || hex.Length == 9)
        {
            if (hex[0] == '#')
                hex = hex.Replace("#", "");
            else
                return Color.black;
        }

        Color c;
        string num = hex.Substring(0, 2);
        c.r = Convert.ToInt32(num, 16) / 255.0f;

        num = hex.Substring(2, 2);
        c.g = Convert.ToInt32(num, 16) / 255.0f;

        num = hex.Substring(4, 2);
        c.b = Convert.ToInt32(num, 16) / 255.0f;

        if (hex.Length == 8)
        {
            num = hex.Substring(6, 2);
            c.a = Convert.ToInt32(num, 16) / 255.0f;
        }
        else
            c.a = 1;

        return c;
    }

    //获得节点Id
    public static string GetNodeId(Transform node)
    {
        //NodeId由树形结构生成
        Transform tempNode = node;
        string nodeId = node.name;
        while (tempNode.parent != null)
        {
            tempNode = tempNode.parent;
            nodeId = tempNode.name + "/" + nodeId;
        }
        return nodeId;

        //return node.name;
    }

    public static bool EqualOrIsParent(Transform node, Transform suspectedParent)
    {
        if (node == suspectedParent)
            return true;
        else
        {
            Transform tempNode = node.parent;
            while (tempNode != null)
            {
                if (tempNode == suspectedParent)
                    return true;
                else
                    tempNode = tempNode.parent;
            }

            return false;
        }
    }

    //等待时间
    public static IEnumerator WaitSecondsTime(float seconds, Action func)
    {
        yield return new WaitForSeconds(seconds);
        if (func != null)
            func.Invoke();
    }

    //新建组件
    public static T NewComponent<T>() where T : Component
    {
        GameObject go = new GameObject();
        go.name = "NewComponent";
        T t = go.AddComponent<T>();
        return t;
    }

    public static T[] NewComponents<T>(int count) where T : Component
    {
        T[] components = new T[count];
        for (int i = 0; i < count; i++)
        {
            GameObject go = new GameObject();
            go.name = "NewComponent";
            T t = go.AddComponent<T>();
            components[i] = t;
        }

        return components;
    }

    public static void RectTransformDeepCopy(RectTransform source, RectTransform destination)
    {
        destination.offsetMax = source.offsetMax;
        destination.offsetMin = source.offsetMin;
        destination.pivot = source.pivot;
        destination.sizeDelta = source.sizeDelta;
        destination.anchoredPosition = source.anchoredPosition;
        destination.anchorMax = source.anchorMax;
        destination.anchoredPosition3D = source.anchoredPosition3D;
        destination.anchorMin = source.anchorMin;
    }

    public static string GetIP()
    {
#if UNITY_STANDALONE_WIN || UNITY_IOS || UNITY_ANDROID||UNITY_EDITOR
        //获取说有网卡信息
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface adapter in nics)
        {
            if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                //获取以太网卡<a href="https://www.baidu.com/s?wd=%E7%BD%91%E7%BB%9C%E6%8E%A5%E5%8F%A3&tn=44039180_cpr&fenlei=mv6quAkxTZn0IZRqIHckPjm4nH00T1Ydm1TzP1NhmWw9nvn3nADd0ZwV5Hcvrjm3rH6sPfKWUMw85HfYnjn4nH6sgvPsT6KdThsqpZwYTjCEQLGCpyw9Uz4Bmy-bIi4WUvYETgN-TLwGUv3EnHnvP10YnHRznjf1n1bznjnLrf" target="_blank" class="baidu-highlight">网络接口</a>信息
                IPInterfaceProperties ip = adapter.GetIPProperties();
                //获取单播地址集
                UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                foreach (UnicastIPAddressInformation ipadd in ipCollection)
                {
                    if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (ip.GatewayAddresses[0].Address.ToString() != "0.0.0.0")
                        {
                            return ipadd.Address.ToString();
                        }
                    }
                }
            }
        }
        return null;
#elif UNITY_UWP
        var hosts = NetworkInformation.GetHostNames();
        foreach (var h in hosts) {
            bool isIpaddr = (h.Type == Windows.Networking.HostNameType.Ipv4) || (h.Type == Windows.Networking.HostNameType.Ipv6);
            if (isIpaddr) {
                // 如果不是IP地址表示的名称，则忽略
                IPInformation ipinfo = h.IPInformation;
                // 71表示无线，6表示以太网
                if (ipinfo.NetworkAdapter.IanaInterfaceType == 71 || ipinfo.NetworkAdapter.IanaInterfaceType == 6)
                {
                    return h.DisplayName;
                }
            }

        }
        return null;
#endif
#if UNITY_WSA
        return null;
#endif
    }

    //值互换
    public static void ValueSwap<T>(ref T v1, ref T v2) where T : struct
    {
        T temp = v1;
        v1 = v2;
        v2 = temp;
    }

    //通过文件路径获取文件名，包括后缀
    public static bool GetFullNameByFilePath(string fullFilePath, out string fullFileName)
    {
        fullFileName = "";
        if (fullFilePath == null || fullFilePath.Length == 0)
            return false;

        int i = fullFilePath.LastIndexOf("\\");
        int i1 = fullFilePath.LastIndexOf("/");
        int index = PreDefScrp.GetMax<int>(i, i1);
        if (index < 0)
        {
            Debug.Log("路径错误");
            return false;
        }

        fullFileName = fullFilePath.Substring(index + 1);//文件名包括后缀
        return true;
    }

    //通过文件路径获取文件名，后缀
    public static bool GetNameSuffixByFilePath(string fullFilePath, out string fileName, out string suffix)
    {
        fileName = "";
        suffix = "";
        string fullFileName;
        if (!GetFullNameByFilePath(fullFilePath, out fullFileName))
            return false;

        int index = fullFileName.LastIndexOf(".");
        if (index >= 0)
        {
            fileName = fullFileName.Substring(0, index);//去掉后缀
            suffix = fullFileName.Substring(index + 1);
        }
        else
        {
            fileName = fullFileName;
            suffix = "";
        }
        return true;
    }

    //通过文件路径获取文件名，后缀
    public static bool GetNameByFilePath(string fullFilePath, out string fileName)
    {
        fileName = "";
        if (fullFilePath == null || fullFilePath.Length == 0)
            return false;

        int i = fullFilePath.LastIndexOf("\\");
        int i1 = fullFilePath.LastIndexOf("/");
        int index = PreDefScrp.GetMax<int>(i, i1);
        if (index < 0)
        {
            Debug.Log("路径错误");
            return false;
        }

        fileName = fullFilePath.Substring(index + 1);//文件名包括后缀
        index = fileName.LastIndexOf(".");
        if (index >= 0)
            fileName = fileName.Substring(0, index);//去掉后缀

        return true;
    }

    //通过文件夹路径获取文件夹名
    public static bool GetNameByFolderPath(string folderPath, out string folderName)
    {
        folderName = "";
        if (folderPath == null || folderPath.Length == 0)
            return false;
        char c = folderPath[folderPath.Length - 1];
        if (c == '\\' || c == '/')
            folderPath = folderPath.Substring(0, folderPath.Length - 1);

        int i = folderPath.LastIndexOf("\\");
        int i1 = folderPath.LastIndexOf("/");
        int index = PreDefScrp.GetMax<int>(i, i1);
        if (index < 0)
        {
            Debug.Log("路径错误");
            return false;
        }

        folderName = folderPath.Substring(index + 1);
        return true;
    }

    public static void SetGOLossyScale(Transform go, Vector3 lossyScale)
    {
        if (go == null)
            return;

        Vector3 localScale;
        if (go.parent != null)
        {
            Vector3 parentLossyScale = go.parent.lossyScale;
            localScale = new Vector3(lossyScale.x / parentLossyScale.x, lossyScale.y / parentLossyScale.y, lossyScale.z / parentLossyScale.z);
        }
        else
            localScale = lossyScale;

        go.transform.localScale = localScale;
    }

    //设置UI宽高
    public static void SetWidgetSize(RectTransform widget, Vector2 size)
    {
        float w = widget.rect.width;
        float h = widget.rect.height;
        widget.sizeDelta = new Vector2(widget.sizeDelta.x + (size.x - w), widget.sizeDelta.y + (size.y - h));
    }

    //适配UI宽高
    public static void AdaptWidgetSize(RectTransform widget, Vector2 StandardSize)
    {
        float w = widget.rect.width;
        float h = widget.rect.height;
        float sdW = StandardSize.x;
        float sdH = StandardSize.y;

        float wRatio = w / sdW;
        float hRatio = h / sdH;

        if (wRatio > hRatio)
        {
            if (w > sdW)
            {
                h = sdW * h / w;
                w = sdW;
            }
        }
        else
        {
            if (h > sdH)
            {
                w = sdH * w / h;
                h = sdH;
            }
        }

        SetWidgetSize(widget, new Vector2(w, h));
    }

    //获取曲线因子 曲线 线型t（标准范围是0-1）
    public static float GetCurveT(AnimationCurve animCurve, float lineT)
    {
        if (lineT < 0)
            lineT = 0;
        else if (lineT > 1)
            lineT = 1;

        return animCurve.Evaluate(lineT / 1);
    }

    public static void CopyTexture2D(Texture2D texSource, out Texture2D texDestination)
    {
        if (texSource != null)
        {
            texDestination = new Texture2D(texSource.width, texSource.height, texSource.format, texSource.mipmapCount > 1);
            texDestination.LoadRawTextureData(texSource.GetRawTextureData());
            texDestination.Apply();
        }
        else
            texDestination = null;
    }

    public static string WeekConvertToString(DayOfWeek week, SystemLanguage launguage)
    {
        if (launguage == SystemLanguage.English)
        {
            return week.ToString();
        }
        else if (launguage == SystemLanguage.ChineseSimplified)
        {
            switch (week)
            {
                case DayOfWeek.Sunday:
                    return "Launcher.SettingPanel.ClockPanel.Time.Week.Sunday";
                case DayOfWeek.Monday:
                    return "Launcher.SettingPanel.ClockPanel.Time.Week.Monday";
                case DayOfWeek.Tuesday:
                    return "Launcher.SettingPanel.ClockPanel.Time.Week.Tuesday";
                case DayOfWeek.Wednesday:
                    return "Launcher.SettingPanel.ClockPanel.Time.Week.Wednesday";
                case DayOfWeek.Thursday:
                    return "Launcher.SettingPanel.ClockPanel.Time.Week.Thursday";
                case DayOfWeek.Friday:
                    return "Launcher.SettingPanel.ClockPanel.Time.Week.Friday";
                case DayOfWeek.Saturday:
                    return "Launcher.SettingPanel.ClockPanel.Time.Week.Saturday";
                default:
                    return "Launcher.SettingPanel.ClockPanel.Time.Week.Sunday";
            }
        }
        else
            return week.ToString();
    }

    public static GestureDirection ComputeGestureDirection(Vector2 v)
    {
        Vector3 rV = Vector3.Project(v, Vector3.right);
        Vector3 uV = Vector3.Project(v, Vector3.up);

        if (rV.magnitude > uV.magnitude)
        {
            if (rV.x > 0)
                return GestureDirection.Right;
            else if (rV.x < 0)
                return GestureDirection.Left;
            else
                return GestureDirection.NA;
        }
        else
        {
            if (uV.y > 0)
                return GestureDirection.Up;
            else if (uV.y < 0)
                return GestureDirection.Down;
            else
                return GestureDirection.NA;
        }
    }

    private const float DELTA = 1.0e-7f;
    private const float RIGHT = 28;
    private const float UP = 151;
    private const float LEFT = 224;
    private const float DOWN = 317;
    public static GestureDirection GetGestureDirection(Vector2 overallVelocity)
    {
        float x, y, gesture_angle;
        x = overallVelocity.x;
        if (x == 0)
            x += DELTA;
        y = -overallVelocity.y;
        gesture_angle = Mathf.Atan2(y, x) * 180 / Mathf.PI;
        if (gesture_angle < 0)
            gesture_angle += 360;

        if (gesture_angle < RIGHT)
        {
            return GestureDirection.Right;
        }
        if (gesture_angle < UP)
        {
            return GestureDirection.Up;
        }
        if (gesture_angle < LEFT)
        {
            return GestureDirection.Left;
        }
        if (gesture_angle < DOWN)
        {
            return GestureDirection.Down;
        }
        return GestureDirection.Right;
    }

    public static float ComputeGestureDirectionLen(GestureDirection gestureDir, Vector2 v)
    {
        float len = 0;

        if(gestureDir == GestureDirection.Up)
        {
            len = GetProjLen(v, Vector3.up);
        }
        else if(gestureDir == GestureDirection.Down)
        {
            len = GetProjLen(v, Vector3.down);
        }
        else if(gestureDir == GestureDirection.Left)
        {
            len = GetProjLen(v, Vector3.left);
        }
        else if (gestureDir == GestureDirection.Right)
        {
            len = GetProjLen(v, Vector3.right);
        }

        return len;
    }
}
