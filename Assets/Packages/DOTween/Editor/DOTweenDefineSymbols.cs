#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
// ReSharper disable once InconsistentNaming
public class DOTweenDefineSymbols : AssetPostprocessor
{
    /// <summary>
    /// A list of all the symbols you want added to the build settings
    /// </summary>
    public static readonly string[] Symbols = {"LANCE_DOTWEEN_SUPPORT"};

    /// <summary>
    /// As soon as this class has finished compiling, adds the specified define symbols to the build settings
    /// </summary>
    static DOTweenDefineSymbols() { AddSymbols(); }

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
            if (Path.GetFileNameWithoutExtension(asset).Equals(nameof(DOTweenDefineSymbols)))
            {
                AddSymbols();
            }
        }
        
        foreach (var asset in deletedAssets)
        {
            if (Path.GetFileNameWithoutExtension(asset).Equals(nameof(DOTweenDefineSymbols)))
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