using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lance.TowerWar.Controller
{
    public class Launcher : MonoBehaviour
    {
        private void Start() { SceneManager.LoadScene("menu"); }
    }
}