using Lance.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lance.TowerWar.Controller
{
    public class Gamemanager : Singleton<Gamemanager>
    {
        #region properties
        

        #endregion
        
        #region unity-api

        private void Start()
        {
            LoadLevel();
        }

        #endregion

        #region function

        public void LoadLevel()
        {
            Debug.Log("LoadLevel");
        }


        #endregion
    }
}