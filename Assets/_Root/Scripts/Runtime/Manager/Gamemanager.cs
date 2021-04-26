using Lance.Common;
using Lance.TowerWar.Data;
using Lance.TowerWar.LevelBase;
using Lance.TowerWar.Unit;
using UnityEngine;

namespace Lance.TowerWar.Controller
{
    public class Gamemanager : Singleton<Gamemanager>
    {
        [SerializeField] private LevelRoot root;

        private bool _isReplay;

        #region properties

        public LevelRoot Root => root;
        public EGameState GameState { get; set; }

        #endregion

        #region unity-api

        private void Start() { LoadLevel(Data.Data.UserCurrentLevel); }

        #endregion

        #region function

        private void ResetFlagNextLevel() { }

        /// <summary>
        /// load level to play
        /// </summary>
        /// <param name="levelIndex"></param>
        public async void LoadLevel(int levelIndex)
        {
            async void LoadNextLevel(int localLevelIndex)
            {
                var go = await DataBridge.Instance.GetLevel(localLevelIndex + 1);
                if (go.Item1 != null)
                {
                    DataBridge.Instance.NextLevelLoaded = go.Item1.GetComponent<LevelMap>();
                    DataBridge.Instance.NextLevelLoaded.SetLevelLoaded(go.Item2);
                }
            }

            void SavePreviousLevel(LevelMap localLevelMap)
            {
                DataBridge.Instance.PreviousLevelLoaded = localLevelMap;
                DataBridge.Instance.PreviousLevelLoaded.SetLevelLoaded(localLevelMap.CurrentLevelIndex);
            }

            ResetFlagNextLevel();
            LevelMap levelInstall = null;
            if (Instance._isReplay)
            {
                Instance._isReplay = false;

                if (DataBridge.Instance.PreviousLevelLoaded != null && DataBridge.Instance.PreviousLevelLoaded.CurrentLevelIndex == levelIndex)
                {
                    levelInstall = DataBridge.Instance.PreviousLevelLoaded;
                    LoadNextLevel(levelIndex);
                }
                else
                {
                    DataBridge.Instance.NextLevelLoaded = null;
                    var level = await DataBridge.Instance.GetLevel(levelIndex);
                    if (level.Item1 != null)
                    {
                        levelInstall = level.Item1.GetComponent<LevelMap>();
                        levelInstall.SetLevelLoaded(level.Item2);
                    }

                    LoadNextLevel(levelIndex);
                }
            }
            else
            {
                if (DataBridge.Instance.NextLevelLoaded != null && DataBridge.Instance.NextLevelLoaded.CurrentLevelIndex == levelIndex)
                {
                    levelInstall = DataBridge.Instance.NextLevelLoaded;
                    LoadNextLevel(levelIndex);
                }
                else
                {
                    DataBridge.Instance.NextLevelLoaded = null;
                    var level = await DataBridge.Instance.GetLevel(levelIndex);
                    if (level.Item1 != null)
                    {
                        levelInstall = level.Item1.GetComponent<LevelMap>();
                        levelInstall.SetLevelLoaded(level.Item2);
                    }

                    LoadNextLevel(levelIndex);
                }
            }

            if (levelInstall == null)
            {
                DataBridge.Instance.MakeCacheLevel();
                DataBridge.Instance.NextLevelLoaded = null;
                var level = await DataBridge.Instance.GetLevel(levelIndex);
                if (level.Item1 != null)
                {
                    levelInstall = level.Item1.GetComponent<LevelMap>();
                    levelInstall.SetLevelLoaded(level.Item2);
                }

                LoadNextLevel(levelIndex);
            }

            Root.Initialized(levelIndex, levelInstall);
            InternalPlayLevel();
            SavePreviousLevel(levelInstall);
        }


        private void InternalPlayLevel()
        {
            ResetFlagNextLevel();

            if (Root != null && Root.LevelMapPrefab)
            {
                Root.Install();
                Root.DarknessRise();
                Instance.GameState = EGameState.Playing;
                // analytic
            }
            else
            {
                Debug.LogError("Can not instantiate level!");
            }
        }

        #endregion
    }
}