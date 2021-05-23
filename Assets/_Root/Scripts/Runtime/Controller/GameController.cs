using Lean.Pool;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
    [SerializeField] private LevelRoot root;
    [SerializeField] private RoomTower roomPrefab;
    [SerializeField] private TextMeshProUGUI txtQuest;

    private bool _isReplay;

    public LeanGameObjectPool poolArrow;
    public LevelRoot Root => root;
    public EGameState GameState { get; set; }
    public RoomTower RoomPrefab => roomPrefab;

    private void Start()
    {
        LoadLevel(Data.CurrentLevel);
    }

    private void ResetFlagNextLevel() { }

    public async void LoadLevel(int fakeIndex)
    {
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
            DataBridge.Instance.PreviousLevelLoaded = localLevelMap;
            DataBridge.Instance.PreviousLevelLoaded.SetLevelLoaded(localLevelMap.CurrentRealLevelIndex, localLevelMap.CurrentFakeLevelIndex);
        }

        ResetFlagNextLevel();
        LevelMap levelInstall = null;
        if (Instance._isReplay)
        {
            Instance._isReplay = false;

            Debug.Log(DataBridge.Instance);
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
            Debug.Log(DataBridge.Instance);
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
        UpdateDislayCurrentLevel(fakeIndex, levelInstall.condition);
        InternalPlayLevel();
        SavePreviousLevel(levelInstall);
    }

    public void UpdateDislayCurrentLevel(int level, ELevelCondition condition)
    {
        var str = "";
        switch (condition)
        {
            case ELevelCondition.KillAll:
                str = "kill all enemy";
                break;
            case ELevelCondition.CollectChest:
                str = "open chest";
                break;
            case ELevelCondition.SavePrincess:
                str = "save the princess";
                break;
            case ELevelCondition.CollectGold:
                str = "collect golds";
                break;
        }

        txtQuest.text = $"Level {level + 1}: {str.ToUpper()}";
    }

    private void InternalPlayLevel()
    {
        ResetFlagNextLevel();

        if (Root != null && Root.LevelMapPrefab)
        {
            Root.Install();
            Root.DarknessRise();
            Instance.GameState = EGameState.Playing;
        }
        else
        {
            Debug.LogError("Can not instantiate level!");
        }
    }

    public void OnNextLevel()
    {
        Instance.root.Clear();
        Instance.LoadLevel(Data.CurrentLevel);
    }

    public void OnReplayLevel()
    {
        _isReplay = true;

        Instance.LoadLevel(Data.CurrentLevel);
    }

    public void OnSkipLevel()
    {
        Data.CurrentLevel++;
        OnNextLevel();
    }

    public void OnBackToHome()
    {
        SceneManager.LoadScene(Constants.HOME_SCENE);
    }

    public void OnWinLevel()
    {
        Data.CurrentLevel++;
        Data.CountPlayLevel++;
        if (Data.MaxLevel < Data.CurrentLevel) Data.MaxLevel = Data.CurrentLevel;

        GameState = EGameState.Win;
        ShowPopupWin();
    }

    public void OnLoseLevel()
    {
        GameState = EGameState.Lose;
        ShowPopupLose();
    }

    private void ShowPopupWin()
    {
        PopupController.Instance.Show<WinPopup>();
    }

    private void ShowPopupLose()
    {
        PopupController.Instance.Show<LosePopup>();
    }
}