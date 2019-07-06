using UnityEngine;
using System.Collections;
using System.IO;

public class WriteLogScrp : SingletonMB<WriteLogScrp> 
{
    //static WriteLogScrp instance;
    StreamWriter sw;

    void OnDestroy()
    {
        CloseWrite();
    }

//    public static WriteLogScrp GetInstance()
//    {
//        if (instance == null)
//            instance = new WriteLogScrp();
//
//        return instance;
//    }

    public void SetFolder(string path)
    {
        if (path.LastIndexOf(".txt") != path.Length - 4)
        {
			string endString = path.Substring(path.Length - 1);
			if (endString != "/" && endString != "\\")
            {
                path += "/log.txt";
            }
            else
            {
                path += "log.txt";
            }
        }
        else
        {
            if (!path.Contains("/") && !path.Contains("\\"))
                path += "D:/LCLLog/" + path;
        }

		if(!File.Exists(path))
		{
            int index = -1;
            if (path.Contains("/"))
                index = path.LastIndexOf("/");
            if (path.Contains("\\"))
            {
                if (index > path.LastIndexOf("\\"))
                    index = path.LastIndexOf("\\");
            }

            Directory.CreateDirectory(path.Substring(0, index));
			File.Create(path);
		}
        sw = new StreamWriter(path, false);
    }

    public void WriteLog(string info)
    {
        if (sw == null)
            SetFolder("D:/LCLLog/");

        sw.WriteLine(info + " " + System.DateTime.Now.ToString());
    }

    public void CloseWrite()
    {
		if(sw != null)
        	sw.Close();
    }
}
