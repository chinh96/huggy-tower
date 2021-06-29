using UnityEditor;
using UnityEngine;

public class VisitTower : Tower
{
    static VisitTower instance;

    public static VisitTower Instance
    {
        get { return instance ?? (instance = FindObjectOfType<VisitTower>()); }
    }
}