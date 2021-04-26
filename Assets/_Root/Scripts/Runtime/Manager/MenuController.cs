using System;
using Lance.Common;
using UnityEngine.SceneManagement;

namespace Lance.TowerWar.Controller
{
    public class MenuController : Singleton<MenuController>
    {
        private void Start()
        {
            SceneManager.LoadScene("gameplay");
        }
    }
}