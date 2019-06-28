using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace IVR.Language
{
    public class LanguageTest : MonoBehaviour
    {
        public Text mText;
        private void Start()
        {
            //LanguageManager.initLanguage(new XMLParser());//需要在程序开始时初始化
            mText.SetTextByKey("Update.Sure");
            mText.SetTextByKey("Update.Detail.Cancel");
            mText.SetTextByKey("Update.Detail.DownLoad");
            mText.SetTextByKey("Update.ReadyToDload");
        }

    }
}
