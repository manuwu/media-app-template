// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// Only invoke custom build processor when building for Android or iOS.
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using System.Linq;
using System.IO;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using System.IO;
#endif

#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#else
using XRSettings = UnityEngine.VR.VRSettings;
#endif  // UNITY_2017_2_OR_NEWER

#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

// Notifies users if they build for Android or iOS without Cardboard or Daydream enabled.
class GvrBuildProcessor : IPreprocessBuild, IPostprocessBuild
{
    private const string VR_SETTINGS_NOT_ENABLED_ERROR_MESSAGE_FORMAT =
      "To use the Google VR SDK on {0}, 'Player Settings > Virtual Reality Supported' setting must be checked.\n" +
      "Please fix this setting and rebuild your app.";
    private const string IOS_MISSING_GVR_SDK_ERROR_MESSAGE =
      "To use the Google VR SDK on iOS, 'Player Settings > Virtual Reality SDKs' must include 'Cardboard'.\n" +
      "Please fix this setting and rebuild your app.";
    private const string ANDROID_MISSING_GVR_SDK_ERROR_MESSAGE =
      "To use the Google VR SDK on Android, 'Player Settings > Virtual Reality SDKs' must include 'Daydream' or 'Cardboard'.\n" +
      "Please fix this setting and rebuild your app.";

    public int callbackOrder
    {
        get { return 0; }
    }

#if UNITY_2018_1_OR_NEWER
  public void OnPreprocessBuild(BuildReport report)
  {
    OnPreprocessBuild(report.summary.platform, report.summary.outputPath);
  }
#endif

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.Android && target != BuildTarget.iOS)
        {
            // Do nothing when not building for Android or iOS.
            return;
        }

        //if (PlayerSettings.GetVirtualRealitySupported(BuildTargetGroup.Android))
        //{
        //    PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
        //}
        if (PlayerSettings.virtualRealitySupported)
        {
            PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
        }
        // 'Player Settings > Virtual Reality Supported' must be enabled.
        //if (!IsVRSupportEnabled())
        //{
        //    Debug.LogErrorFormat(VR_SETTINGS_NOT_ENABLED_ERROR_MESSAGE_FORMAT, target);
        //}

        //if (target == BuildTarget.Android)
        //{
        //    // When building for Android at least one VR SDK must be included.
        //    // For Google VR valid VR SDKs are 'Daydream' and/or 'Cardboard'.
        //    if (!IsSDKOtherThanNoneIncluded())
        //    {
        //        Debug.LogError(ANDROID_MISSING_GVR_SDK_ERROR_MESSAGE);
        //    }
        //}

        if (target == BuildTarget.iOS)
        {
            // When building for iOS at least one VR SDK must be included.
            // For Google VR only 'Cardboard' is supported.
            if (!IsSDKOtherThanNoneIncluded())
            {
                Debug.LogError(IOS_MISSING_GVR_SDK_ERROR_MESSAGE);
            }
        }

        if (target == BuildTarget.Android)
        {
            Debug.Log("PlayerSettings.SplashScreen.show:"+ PlayerSettings.SplashScreen.show);
           

            string configpath = Application.dataPath + "/Plugins/Android/assets/splash.cfg";
            if (!Directory.Exists(Application.dataPath + "/Plugins")) Directory.CreateDirectory(Application.dataPath + "/Plugins");
            if (!Directory.Exists(Application.dataPath + "/Plugins/Android")) Directory.CreateDirectory(Application.dataPath + "/Plugins/Android");
            if (!Directory.Exists(Application.dataPath + "/Plugins/Android/assets")) Directory.CreateDirectory(Application.dataPath + "/Plugins/Android/assets");
            if (File.Exists(configpath))
            {
                File.Delete(configpath);
            }
            StreamWriter sw = File.CreateText(configpath);
            if (PlayerSettings.SplashScreen.show)
            {
                sw.WriteLine("UNITY_USE_SPLASH=1");
            }
            else
            {
                sw.WriteLine("UNITY_USE_SPLASH=0");
            }
            
            sw.Close();

        }
    }
   
    private string GetResourcePath()
    {
        var monos = MonoScript.FromScriptableObject(ScriptableObject.CreateInstance(typeof(GvrEditorSettings)));
        var path = AssetDatabase.GetAssetPath(monos);
        path = path.Substring(0, path.LastIndexOf("/"));
        path = path.Substring(0, path.LastIndexOf("/"));
        path = path + "/Plugins/Android/assets/splash.cfg";
        
        return path;
    }
#if UNITY_2018_1_OR_NEWER
  public void OnPostprocessBuild(BuildReport report) {
    OnPostprocessBuild(report.summary.platform, report.summary.outputPath);
  }
#endif

    public void OnPostprocessBuild(BuildTarget target, string outputPath)
    {
#if UNITY_IOS
    // Add Camera usage description for scanning viewer QR codes on iOS.
    if (target == BuildTarget.iOS) {
      // Read plist
      var plistPath = Path.Combine(outputPath, "Info.plist");
      var plist = new PlistDocument();
      plist.ReadFromFile(plistPath);

      // Update value
      PlistElementDict rootDict = plist.root;
      rootDict.SetString("NSCameraUsageDescription", "Scan Cardboard viewer QR code");

      // Write plist
      File.WriteAllText(plistPath, plist.WriteToString());
    }
#endif
    }

    // 'Player Settings > Virtual Reality Supported' enabled?
    private bool IsVRSupportEnabled()
    {
        return PlayerSettings.virtualRealitySupported;
    }

    // 'Player Settings > Virtual Reality SDKs' includes any VR SDK other than 'None'?
    private bool IsSDKOtherThanNoneIncluded()
    {
        bool containsNone = XRSettings.supportedDevices.Contains(GvrSettings.VR_SDK_NONE);
        int numSdks = XRSettings.supportedDevices.Length;
        return containsNone ? numSdks > 1 : numSdks > 0;
    }
}
#endif  // UNITY_ANDROID || UNITY_IOS
