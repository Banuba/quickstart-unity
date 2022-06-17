#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

class PreBuildManager : IPreprocessBuildWithReport
{
    public int callbackOrder
    {
        get {
            return 0;
        }
    }

    protected string streamingAssetsPath => Application.streamingAssetsPath + "/BanubaFaceAR/";

    protected BNB.ResourcesJSON files;

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        // Do the preprocessing here
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        var resourceDir = Application.dataPath + "/Resources";
        if (!Directory.Exists(resourceDir)) {
            Directory.CreateDirectory(resourceDir);
        }
        files = new BNB.ResourcesJSON();

        var file = resourceDir + "/BNBResourceList.json";
        ProcessDirectory(streamingAssetsPath);

        File.WriteAllText(file, files.SaveToString());
    }

    public void ProcessDirectory(string targetDirectory)
    {
        string[] fileEntries = Directory.GetFiles(targetDirectory);
        foreach (string fileName in fileEntries) {
            fileName.Replace(@"\\", @"/");
            fileName.Replace(@"\", @"/");
            ProcessFile(fileName);
        }

        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries) {
            ProcessDirectory(subdirectory);
        }
    }

    public void ProcessFile(string path)
    {
        if ((File.GetAttributes(path) & FileAttributes.Hidden) == FileAttributes.Hidden) {
            return;
        }
        if (Path.GetExtension(path) == ".meta") {
            return;
        }
        files.resources.Add(path.Substring(streamingAssetsPath.Length));
    }
}
#endif
