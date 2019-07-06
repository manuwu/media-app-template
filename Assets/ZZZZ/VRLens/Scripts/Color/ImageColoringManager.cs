using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SVR.Coloring;

public class ImageColoringManager : MonoBehaviour
{
    [SerializeField]
    private string NormalColor;
    [SerializeField]
    private string HoverColor;
    [SerializeField]
    private string SelectedColor;
    [SerializeField]
    private string DisableColor;

    private Image _image;
    private string _key;
    public bool isAlwaysSelectedStay;
    private bool isNeedInit = true;
    void Awake ()
    {
        if (_image == null)
            _image = gameObject.GetComponent<Image>();
        isAlwaysSelectedStay = false;
        if (isNeedInit)
            OnNormalStatus();
    }
	
    public void OnNormalStatus()
    {
        if (isAlwaysSelectedStay)
        {
            return;
        }
        if (_image == null)
            _image = gameObject.GetComponent<Image>();

        if (_image != null)
            _image.SetImageColorByKey(NormalColor);
        _key = NormalColor;
    }

    public void OnHoverStatus()
    {
        if (_image == null)
            _image = gameObject.GetComponent<Image>();

        if (_image != null)
            _image.SetImageColorByKey(HoverColor);
        _key = HoverColor;
        isNeedInit = false;
    }

    public void OnSelectedStatus()
    {
        if (_image == null)
            _image = gameObject.GetComponent<Image>();

        if (_image != null)
            _image.SetImageColorByKey(SelectedColor);
        _key = SelectedColor;
        isNeedInit = false;
    }

    public void OnDisableStatus()
    {
        if (_image == null)
            _image = gameObject.GetComponent<Image>();

        if (_image != null)
            _image.SetImageColorByKey(DisableColor);
        _key = DisableColor;
        isNeedInit = false;
    }

    public Color GetCurColor()
    {
        return Coloring.Get(_key);
    }
}
