using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class VisitTower : Tower
{
    static VisitTower instance;

    public static VisitTower Instance
    {
        get { return instance ?? (instance = FindObjectOfType<VisitTower>()); }
    }

}