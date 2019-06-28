using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DVDFileInfo : MonoBehaviour {

    public string fileUrl;
    public string fileName;
    public bool isDir;
    public string extension;
    public string directoryName;
    public AndroidJavaObject javaObj;

    public DVDFileInfo()
    {
        this.fileUrl = string.Empty;
        this.fileName = string.Empty;
        this.isDir = true;
        this.extension = string.Empty;
        javaObj = null;
    }

    public DVDFileInfo(string fileName)
    {
        this.fileUrl = string.Empty;
        this.fileName = fileName;
        this.isDir = true;
        this.extension = string.Empty;
        javaObj = null;
    }


}
