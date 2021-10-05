using Lean.Pool;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Lean.Touch;
using Spine.Unity;
using I2.Loc;

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
    [SerializeField] private List<GameObject> backgroundsNormal;
    [SerializeField] private List<GameObject> backgroundsHalloween;
    [SerializeField] private List<MoveOutAnimation> moveOutAnimations;
    private FighterOverlay fighterOverlay;
    [SerializeField] private GameObject opacity;
    public LeanTouch LeanTouch;
    [SerializeField] private GameObject rescuePartyButton;

    private Player player;
    public Player Player => player ? player : player = FindObjectOfType<Player>();

    private ItemLock itemLock;
    public ItemLock ItemLock => itemLock ? itemLock : itemLock = FindObjectOfType<ItemLock>();

    private Vector3 zoomCameraPositionOrigin;
    private float zoomOrthoSizeOrigin;
    private bool _isReplay;
    private bool isSlice;
    private bool isZoomIn;

    public LeanGameObjectPool poolArrow;
    public LevelRoot Root => root;
    public EGameState GameState;
    public RoomTower RoomPrefab => roomPrefab;

    [NonSerialized] public bool IsOnboarding;
    [NonSerialized] public bool IsJapanBackground;
    [NonSerialized] public bool IsSeaBackground;
    [NonSerialized] public bool IsHalloweenBackground;
    [NonSerialized] public Vector3 positionCameraOrigin;
    [NonSerialized] public List<GameObject> Kraken0s = new List<GameObject>();

    public void RemoveKraken0()
    {
        Kraken0s[0].GetComponent<SkeletonGraphic>().Play("Die", false);
        Kraken0s.RemoveAt(0);
    }

    public void SetEnableLeanTouch(bool enable)
    {
        LeanTouch.enabled = enable;
    }

    protected override void Awake()
    {
        base.Awake();

        overlay.DOFade(1, 0);
        SetEnableLeanTouch(false);
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
        positionCameraOrigin = Camera.main.transform.position;
        LoadLevel(Data.CurrentLevel);
        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.LogIntoTheGame);
        rescuePartyButton.SetActive(Data.TimeToRescueParty.TotalMilliseconds > 0);
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
        List<GameObject> backgrounds = backgroundsNormal;

        if (Data.TimeToRescueParty.TotalMilliseconds > 0)
        {
            backgrounds = backgroundsHalloween;
        }

        backgrounds.ForEach(item => item.SetActive(false));
        int random = UnityEngine.Random.Range(0, backgrounds.Count);
        backgrounds[random].SetActive(true);

        IsJapanBackground = backgrounds[random].name == "Jav";
        IsSeaBackground = backgrounds[random].name == "Sea";
        IsHalloweenBackground = Data.TimeToRescueParty.TotalMilliseconds > 0;
    }

    public async void LoadLevel(int fakeIndex)
    {
        if (fighterOverlay != null)
        {
            Destroy(fighterOverlay.gameObject);
        }

        if (fakeIndex > 0)
        {
            IsOnboarding = false;
        }
        AdController.Instance.JustShowReward = false;
        AdController.Instance.Request();

        MoveInAnim();

        SoundController.Instance.PlayOnce(SoundType.EnemyStart);

        FadeOutOverlay(() =>
        {
            if (Root.LevelMap != null && Root.LevelMap.DurationMoveCamera == 0 && !Root.LevelMap.hasNewVisitTower)
            {
                SetEnableLeanTouch(true);
            }
        });
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


        switch (Data.CurrentLevel)
        {
            case 0:
                if (!Data.FlagPlayLevel1)
                {
                    AnalyticController.StartLevel1Funnel();
                    AnalyticController.Level1StartFunnel();
                    AnalyticController.AdjustLogEventPlayLevel1();
                }

                break;
            case 1:
                if (!Data.FlagPlayLevel2)
                {
                    AnalyticController.AdjustLogEventPlayLevel2();
                }

                break;
            case 2:
                if (!Data.FlagPlayLevel3)
                {
                }

                AnalyticController.AdjustLogEventPlayLevel3();
                break;
            case 3:
                if (!Data.FlagPlayLevel4)
                {
                    AnalyticController.AdjustLogEventPlayLevel4();
                }

                break;
            case 4:
                if (!Data.FlagPlayLevel5)
                {
                    AnalyticController.AdjustLogEventPlayLevel5();
                }

                break;
            case 5:
                if (!Data.FlagPlayLevel6)
                {
                    AnalyticController.AdjustLogEventPlayLevel6();
                }

                break;
            case 6:
                if (!Data.FlagPlayLevel7)
                {
                    AnalyticController.AdjustLogEventPlayLevel7();
                }

                break;
            case 7:
                if (!Data.FlagPlayLevel8)
                {
                    AnalyticController.StartLevel8Funnel();
                    AnalyticController.AdjustLogEventPlayLevel8();
                }

                break;
            case 8:
                if (!Data.FlagPlayLevel9)
                {
                    AnalyticController.AdjustLogEventPlayLevel9();
                }

                break;
            case 9:
                if (!Data.FlagPlayLevel10)
                {
                    AnalyticController.Level10StartFunnel();
                    AnalyticController.AdjustLogEventPlayLevel10();
                }

                break;
            case 14:
                if (!Data.FlagPlayLevel15)
                {
                    AnalyticController.Level15StartFunnel();
                }
                break;
            case 19:
                if (!Data.FlagPlayLevel20)
                {
                    AnalyticController.Level20StartFunnel();
                }
                break;
        }

        opacity.SetActive(Root.LevelMap.DurationMoveCamera > 0);
    }

    public void UpdateDislayCurrentLevel(int level, ELevelCondition condition)
    {
        var data = ResourcesController.Quest.GetQuestByCondition(condition);

        //txtQuest.text = $"Level {level + 1}: {data.Quest}";
        txtQuest.GetComponent<Localize>().SetTerm("QuestInGame_txt" + data.Condition + "Type");
        txtQuest.GetComponent<LocalizationParamsManager>().SetParameterValue("VALUE", $"{level + 1}", true);
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
        SetEnableLeanTouch(false);

        PopupController.Instance.DismissAll();
        FadeInOverlay(() =>
        {
            AdController.Instance.ShowInterstitial(() =>
            {
                Instance.root.Clear();
                Camera.main.transform.position = positionCameraOrigin;
                Instance.LoadLevel(Data.CurrentLevel);
            });
        });
    }

    public void OnReplayLevel()
    {
        SetEnableLeanTouch(false);

        ResourcesController.Achievement.ResetNumberTemp();
        ResourcesController.DailyQuest.ResetNumberTemp();

        PopupController.Instance.DismissAll();
        FadeInOverlay(() =>
        {
            DOTween.KillAll();
            _isReplay = true;
            Camera.main.transform.position = positionCameraOrigin;
            Instance.LoadLevel(Data.CurrentLevel);
        });
    }

    public void OnSkipLevel()
    {
        OnSkipLevel(null);
    }

    public void OnSkipLevel(Action onAdCompleted)
    {
        AnalyticController.SkipLevel();

        PopupController.Instance.DismissAll();
        DOTween.KillAll();
        AdController.Instance.ShowRewardedAd(() =>
        {
            Data.CurrentLevel++;
            OnNextLevel();
            onAdCompleted?.Invoke();
        });
    }

    public void OnBackToHome()
    {
        FadeInOverlay(() =>
        {
            PopupController.Instance.DismissAll();
            DOTween.KillAll();
            SceneManager.LoadScene(Constants.HOME_SCENE);
        });
    }

    public void OnWinLevel()
    {
        AnalyticController.CompleteLevel();

        switch (Data.CurrentLevel)
        {
            case 0:
                if (!Data.FlagPlayLevel1)
                {
                    AnalyticController.CompleteLevel1Funnel();
                    AnalyticController.Level1CompleteFunnel();
                    Data.FlagPlayLevel1 = true;
                }

                break;
            case 1:
                if (!Data.FlagPlayLevel2)
                {
                    Data.FlagPlayLevel2 = true;
                }

                break;
            case 2:
                if (!Data.FlagPlayLevel3)
                {
                    Data.FlagPlayLevel3 = true;
                }

                break;
            case 3:
                if (!Data.FlagPlayLevel4)
                {
                    Data.FlagPlayLevel4 = true;
                }

                break;
            case 4:
                if (!Data.FlagPlayLevel5)
                {
                    Data.FlagPlayLevel5 = true;
                }

                break;
            case 5:
                if (!Data.FlagPlayLevel6)
                {
                    Data.FlagPlayLevel6 = true;
                }

                break;
            case 6:
                if (!Data.FlagPlayLevel7)
                {
                    Data.FlagPlayLevel7 = true;
                }
                break;
            case 7:
                if (!Data.FlagPlayLevel8)
                {
                    Data.FlagPlayLevel8 = true;
                }

                break;
            case 8:
                if (!Data.FlagPlayLevel9)
                {
                    Data.FlagPlayLevel9 = true;
                }

                break;
            case 9:
                if (!Data.FlagPlayLevel10)
                {
                    AnalyticController.Level10CompleteFunnel();
                    Data.FlagPlayLevel10 = true;
                }

                break;
            case 14:
                if (!Data.FlagPlayLevel15)
                {
                    AnalyticController.Level15CompleteFunnel();
                    Data.FlagPlayLevel15 = true;
                }

                break;
            case 19:
                if (!Data.FlagPlayLevel20)
                {
                    AnalyticController.Level20CompleteFunnel();
                    Data.FlagPlayLevel20 = true;
                }

                break;
        }


        root.LevelMap.visitTower.ChangeToHomeTower();

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

        Vector2 endValue = new Vector2(Player.transform.position.x, Player.transform.position.y) + zoomOffset;
        if (Player.IsDie3)
        {
            endValue -= new Vector2(1f, 0);
        }
        Camera.main.transform.DOMove(endValue, zoomCameraDuration);

        zoomOrthoSizeOrigin = Camera.main.orthographicSize;
        Camera.main.DOOrthoSize(zoomOrthoSize, zoomCameraDuration);

        isZoomIn = true;
    }

    private void ZoomOutCamera()
    {
        if (isZoomIn)
        {
            Camera.main.orthographicSize = zoomOrthoSizeOrigin;

            isZoomIn = false;
        }
    }

    private void FadeInOverlay(Action action = null)
    {
        if (ResourcesController.Config.EnableTest)
        {
            action?.Invoke();
        }
        else
        {
            overlay.gameObject.SetActive(true);
            overlay.DOFade(1, .3f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                action?.Invoke();
            });
        }
    }

    private void FadeOutOverlay(Action action)
    {
        if (ResourcesController.Config.EnableTest)
        {
            overlay.gameObject.SetActive(false);
            NotiQuestController.Instance.Show();
        }
        else
        {
            overlay.DOFade(0, 1f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                overlay.gameObject.SetActive(false);
                NotiQuestController.Instance.Show();
                action?.Invoke();
            });
        }
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

    public void ShowFighterOverlay()
    {
        fighterOverlay = Instantiate(ResourcesController.Config.FighterOverlay, Root.transform.parent);
    }
}