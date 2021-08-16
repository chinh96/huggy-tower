#if UNITY_IOS && UNITY_EDITOR
using System;
using System.IO;

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class PListProcessor
{
    [PostProcessBuild]
    public static void OnPostProcessBuildAddFirebaseFile(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS) 
        {
            // Go get pbxproj file
            string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
    
            // PBXProject class represents a project build settings file,
            // here is how to read that in.
            PBXProject proj = new PBXProject ();
            proj.ReadFromFile (projPath);
    
            // Copy plist from the project folder to the build folder
            proj.AddFileToBuild (proj.GetUnityMainTargetGuid(), proj.AddFile("GoogleService-Info.plist", "GoogleService-Info.plist"));
    
            // Write PBXProject object back to the file
            proj.WriteToFile (projPath);
        }
    }
}

#endif