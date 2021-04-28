namespace Lance.TowerWar
{
    using UnityEngine;

    public class DontDestroyObject : MonoBehaviour
    {
        private void Awake() { DontDestroyOnLoad(gameObject); }
    }

}