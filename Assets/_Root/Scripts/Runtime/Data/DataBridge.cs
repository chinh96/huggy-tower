using System;
using Lance.Common;
using Lance.TowerWar.LevelBase;
using UnityEngine;

namespace Lance.TowerWar.Data
{
    public class DataBridge : Singleton<DataBridge>
    {
        #region properties

        [SerializeField, ReadOnly] private LevelMap previousLevelLoaded = null;
        [SerializeField, ReadOnly] private LevelMap nextLevelLoaded = null;
        [SerializeField, ReadOnly] private bool isReplay;

        public LevelMap PreviousLevelLoaded => previousLevelLoaded;
        public LevelMap NextLevelLoaded => nextLevelLoaded;
        public bool IsReplay => isReplay;
        private int[] _cacheLevels;

        #endregion

        #region unity-api

        private void Start()
        {
            
        }

        #endregion
        
        #region function

        private void CheckCacheLevel()
        {
            bool flagOutLevel = false;
        }

        private void MakeCacheLevel()
        {
            
        }
        
        
        
        #endregion
    }
}