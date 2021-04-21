using System;
using Lance.Common;
using UnityEngine.SceneManagement;

namespace Lance.TowerWar.Manager
{
    public class Gamemanager : Singleton<Gamemanager>
    {
        #region properties
        

        #endregion
        
        #region unity-api

        private void Start() { SceneManager.LoadScene("menu"); }

        #endregion

        #region function



        #endregion
    }
}