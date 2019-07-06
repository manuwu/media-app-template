using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SVR.Coloring;

public class TextColoringManager : MonoBehaviour
{
    [SerializeField]
    private string NormalColor;
    [SerializeField]
    private string HoverColor;
    [SerializeField]
    private string SelectedColor;
    [SerializeField]
    private string DisableColor;

    private Text _text;
    private string _key;

    void Awake()
    {
        if(_text == null)
            _text = gameObject.GetComponent<Text>();

        OnNormalStatus();
    }

    public void OnNormalStatus()
    {
        if (_text == null)
            _text = gameObject.GetComponent<Text>();

        if(_text != null)
            _text.SetTextColorByKey(NormalColor);
        _key = NormalColor;
    }

    public void OnHoverStatus()
    {
        if (_text == null)
            _text = gameObject.GetComponent<Text>();

        if(_text != null)
            _text.SetTextColorByKey(HoverColor);
        _key = HoverColor;
    }

    public void OnSelectedStatus()
    {
        if (_text == null)
            _text = gameObject.GetComponent<Text>();

        if(_text != null)
            _text.SetTextColorByKey(SelectedColor);
        _key = SelectedColor;
    }

    public void OnDisableStatus()
    {
        if (_text == null)
            _text = gameObject.GetComponent<Text>();

        if(_text != null)
            _text.SetTextColorByKey(DisableColor);
        _key = DisableColor;
    }

    public Color GetCurColor()
    {
        return Coloring.Get(_key);
    }
}
