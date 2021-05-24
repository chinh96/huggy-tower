using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesController : Singleton<ResourcesController>
{
    public SkinResources Hero;
    public SkinResources Princess;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        Hero.SkinDefault.IsUnlocked = true;
    }
}
