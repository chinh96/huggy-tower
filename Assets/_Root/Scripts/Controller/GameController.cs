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
    [SerializeField] private Camera UICamera;
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
    [SerializeField] private LevelText levelText;

    [SerializeField] private GameObject backgroundBoss;
    [SerializeField] private GameObject bloodVsBoss;
    [SerializeField] private Image bossFaceBlood;
    [SerializeField] private GameObject skipButton;
    public GameObject BloodVsBoss
    {
        get { return bloodVsBoss; }
        set { bloodVsBoss = value; }
    }
    private Player player;
    public Player Player => player ? player : player = FindObjectOfType<Player>();
    public Princess Princess => FindObjectOfType<Princess>();
    public GameObject Boss()
    {
        foreach(Unit obj in FindObjectsOfType<Unit>())
        {
            if(obj.Type == EUnitType.Boss)
            {
                return obj.gameObject;
            }
        }
        return null;
    } 
    private ItemLock itemLock;
    public ItemLock ItemLock => itemLock ? itemLock : itemLock = FindObjectOfType<ItemLock>();

    private Vector3 zoomCameraPositionOrigin;
    private float zoomOrthoSizeOrigin;
    private bool _isReplay;
    private bool isSlice;
    private bool isZoomIn;
    private Sequence sequence;

    public LeanGameObjectPool poolArrow;
    public LevelRoot Root => root;
    public EGameState GameState;
    public RoomTower RoomPrefab => roomPrefab;

    [NonSerialized] public bool IsOnboarding;
    [NonSerialized] public bool IsJapanBackground;
    [NonSerialized] public bool IsSeaBackground;
    [NonSerialized] public bool IsHalloweenBackground;
    [NonSerialized] public Vector3 positionCameraOrigin;
    private float cameraSizeOrigin;
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
        AdController.Instance.ShowBanner();
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
        // ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.LogIntoTheGame);
        rescuePartyButton.SetActive(Data.TimeToRescueParty.TotalMilliseconds > 0);
    }

    private void CheckRadioCamera()
    {
        float ratio = ((Screen.height / Screen.width) - (1920 / 1080f)) * 12;
        if (ratio > 0)
        {
            Camera.main.transform.position += new Vector3(0, ratio, 0);
            Camera.main.orthographicSize += ratio;

            UICamera.orthographicSize = Camera.main.orthographicSize;
            UICamera.transform.position = Camera.main.transform.position;
        }
    }

    private void ResetFlagNextLevel() { }

    private void LoadBackground()
    {
        List<GameObject> backgrounds = backgroundsNormal;

        //if (Data.TimeToRescueParty.TotalMilliseconds > 0)
        //{
        //    backgrounds = backgroundsHalloween;
        //}
        backgroundBoss.SetActive(false);

        backgrounds.ForEach(item => item.SetActive(false));
        int random = UnityEngine.Random.Range(0, backgrounds.Count);
        backgrounds[random].SetActive(true);

        //IsJapanBackground = backgrounds[random].name == "Jav";
        //IsSeaBackground = backgrounds[random].name == "Sea";
        //IsHalloweenBackground = Data.TimeToRescueParty.TotalMilliseconds > 0 && UnityEngine.Random.Range(0, 2) == 0;


    }

    public async void LoadLevel(int fakeIndex)
    {
        // EventController.CurrentLevelChanged?.Invoke();
        levelText.ChangeLevel();
        TGDatas.TotalTurkeyText = TGDatas.TotalTurkey;
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
        AdController.Instance.ShowBanner();

        MoveInAnim();

        // SoundController.Instance.PlayOnce(SoundType.EnemyStart);

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
                if (DataBridge.Instance.NextLevelLoaded != DataBridge.Instance.PreviousLevelLoaded)
                    DataBridge.Instance.NextLevelLoaded.SetLevelLoaded(go.Item2, fakeLevelIndex + 1); // fakeLevelIndex + 1, fakeLevelIndex + 1
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
            // Next level || Home to GamePlay and Next = Previous (random)
            if (DataBridge.Instance.NextLevelLoaded != null && DataBridge.Instance.NextLevelLoaded.CurrentFakeLevelIndex == fakeIndex)
            {
                levelInstall = DataBridge.Instance.NextLevelLoaded;
                LoadNextLevel(fakeIndex);
            }
            // Home to Play and Previousloaded
            else if (DataBridge.Instance.PreviousLevelLoaded != null && DataBridge.Instance.PreviousLevelLoaded.CurrentFakeLevelIndex == fakeIndex)
            {
                levelInstall = DataBridge.Instance.PreviousLevelLoaded;
                LoadNextLevel(fakeIndex);
            }
            else
            {
                // start game || loop
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

        while (levelInstall == null)
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

        // opacity.SetActive(Root.LevelMap.DurationMoveCamera > 0);
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
            KillSequence();
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

        // ResourcesController.Achievement.ResetNumberTemp();
        // ResourcesController.DailyQuest.ResetNumberTemp();

        PopupController.Instance.DismissAll();

        FadeInOverlay(() =>
        {
            KillSequence();
            _isReplay = true;
            Camera.main.transform.position = positionCameraOrigin;
            Instance.LoadLevel(Data.CurrentLevel);

            AdController.Instance.ShowInterstitial(() =>
            {

            }, false);
            if (RemoteConfigController.Instance.IsShowInterLose)
                Root.IncreaseTotalLevelWin();

        });
    }

    public void OnSkipLevel()
    {
        FadeInOverlay();
        OnSkipLevel(null);
    }

    public void OnSkipLevel(Action onAdCompleted)
    {
        AnalyticController.SkipLevel();

        PopupController.Instance.DismissAll();
        KillSequence();
        AdController.Instance.ShowRewardedAd(() =>
        {
            Data.CurrentLevel++;
            if (Data.CurrentLoopLevel != -1) Data.IsWinCurrentLoopLevel = true;
            OnNextLevel();
            onAdCompleted?.Invoke();
        });
    }

    public void OnBackToHome()
    {
        FadeInOverlay(() =>
        {
            AdController.Instance.HideBanner();
            PopupController.Instance.DismissAll();
            KillSequence();
            SceneManager.LoadScene(Constants.HOME_SCENE);
        });
    }

    private void KillSequence()
    {
        sequence.Kill();
        Player.KillSequence();
    }

    public void OnWinLevel()
    {
        AnalyticController.CompleteLevel();
        TGDatas.TotalTurkey = TGDatas.TotalTurkeyText;
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
        // ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.LevelPassed);

        MoveOutAnim();

        // firePaper.gameObject.SetActive(true); don't use it anymore because we have win scene.
        GameState = EGameState.Win;
        SoundController.Instance.PlayOnce(SoundType.Win);

        sequence = DOTween.Sequence().AppendInterval(delayWinLose / 2).AppendCallback(() =>
        {
            FadeInOverlay(() =>
            {
                Data.CurrentLevel++;
                Data.CountPlayLevel++;
                if (Data.CurrentLoopLevel != -1) Data.IsWinCurrentLoopLevel = true;
                if (Data.MaxLevel < Data.CurrentLevel) Data.MaxLevel = Data.CurrentLevel;
                ShowPopupWin();
                // if (Data.CurrentLevel == ResourcesController.Config.LevelShowRate)
                // {
                //     PopupController.Instance.Show<RatingPopup>(null, ShowAction.DoNothing);
                // }
            });
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

            sequence = DOTween.Sequence().AppendInterval(delayWinLose / 2).AppendCallback(() =>
            {
                FadeInOverlay(() =>
                {
                    ShowPopupLose();
                });
            });
        });
    }

    private void ShowPopupWin()
    {
        PopupController.Instance.DismissAll();
        PopupController.Instance.Show<WinPopup>();
    }

    private void ShowPopupLose()
    {
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
        else if (GameState == EGameState.Lose)
        {
            endValue -= new Vector2(.5f, 0);
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
        // if (ResourcesController.Config.EnableTest)
        // {
        //     action?.Invoke();
        // }
        // else
        // {
        //     overlay.gameObject.SetActive(true);
        //     overlay.DOFade(1, .3f).SetEase(Ease.InCubic).OnComplete(() =>
        //     {
        //         action?.Invoke();
        //     });
        // }
        overlay.gameObject.SetActive(true);
        overlay.DOFade(1, .3f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            action?.Invoke();
        });
    }

    private void FadeOutOverlay(Action action)
    {
        // if (ResourcesController.Config.EnableTest)
        // {
        //     overlay.gameObject.SetActive(false);
        //     NotiQuestController.Instance.Show();
        // }
        // else
        // {
        //     overlay.DOFade(0, 1f).SetEase(Ease.InCubic).OnComplete(() =>
        //     {
        //         overlay.gameObject.SetActive(false);
        //         NotiQuestController.Instance.Show();
        //         action?.Invoke();
        //     });
        // }
        overlay.DOFade(0, 1f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            overlay.gameObject.SetActive(false);
            // NotiQuestController.Instance.Show();
            action?.Invoke();
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
        // moveOutAnimations.ForEach(item => {item.Reset());

        foreach (var item in moveOutAnimations)
        {
            if (item != null) item.Reset();
        }
    }

    public void ShowFighterOverlay()
    {
        fighterOverlay = Instantiate(ResourcesController.Config.FighterOverlay, Root.transform.parent);
    }

    public void FightingBoss()
    {
        FadeInOverlay(() =>
        {
            skipButton.SetActive(false);
            if(Boss().GetComponent<Unit>() as EnemyDragonHead) bossFaceBlood.sprite = Boss().GetComponent<EnemyDragonHead>().bossFace;
            bossFaceBlood.SetNativeSize();
            backgroundBoss.SetActive(true);
            float endValue = (Player.transform.position.x + Boss().transform.position.x)/2.0f;
            Camera.main.transform.position = new Vector3(endValue, Camera.main.transform.position.y, 0);
            FadeOutOverlay(() =>
            {
                SetEnableLeanTouch(false);
                //float endValue = (Player.transform.position.x + visitTowers[indexVisitTower + 1].transform.position.x) / 2;
                SetEnableLeanTouch(true);
                DOTween.Sequence().AppendInterval(0.2f).OnComplete(() =>
                {
                    player.Turn = ETurn.FightingBoss;
                });
            });

        });
    }
}