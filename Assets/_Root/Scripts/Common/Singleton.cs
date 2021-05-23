using UnityEngine;

public class Singleton <T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;

    public static T Instance
    {
        get { return instance ?? (instance = FindObjectOfType<T>()); }
    }

    protected virtual void Awake()
    {
        instance = this as T;
    }

    protected virtual void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
