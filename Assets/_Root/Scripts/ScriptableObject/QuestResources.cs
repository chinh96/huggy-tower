using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "QuestResources", menuName = "ScriptableObjects/QuestResources")]
public class QuestResources : ScriptableObject
{
    public List<QuestData> QuestDatas;

    public QuestData GetQuestByCondition(ELevelCondition condition) => QuestDatas.Find(item => item.Condition == condition);
}

[Serializable]
public class QuestData
{
    public ELevelCondition Condition;
    public string Quest;
    public Sprite Sprite;
}