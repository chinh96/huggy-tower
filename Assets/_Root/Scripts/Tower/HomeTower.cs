using UnityEngine;

public class HomeTower : Tower
{
    static HomeTower instance;

    public static HomeTower Instance
    {
        get { return instance ?? (instance = FindObjectOfType<HomeTower>()); }
    }
}