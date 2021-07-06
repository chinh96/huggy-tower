using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldResources", menuName = "ScriptableObjects/WorldResources")]
public class WorldResources : ScriptableObject
{
    public WorldType WorldType;
    public int LevelUnlock;
    public List<CastleResources> Castles;
    public bool IsUnlocked => Data.CurrentLevel >= LevelUnlock;
    public Sprite background;
    public bool IsComplete => Castles.TrueForAll(item => item.IsComplete);
}