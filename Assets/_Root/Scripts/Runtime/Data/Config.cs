using System.Collections.Generic;
using UnityEngine;

public class Config : ScriptableObject
{
    private static Config instance;
    private static Config Instance => instance ? instance : (instance = Resources.Load<Config>(Constants.CONFIG));


    [SerializeField] private int maxLevelCanReach;
    [SerializeField] private int maxLevelWithOutTutorial;

    [Space] [SerializeField] private List<int> levelSkips;


    #region api

    public static int MaxLevelCanReach => Instance.maxLevelCanReach;
    public static int MaxLevelWithOutTutorial => Instance.maxLevelWithOutTutorial;
    public static List<int> LevelSkips => instance.levelSkips;

    #endregion
}

#if UNITY_EDITOR
public class ConfigEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // todo
    }
}
#endif