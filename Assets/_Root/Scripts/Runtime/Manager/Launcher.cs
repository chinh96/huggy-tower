using Lance.Common.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lance.TowerWar.Controller
{
    public class Launcher : MonoBehaviour
    {
        public LoadingScreenManager loading;
        private void Start() { loading.LoadScene("menu"); }
    }
}