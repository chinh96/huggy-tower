using Lance.Common;
using Lance.TowerWar.Data;
using Lance.TowerWar.LevelBase;
using Lance.TowerWar.UI;
using Lance.TowerWar.Unit;
using UnityEngine;

namespace Lance.TowerWar.Controller
{
    public class Gamemanager : Singleton<Gamemanager>
    {
        [SerializeField] private LevelRoot root;
        [SerializeField] private RoomTower roomPrefab;

        [SerializeField] private HubGameplay hub;

        private bool _isReplay;

        #region properties

        public LevelRoot Root => root;
        public EGameState GameState { get; set; }
        public RoomTower RoomPrefab => roomPrefab;

        #endregion

        #region unity-api

        private void Start() { LoadLevel(Data.Data.UserCurrentLevel); }

        #endregion

        #region function

        private void ResetFlagNextLevel() { }

        /// <summary>
        /// load level to play
        /// </summary>
        /// <param name="fakeIndex"></param>
        public async void LoadLevel(int fakeIndex)
        {
            Debug.Log("Load fakeIndex :" + fakeIndex);

            async void LoadNextLevel(int fakeLevelIndex)
            {
                var go = await DataBridge.Instance.GetLevel(fakeLevelIndex + 1);
                if (go.Item1 != null)
                {
                    DataBridge.Instance.NextLevelLoaded = go.Item1.GetComponent<LevelMap>();
                    DataBridge.Instance.NextLevelLoaded.SetLevelLoaded(go.Item2, fakeLevelIndex + 1);
                }
            }

            void SavePreviousLevel(LevelMap localLevelMap)
            {
                Debug.Log("previous real level index :" + localLevelMap.CurrentRealLevelIndex);
                Debug.Log("previous fake level index :" + localLevelMap.CurrentFakeLevelIndex);
                DataBridge.Instance.PreviousLevelLoaded = localLevelMap;
                DataBridge.Instance.PreviousLevelLoaded.SetLevelLoaded(localLevelMap.CurrentRealLevelIndex, localLevelMap.CurrentFakeLevelIndex);
            }

            ResetFlagNextLevel();
            LevelMap levelInstall = null;
            if (Instance._isReplay)
            {
                Instance._isReplay = false;

                if (DataBridge.Instance.PreviousLevelLoaded != null && DataBridge.Instance.PreviousLevelLoaded.CurrentFakeLevelIndex == fakeIndex)
                {
                    levelInstall = DataBridge.Instance.PreviousLevelLoaded;
                    LoadNextLevel(fakeIndex);
                }
                else
                {
                    DataBridge.Instance.NextLevelLoaded = null;
                    var level = await DataBridge.Instance.GetLevel(fakeIndex);
                    if (level.Item1 != null)
                    {
                        levelInstall = level.Item1.GetComponent<LevelMap>();
                        levelInstall.SetLevelLoaded(level.Item2, fakeIndex);
                    }

                    LoadNextLevel(fakeIndex);
                }
            }
            else
            {
                if (DataBridge.Instance.NextLevelLoaded != null && DataBridge.Instance.NextLevelLoaded.CurrentFakeLevelIndex == fakeIndex)
                {
                    levelInstall = DataBridge.Instance.NextLevelLoaded;
                    LoadNextLevel(fakeIndex);
                }
                else
                {
                    DataBridge.Instance.NextLevelLoaded = null;
                    var level = await DataBridge.Instance.GetLevel(fakeIndex);
                    if (level.Item1 != null)
                    {
                        levelInstall = level.Item1.GetComponent<LevelMap>();
                        levelInstall.SetLevelLoaded(level.Item2, fakeIndex);
                    }

                    LoadNextLevel(fakeIndex);
                }
            }

            if (levelInstall == null)
            {
                DataBridge.Instance.MakeCacheLevel();
                DataBridge.Instance.NextLevelLoaded = null;
                var level = await DataBridge.Instance.GetLevel(fakeIndex);
                if (level.Item1 != null)
                {
                    levelInstall = level.Item1.GetComponent<LevelMap>();
                    levelInstall.SetLevelLoaded(level.Item2, fakeIndex);
                }

                LoadNextLevel(fakeIndex);
            }

            Root.Initialized(fakeIndex, levelInstall);
            hub.UpdateDislayCurrentLevel(fakeIndex);
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

        private void OnNextLevel()
        {
            Instance.root.Clear();
            Instance.LoadLevel(Data.Data.UserCurrentLevel);
        }

        private void OnReplayLevel()
        {
            _isReplay = true;


            Debug.Log("USER CURRENT LEVEL: " + Data.Data.UserCurrentLevel);
            Instance.LoadLevel(Data.Data.UserCurrentLevel);
        }

        private void OnSkipLevel() { }

        #endregion

        #region show-popup

        public void OnWinLevel()
        {
            Data.Data.UserCurrentLevel++;
            Data.Data.CountPlayLevel++;
            if (Data.Data.UserMaxLevel < Data.Data.UserCurrentLevel) Data.Data.UserMaxLevel = Data.Data.UserCurrentLevel;

            GameState = EGameState.Win;
            ShowPopupWin();
        }

        public void OnLoseLevel()
        {
            GameState = EGameState.Lose;
            ShowPopupLose();
        }

        public void ShowPopupWin() { GamePopup.Instance.ShowPopupWin(OnNextLevel, "You Win"); }

        public void ShowPopupLose() { GamePopup.Instance.ShowPopupLose(OnReplayLevel, OnSkipLevel, "You Lose"); }

        #endregion
    }
}