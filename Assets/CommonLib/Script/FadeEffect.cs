using UnityEngine;
using System.Collections;
using System;

public class ScreenAnimationFadeOut : IEnumerator
{
    private Coroutine fadeoutCoroutine;
    public ScreenAnimationFadeOut()
    {
        FadeEffect effect = FadeEffect.Instance;
        if (effect == null)
        {
            Debug.LogWarning("You need attach FadeEffect on to the main camera.");
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            if (mainCamera == null)
                throw new UnityException("Can't find MainCamera");
            effect = GameObject.FindGameObjectWithTag("MainCamera").AddComponent<FadeEffect>();
        }
            
        if (fadeoutCoroutine != null)
            effect.StopCoroutine(fadeoutCoroutine);
        fadeoutCoroutine = FadeEffect.Instance.StartFadeOut();
    }
    public object Current
    {
        get
        {
            return null;
        }
    }

    public bool MoveNext()
    {
        return FadeEffect.Instance.FadeoutOver();
    }

    public void Reset()
    {

    }
}


[RequireComponent(typeof(Camera))]
/// <summary>
/// Fades the screen from black after a new scene is loaded.
/// </summary>
public class FadeEffect : MonoBehaviour
{
    public bool needFadeIn = true;
    /// <summary>
    /// How long it takes to fade.
    /// </summary>
    public float fadeTime = 1.0f;
    public float fadeInDelay = 0.0f;
    /// <summary>
    /// The initial screen color.
    /// </summary>
    public Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);

    private Material fadeMaterial = null;
    private bool isFading = false;
    private YieldInstruction fadeInstruction = new WaitForEndOfFrame();
    private Camera mCamera;
    private Vector3[] mVertex;

    private GameObject UseFadeObject;
    private MeshRenderer mFadeRender;
    private MeshFilter mFadeFileter;
    public static FadeEffect Instance { get; private set; }
    /// <summary>
    /// Initialize.
    /// </summary>
    void Awake()
    {
        Instance = this;
        // create the fade material
        fadeMaterial = new Material(Shader.Find("Unlit/Transparent HUD"));
        fadeMaterial.color = fadeColor;
        mCamera = GetComponent<Camera>();

        mVertex = GetVertexBySize(4, Vector3.zero);
    }


    /// <summary>
    /// Starts the fade in
    /// </summary>
    void OnEnable()
    {
        //IVRManager.OnPostRenderEvent += IVRManager_OnPostRenderEvent;

        if (needFadeIn)
            StartCoroutine(FadeIn());
    }


    /// <summary>
    /// Cleans up the fade material
    /// </summary>
    void OnDestroy()
    {
        //IVRManager.OnPostRenderEvent -= IVRManager_OnPostRenderEvent;
        if (fadeMaterial != null)
        {
            Destroy(fadeMaterial);
        }
    }

    /// <summary>
    /// Fades alpha from 1.0 to 0.0
    /// </summary>
    IEnumerator FadeIn()
    {
        float elapsedTime = 0.0f;
        fadeMaterial.color = fadeColor;
        Color color = fadeColor;
        isFading = true;
        if (fadeInDelay > 0)
            yield return new WaitForSeconds(fadeInDelay);
        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            color.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            fadeMaterial.color = color;
        }
        isFading = false;
    }
    public Coroutine StartFadeOut()
    {
        StopAllCoroutines();
        return StartCoroutine(FadeOut());
    }
    IEnumerator FadeOut()
    {
        float elapsedTime = 0.0f;
        Color color = fadeMaterial.color = new Color(0.01f, 0.01f, 0.01f, 0.0f);
        isFading = true;
        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeTime);
            fadeMaterial.color = color;
        }
    }
    public bool FadeoutOver()
    {
        return (int)fadeMaterial.color.a != 1;
    }
#if !UNITY_EDITOR
    /// <summary>
    /// Renders the fade overlay when attached to a camera object
    /// </summary>
    void OnPostRender()
    {
        if (isFading)
        {
            fadeMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadOrtho();
            //IVR.IVR_GL.LoadOrtho(fadeMaterial);
            GL.Color(fadeMaterial.color);
            GL.Begin(GL.QUADS);
            GL.Vertex3(0f, 0f, -12f);
            GL.Vertex3(0f, 1f, -12f);
            GL.Vertex3(1f, 1f, -12f);
            GL.Vertex3(1f, 0f, -12f);
            GL.End();
            GL.PopMatrix();
        }
    }
