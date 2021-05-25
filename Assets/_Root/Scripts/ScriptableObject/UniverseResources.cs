using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UniverseResources", menuName = "ScriptableObjects/UniverseResources")]
public class UniverseResources : ScriptableObject
{
    private static UniverseResources instance;
    public static UniverseResources Instance => instance ? instance : instance = Resources.Load<UniverseResources>("UniverseResources");

    public List<WorldResources> Worlds;

    public WorldResources WorldCurrent => Worlds.Find(item => item.WorldType == Data.WorldCurrent);
}
