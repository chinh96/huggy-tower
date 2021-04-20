#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
// ReSharper disable once InconsistentNaming
public class LoadingDefineSymbols : AssetPostprocessor
{
    /// <summary>
    /// A list of all the symbols you want added to the build settings
    /// </summary>
    public static readonly string[] Symbols = {"LANCE_LOADING_SUPPORT"};

    /// <summary>
    /// As soon as this class has finished compiling, adds the specified define symbols to the build settings
    /// </summary>
    static LoadingDefineSymbols() { AddSymbols(); }

    private static void AddSymbols()
    {
        string scriptingDefinesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> scriptingDefinesStringList = scriptingDefinesString.Split(';').ToList();
        scriptingDefinesStringList.AddRange(Symbols.Except(scriptingDefinesStringList));
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", scriptingDefinesStringList.ToArray()));
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (var asset in importedAssets)
        {
            if (Path.GetFileNameWithoutExtension(asset).Equals(nameof(LoadingDefineSymbols)))
            {
                AddSymbols();
            }
        }
        
        foreach (var asset in deletedAssets)
        {
            if (Path.GetFileNameWithoutExtension(asset).Equals(nameof(LoadingDefineSymbols)))
            {
                string scriptingDefinesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                List<string> scriptingDefinesStringList = scriptingDefinesString.Split(';').ToList();

                foreach (var symbol in Symbols)
                {
                    scriptingDefinesStringList.Remove(symbol);
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                    string.Join(";", scriptingDefinesStringList.ToArray()));
            }
        }
    }
}

#endif