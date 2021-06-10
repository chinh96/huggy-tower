#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;


[CustomEditor(typeof(LevelMap))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);
        var prefabUtility = PrefabStageUtility.GetCurrentPrefabStage();
        if (GUILayout.Button("PLAY", GUILayout.Height(45)))
        {
            var levelAsset = AssetDatabase.LoadAssetAtPath<LevelMap>(prefabUtility.prefabAssetPath);
            Resources.Load<ConfigResources>("ConfigResources").LevelDebug = levelAsset;

            EditorSceneManager.OpenScene("Assets/_Root/Scenes/LoadingScene.unity");
            EditorApplication.isPlaying = true;
        }
    }
}
#endif