#else
    void OnPostRender()// OnPreRender OnPostRender
    {
        if (isFading)
        {
            fadeMaterial.SetPass(0);
            GL.PushMatrix();
            GL.Color(fadeMaterial.color);
            GL.Begin(GL.TRIANGLES);

            for (int i = 0; i < mVertex.Length; i++)
            {
                GL.Vertex(transform.localToWorldMatrix.MultiplyPoint(mVertex[i]));
            }
            GL.End();
            GL.PopMatrix();
        }
    }
#endif
    // cube ///////////////////////////////////////////////////////////////////////
    //  v6------v5
    // / |     / |
    // v1------v0|
    // | |     | |
    // | |v7---|-|v4
    // |/      |/
    // v2------v3
    Vector3[] GetVertexBySize(float size,Vector3 origin)
    {
        size = Mathf.Abs(size);
        Vector3 p0 = new Vector3(size * 0.5f, size*0.5f, -size*0.5f);
        Vector3 p1 = new Vector3(-size*0.5f, size*0.5f, -size*0.5f);
        Vector3 p2 = new Vector3(-size*0.5f, -size*0.5f, -size*0.5f);
        Vector3 p3 = new Vector3(size*0.5f, -size*0.5f, -size*0.5f);
        Vector3 p4 = new Vector3(size*0.5f, -size*0.5f, size*0.5f);
        Vector3 p5 = new Vector3(size*0.5f, size*0.5f, size*0.5f);
        Vector3 p6 = new Vector3(-size*0.5f, size*0.5f, size*0.5f);
        Vector3 p7 = new Vector3(-size*0.5f, -size*0.5f, size*0.5f);
        p0 += origin;
        p1 += origin;
        p2 += origin;
        p3 += origin;
        p4 += origin;
        p5 += origin;
        p6 += origin;
        p7 += origin;
        Vector3[] vertex = new Vector3[] { p0,p1,p2,p2,p3,p0,
                                               p0,p3,p4,p4,p5,p0,
                                               p0,p5,p6,p6,p1,p0,
                                               p3,p2,p7,p7,p4,p3,
                                               p7,p2,p1,p1,p6,p7,
                                               p7,p6,p5,p5,p4,p7};
        return vertex;
    }

    Mesh GetCubeMesh(float size)
    {
        size = Mathf.Abs(size);
        Mesh mesh = new Mesh();

        Vector3[] vertex = new Vector3[8];
        Vector3 p0 = new Vector3(size*0.5f, size*0.5f, -size*0.5f);
        Vector3 p1 = new Vector3(-size*0.5f, size*0.5f, -size*0.5f);
        Vector3 p2 = new Vector3(-size*0.5f, -size*0.5f, -size*0.5f);
        Vector3 p3 = new Vector3(size*0.5f, -size*0.5f, -size*0.5f);
        Vector3 p4 = new Vector3(size*0.5f, -size*0.5f, size*0.5f);
        Vector3 p5 = new Vector3(size*0.5f, size*0.5f, size*0.5f);
        Vector3 p6 = new Vector3(-size*0.5f, size*0.5f, size*0.5f);
        Vector3 p7 = new Vector3(-size*0.5f, -size*0.5f, size*0.5f);
        mesh.vertices = new Vector3[] { p0,p1,p2,p2,p3,p0,
                                               p0,p3,p4,p4,p5,p0,
                                               p0,p5,p6,p6,p1,p0,
                                               p3,p2,p7,p7,p4,p3,
                                               p7,p2,p1,p1,p6,p7,
                                               p7,p6,p5,p5,p4,p7};
        mesh.triangles = new int[] { 0,1,2,3,4,5,
                                      6,7,8,9,10,11,
                                       12,13,14,15,16,17,
                                        18,19,20,21,22,23,
                                        24,25,26,27,28,29,
                                        30,31,32,33,34,35};

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
