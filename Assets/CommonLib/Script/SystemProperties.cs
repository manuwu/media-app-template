using UnityEngine;
using System.Collections;

public class SystemProperties
{
    private static SystemProperties mInstance = new SystemProperties ();
    private AndroidJavaClass jc;

    private SystemProperties(){
        #if UNITY_EDITOR
        #else
        jc = new AndroidJavaClass("android.os.SystemProperties");
        #endif
    }

    private string getProperty(string key, string def){
        #if UNITY_EDITOR
        return def;
        #else
        return jc.CallStatic<string> ("get", key, def);
        #endif
    }

    private void setProperty (string key, string value){
        #if UNITY_EDITOR
        //Do nothing in editor mode
        #else
        jc.CallStatic("set", key, value);
        #endif
    }

    public static string get(string key, string def){
        return mInstance.getProperty(key, def);
    }

    public static void set(string key, string value){
        mInstance.setProperty(key, value);
    }

    private static string getInternalVersion(){
        return get ("ro.build.display.version", "");
    }

    public static string getChannelNumber(){
        string internalVersion = getInternalVersion ();
        string[] ss = internalVersion.Split(' ');
        if (ss.Length < 3){
            return "000";
        }
        return ss[ss.Length - 2];
    }

    public static string getProjectCode(){
        string internalVersion = getInternalVersion ();
        string[] ss = internalVersion.Split(' ');
        if (ss.Length < 2){
            return "";
        }
        return ss[1].ToUpper();
    }

    private static readonly string[] SPECIAL_CHANNEL_LIST = {"002", "004", "007"};

    public static bool isSpecialChannel(){
        string channel = getChannelNumber();
        foreach (string s in SPECIAL_CHANNEL_LIST) {
            if (channel.Equals(s)){
                return true;
            }
        }
        return false;
    }

}

