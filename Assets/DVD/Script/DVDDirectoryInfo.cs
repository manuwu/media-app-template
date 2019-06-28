using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DVDDirectoryInfo : MonoBehaviour {

    public string name;
    public bool exists;
    public string path;
    public string fullName;
    public AndroidJavaObject javaObj;
    public bool isDir;
    public string extension;
    private List<DVDDirectoryInfo> directArray=new List<DVDDirectoryInfo>();
    private  List<DVDFileInfo> fileArray=new List<DVDFileInfo>();
    public DVDDirectoryInfo()
    {
        this.name = string.Empty;
        this.exists = true;
        this.path = string.Empty;
        this.fullName = string.Empty;
        this.javaObj = null;
        this.isDir = true;
        this.extension = string.Empty;
    }

    public DVDDirectoryInfo(string path)
    {
        this.name = path;
        this.exists = true;
        this.path = path;
        this.fullName = path;
        this.javaObj = null;
        this.isDir = true;
        this.extension = string.Empty;
    }

    public void  AnalyzeDirectorys()
    {
        directArray.Clear();
        fileArray.Clear();
        DVDDirectoryInfo[] dctArray=DvDInterface.GetInstance().GetFileList(this.javaObj);
        for (int i = 0; i < dctArray.Length; i++)
        {
            if (dctArray[i].isDir)
            {
                directArray.Add(dctArray[i]);
            }
            else
            {
                DVDFileInfo file = new DVDFileInfo();
                file.fileName = dctArray[i].name;
                file.fileUrl = dctArray[i].path;
                file.isDir = dctArray[i].isDir;
                file.extension = dctArray[i].extension;
                file.javaObj= dctArray[i].javaObj;
                fileArray.Add(file);
            }
        }
    }

    public DVDDirectoryInfo[] GetDirectorys()
    {
        return directArray.ToArray();
    }

    public DVDFileInfo[] GetFiles()
    {
        return fileArray.ToArray();
    }
}
