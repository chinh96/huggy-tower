using Lance.Common.Loading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Lance.TowerWar.Controller
{
    public class Launcher : MonoBehaviour
    {
        public LoadingScreenManager loading;

        private void Start()
        {
            Addressables.InitializeAsync();
            loading.LoadScene("menu");
        }
    }
}