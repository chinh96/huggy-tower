using Lean.Pool;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GameController : Singleton<GameController>
{
    [SerializeField] private LevelRoot root;
    [SerializeField] private RoomTower roomPrefab;
    [SerializeField] private TextMeshProUGUI txtQuest;
    [SerializeField] private Image imgQuest;
    [SerializeField] private float zoomCameraDuration;
    [SerializeField] private float zoomOrthoSize;
    [SerializeField] private Vector2 zoomOffset;
    [SerializeField] private ParticleSystem firePaper;
    [SerializeField] private float delayWinLose = 2;
    [SerializeField] private GameObject slicer;
    [SerializeField] private Image overlay;
    [SerializeField] private List<GameObject> backgrounds;
    [SerializeField] private List<MoveOutAnimation> moveOutAnimations;

    private Player player;
    public Player Player => player ? player : player = FindObjectOfType<Player>();

    private Vector3 zoomCameraPositionOrigin;
    private float zoomOrthoSizeOrigin;
    private bool _isReplay;
    private bool isSlice;
    private bool isZoomIn;

    public LeanGameObjectPool poolArrow;
    public LevelRoot Root => root;
    public EGameState GameState { get; set; }
    public RoomTower RoomPrefab => roomPrefab;
    public bool IsOnboarding;

    protected override void Awake()
    {
        base.Awake();

        overlay.DOFade(1, 0);
    }

    private void Update()
    {
        if (isSlice)
        {
            slicer.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public void SetSlicerActive(bool active)
    {
        isSlice = active;
        slicer.SetActive(active);
    }

    private void Start()
    {
        MoveInAnim();
        SoundController.Instance.PlayBackground(SoundType.BackgroundInGame);
        CheckRadioCamera();
        LoadLevel(Data.CurrentLevel);
    }

    private void CheckRadioCamera()
    {
        float ratio = ((Screen.height / Screen.width) - (1920 / 1080f)) * 12;
        if (ratio > 0)
        {
            Camera.main.transform.position += new Vector3(0, ratio, 0);
            Camera.main.orthographicSize += ratio;
        }
    }

    private void ResetFlagNextLevel() { }

    private void LoadBackground()
    {
        backgrounds.ForEach(item => item.SetActive(false));
        int random = UnityEngine.Random.Range(0, backgrounds.Count);
        backgrounds[random].SetActive(true);
    }

    public async void LoadLevel(int fakeIndex)
    {
        AdController.Instance.Request();

        MoveInAnim();

        SoundController.Instance.PlayOnce(SoundType.EnemyStart);

        FadeOutOverlay();
        ZoomOutCamera();

        firePaper.gameObject.SetActive(false);

        LoadBackground();

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
        UpdateDislayCurrentLevel(fakeIndex, levelInstall.condition);
        InternalPlayLevel();
        SavePreviousLevel(levelInstall);
        AnalyticController.StartLevel();
    }

    public void UpdateDislayCurrentLevel(int level, ELevelCondition condition)
    {
        Debug.Log(level);
        var data = ResourcesController.Quest.GetQuestByCondition(condition);

        txtQuest.text = $"Level {level + 1}: {data.Quest}";
        imgQuest.sprite = data.Sprite;
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
        PopupController.Instance.DismissAll();
        FadeInOverlay(() =>
        {
            AdController.Instance.ShowInterstitial(() =>
            {
                Instance.root.Clear();
                Instance.LoadLevel(Data.CurrentLevel);
            });
        });
    }

    public void OnReplayLevel()
    {
        PopupController.Instance.DismissAll();
        FadeInOverlay(() =>
        {
            DOTween.KillAll();
            _isReplay = true;

            Instance.LoadLevel(Data.CurrentLevel);
        });
    }

    public void OnSkipLevel()
    {
        AnalyticController.SkipLevel();

        PopupController.Instance.DismissAll();
        DOTween.KillAll();
        AdController.Instance.ShowRewardedAd(() =>
        {
            Data.CurrentLevel++;
            OnNextLevel();
        });
    }

    public void OnBackToHome()
    {
        FadeInOverlay(() =>
        {
            PopupController.Instance.DismissAll();
            SceneManager.LoadScene(Constants.HOME_SCENE);
        });
    }

    public void OnWinLevel()
    {
        AnalyticController.CompleteLevel();

        root.LevelMap.visitTower.ChangeToHomTower();

        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.LevelPassed);

        MoveOutAnim();

        firePaper.gameObject.SetActive(true);

        Data.CurrentLevel++;
        Data.CountPlayLevel++;
        if (Data.MaxLevel < Data.CurrentLevel) Data.MaxLevel = Data.CurrentLevel;

        GameState = EGameState.Win;
        SoundController.Instance.PlayOnce(SoundType.Win);

        DOTween.Sequence().AppendInterval(delayWinLose).AppendCallback(() =>
        {
            ShowPopupWin();
            if (Data.CurrentLevel == ResourcesController.Config.LevelShowRate)
            {
                PopupController.Instance.Show<RatingPopup>(null, ShowAction.DoNothing);
            }
        });
    }

    public void OnLoseLevel()
    {
        root.LevelMap.homeTower.RemoveAll(() =>
        {
            AnalyticController.FailLevel();

            MoveOutAnim();

            GameState = EGameState.Lose;
            SoundController.Instance.PlayOnce(SoundType.Lose);

            DOTween.Sequence().AppendInterval(delayWinLose / 2).AppendCallback(() =>
            {
                ShowPopupLose();
            });
        });
    }

    private void ShowPopupWin()
    {
        ZoomInCamera();
        PopupController.Instance.Show<WinPopup>();
    }

    private void ShowPopupLose()
    {
        ZoomInCamera();
        PopupController.Instance.Show<LosePopup>();
    }

    private void ZoomInCamera()
    {
        Player.TxtDamage.gameObject.SetActive(false);

        zoomCameraPositionOrigin = Camera.main.transform.position;
        Vector2 endValue = new Vector2(Player.transform.position.x, Player.transform.position.y) + zoomOffset;
        Camera.main.transform.DOMove(endValue, zoomCameraDuration);

        zoomOrthoSizeOrigin = Camera.main.orthographicSize;
        Camera.main.DOOrthoSize(zoomOrthoSize, zoomCameraDuration);

        isZoomIn = true;
    }

    private void ZoomOutCamera()
    {
        if (isZoomIn)
        {
            Camera.main.transform.position = zoomCameraPositionOrigin;
            Camera.main.orthographicSize = zoomOrthoSizeOrigin;

            isZoomIn = false;
        }
    }

    private void FadeInOverlay(Action action = null)
    {
        overlay.gameObject.SetActive(true);
        overlay.DOFade(1, .3f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            action?.Invoke();
        });
    }

    private void FadeOutOverlay()
    {
        overlay.DOFade(0, 1f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            overlay.gameObject.SetActive(false);
        });
    }

    public void OnClickCastleButton()
    {
        PopupController.Instance.Show<WorldPopup>();
    }

    public void MoveOutAnim()
    {
        moveOutAnimations.ForEach(item => item.Play());
    }

    public void MoveInAnim()
    {
        moveOutAnimations.ForEach(item => item.Reset());
    }
}