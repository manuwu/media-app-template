using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SVR.Coloring;

public class ColoringTest : MonoBehaviour
{
    private ColorTheme[] colorThemes = { ColorTheme.Green, ColorTheme.Blue };
    private int index = 0;

    public void ChangeColorTheme()
    {
        ColoringManager.Instance.CurrentColorTheme = colorThemes[index % 2];
        index++;
    }
}
