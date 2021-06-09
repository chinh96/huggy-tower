using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "QuestResources", menuName = "ScriptableObjects/QuestResources")]
public class QuestResources : ScriptableObject
{
    public List<QuestData> questDatas;
}

[Serializable]
public class QuestData
{
    ELevelCondition condition;
    string quest;
}

public enum ELevelCondition
{
    KillAll = 0,
    CollectChest = 1,
    SavePrincess = 2,
    CollectGold = 3,
}