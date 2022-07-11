using System;
using System.Collections.Generic;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Lean.Touch;
using UnityEngine;
using Spine.Unity;

public class Player : Unit, IAnim, IHasSkeletonDataAsset
{
    public bool IsDie;
    public bool IsDie3;
    [SerializeField] private SkeletonDataAsset skeletonDataAsset;
    public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;

    [SerializeField] private SkeletonGraphic skeleton;
    [SerializeField] private Rigidbody2D rigid2D;
    [SerializeField] private Collider2D coll2D;
    [SerializeField] private Collider2D groundCollider;
    [SerializeField] private Collider2D searchTargetCollider;

    [SerializeField] private LeanSelectableByFinger leanSelectableByFinger;
    [SerializeField] private PlayerDragTranslate dragTranslate;

    [SerializeField] private LayerMask searchTargetMark;
    [SerializeField] private SpineAttackHandle attackHandle;
    [SerializeField] private float countdownAttack = 1.25f;
    [SerializeField, Range(0, 10)] private float moveSpeed = 1.5f;
    [SerializeField] private ETurn turn = ETurn.None;
    [SerializeField, SpineSkin] private string skinLess;
    [SerializeField, SpineSkin] private string skinWolfGhost;

    public ItemType EquipType;

    [SerializeField] private ParticleSystem effectIncreaseDamge;
    [SerializeField] private ParticleSystem effectBlood;
    [SerializeField] private ParticleSystem effectBlood2;
    [SerializeField] private ParticleSystem effectBlood3;
    [SerializeField] private ParticleSystem effectBloodSaw;
    [SerializeField] private ParticleSystem effectHitWall;
    [SerializeField] private ParticleSystem effectPickSword;
    [SerializeField] private ParticleSystem effectHit;
    [SerializeField] private ParticleSystem effectHitKappa;
    [SerializeField] private ParticleSystem effectFingerPress;
    [SerializeField] private ParticleSystem effectThunder1;
    [SerializeField] private ParticleSystem effectThunder2;
    [SerializeField] private ParticleSystem effectBomb;
    [SerializeField] private ParticleSystem effectTornado;
    [SerializeField] private ParticleSystem effectHitEnemy;
    [SerializeField] private ParticleSystem effectKillWolfGhost;
    [SerializeField] private ParticleSystem effectFire;
    [SerializeField] private ParticleSystem effectIce;
    [SerializeField] private ParticleSystem effectElectric;
    [SerializeField] private ParticleSystem effectPoison;
    [SerializeField] private ParticleSystem effectFadeIn;
    [SerializeField] private ParticleSystem effectFadeOut;
    [SerializeField] private ParticleSystem effectPoisonSecretary;
    [SerializeField] private ParticleSystem effectClaws;
    [SerializeField] private ParticleSystem effectElectricHuggy;
    [SerializeField] private GameObject effectPoisonGroundSecretary;

    [SerializeField] private GameObject shuriken;
    [SerializeField] private GameObject bow;
    [SerializeField] private GameObject keyObject;
    public override EUnitType Type { get; protected set; } = EUnitType.Hero;
    public ETurn Turn { get => turn; private set => turn = value; }

    private Vector3 _defaultPosition;
    private RoomTower _defaultRoom = null;
    private float _countdownAttack = 0f;
    private List<Collider2D> _cachedSearchCollider = new List<Collider2D>();
    private Unit _target;
    private Item _itemTarget;
    private bool _flagAttack;
    private Vector2 _movement;
    private RaycastHit2D _hitItem;

    private bool _isMouseUpDragDetected;
    private RoomTower _parentRoom;
    private bool _dragValidateRoomFlag;
    private List<string> swordNames = new List<string>();
    private bool hasKey = false;
    private LevelMap levelMap => GameController.Instance.Root.LevelMap;
    private bool hasBloodEnemy;
    private Sequence sequence;

    public void SetParentRoom(RoomTower parentRoom)
    {
        _parentRoom = parentRoom;
    }

    private void Start()
    {
        dragTranslate.valiateAction = ValidateChooseRoom;
        attackHandle.Initialize(OnAttackByEvent, OnEndAttackByEvent);
        UpdateDefault();
        StartDragTurn();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_isMouseUpDragDetected && other.gameObject == _parentRoom.floor.gameObject)
        {
            StartSearchingTurn();
        }
    }

    // Update previous position and previous visit room.
    public void UpdateDefault()
    {
        _defaultPosition = transform.localPosition;
        _defaultRoom = transform.parent.GetComponent<RoomTower>();
    }

    public (bool, int) CheckCorrectArea()
    {
        bool check = false;
        int indexSlot = 0;
        var tower = levelMap.visitTower;
        for (int i = 0; i < tower.slots.Count; i++)
        {
            check = tower.slots[i].GetComponent<RectTransform>().Contains(Input.mousePosition, Camera.main); // why don't use RectTransformUtility.ScreenPointToWorldPointInRectangle
            if (check)
            {
                var hasUnitNotInvalid = tower.slots[i].IsRoomHaveUnitNotInvalid();
                if (!hasUnitNotInvalid) return (false, 0);
                indexSlot = i;
                break;
            }
        }

        return (check, indexSlot);
    }

    // when drag the player to a visit room, it will be marked.
    public void ValidateChooseRoom()
    {
        var result = CheckCorrectArea();

        if (result.Item1)
        {
            if (!_dragValidateRoomFlag)
            {
                _dragValidateRoomFlag = true;

                var temp = levelMap.visitTower.slots[result.Item2];
                temp.UpdateStatusSelectRoom(true, true);
            }
        }
        else
        {
            if (_dragValidateRoomFlag)
            {
                _dragValidateRoomFlag = false;
                levelMap.ResetSelectVisitTower();
            }
        }
    }

    public void ResetPlayerState()
    {
        transform.SetParent(_defaultRoom.transform, false);
        transform.localPosition = _defaultPosition;
    }

    public void OnSelected()
    {
        rigid2D.gravityScale = 0;
        // turn off all colliders
        coll2D.enabled = false;
        groundCollider.enabled = false;
        searchTargetCollider.enabled = false;

        GameController.Instance.SetSlicerActive(true);
        // GetComponent<Canvas>().overrideSorting = true;
    }

    public void OnDeSelected()
    {
        rigid2D.gravityScale = 1;
        // turn on all colliders
        coll2D.enabled = true;
        groundCollider.enabled = true;
        searchTargetCollider.enabled = true;

        GameController.Instance.SetSlicerActive(false);
        // GetComponent<Canvas>().overrideSorting = false;
    }

    public void OnMouseDown() // => can drag player even by its searching collider.
    {
        if (GameController.Instance.GameState == EGameState.Playing && Turn == ETurn.Drag)
        {
            effectFingerPress.gameObject.SetActive(true);
            effectFingerPress.Play();

            SoundController.Instance.PlayOnce(SoundType.HeroDrag);

            _isMouseUpDragDetected = false;
            if (Turn == ETurn.Drag)
            {
                OnSelected();
                leanSelectableByFinger.SelfSelected = true;
            }
        }
    }

    public void OnMouseUp()
    {
        if (GameController.Instance.GameState == EGameState.Playing && Turn == ETurn.Drag)
        {
            // SoundController.Instance.PlayOnce(SoundType.HeroDrop);
            if (!dragTranslate.DragTranslateFlag)
            {
                _isMouseUpDragDetected = false;
                OnDeSelected();
                leanSelectableByFinger.Deselect();
                return;
            }


            dragTranslate.DragTranslateFlag = false;
            levelMap.ResetSelectVisitTower();
            var checkArea = CheckCorrectArea();
            if (checkArea.Item1)
            {
                var parentRoom = levelMap.visitTower.slots[checkArea.Item2];
                MoveToSlot(parentRoom);
                Turn = ETurn.None;
            }
            else
            {
                _isMouseUpDragDetected = false;
                ResetPlayerState();
                OnDeSelected();
                leanSelectableByFinger.Deselect();
            }
        }

    }

    public void FlashToSlot(RoomTower parentRoom) // use while people click to a room
    {
        if (Turn != ETurn.Drag) return;
        Turn = ETurn.None;

        effectFadeIn.Play();
        Skeleton.Play("FadeOut", false);
        DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
        {
            MoveToSlot(parentRoom);
            effectFadeOut.Play();
            Skeleton.Play("FadeIn", false);
        });
    }

    public void MoveToSlot(RoomTower parentRoom)
    {
        RoomTower cache = null;
        _parentRoom = parentRoom;
        var currentRoom = transform.parent.GetComponent<RoomTower>();

        // increasing room for home tower
        if (currentRoom != null && levelMap.visitTower.slots.Contains(currentRoom) && currentRoom.IsClearEnemyInRoom() && !currentRoom.IsContaintItem() && !currentRoom.IsContaintPrincess())
        {
            cache = currentRoom;
        }

        transform.SetParent(_parentRoom.transform, false);
        transform.localPosition = _parentRoom.spawnPoint.localPosition;
        UpdateDefault();

        if (cache != null)
        {
            levelMap.visitTower.RemoveSlot(cache);

            levelMap.homeTower.AddSlot();
        }

        levelMap.visitTower.RefreshRoom();
        levelMap.homeTower.RefreshRoom();

        OnDeSelected();
        leanSelectableByFinger.Deselect();

        _isMouseUpDragDetected = true;

        // not general
        if (Onboarding1.Instance != null)
        {
            Onboarding1.Instance.ShowRound2();
        }
        if (Onboarding.Instance != null)
        {
            Onboarding.Instance.EndRound();
        }
    }

    #region turn

    public void StartDragTurn()
    {
        Turn = ETurn.Drag;
        _countdownAttack = countdownAttack;
        if (Onboarding.Instance != null)
        {
            Onboarding.Instance.StartRound();
        }
        if (_parentRoom != null)
        {
            _parentRoom.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void StartSearchingTurn()
    {
        Turn = ETurn.Searching;
    }

    public void StartMoveToItemTurn() { Turn = ETurn.MoveToItem; }

    private void StartAwaitTurn() { Turn = ETurn.None; }

    #endregion

    private void Update()
    {
        if (GameController.Instance.GameState != EGameState.Playing && (Turn == ETurn.Drag || Turn == ETurn.None) || state == EUnitState.Invalid) return;

        _countdownAttack = Mathf.Max(0, _countdownAttack - Time.deltaTime);
        //_countdownAttack <= 0 && 

        if (!GameController.Instance.IsOnboarding)
        {
            SearchingTarget();
        }
    }

    private void AddJumpAnimation()
    {
        foreach (var slot in levelMap.visitTower.slots)
        {
            foreach (var item in slot.items)
            {
                if (item.EquipType == ItemType.Key)
                {
                    item.GetComponent<ItemEquip>().AddJumpAnimation();
                }
            }
        }
    }

    private void SearchingTarget()
    {
        if (Turn != ETurn.Searching) return;
        _cachedSearchCollider = new List<Collider2D>();
        searchTargetCollider.OverlapCollider(new ContactFilter2D() { layerMask = searchTargetMark.value, useTriggers = true, useLayerMask = true },
            _cachedSearchCollider);

        _cachedSearchCollider.RemoveAll(_ => _.gameObject.CompareTag("Ground") || _ == coll2D || _ == groundCollider);
        if (_cachedSearchCollider.Count == 0)
        {
            StartDragTurn();
            return;
        }

        #region detect target

        float length = 1000;
        int index = 0;

        // why don't use raycast for get the nearest item/enemy
        for (int i = 0; i < _cachedSearchCollider.Count; i++)
        {
            var coll = _cachedSearchCollider[i];
            float distance = Mathf.Abs(coll.transform.parent.position.x - transform.position.x);

            if (distance < length)
            {
                var unit = coll.GetComponentInParent<IUnit>();
                if (unit is { State: EUnitState.Invalid })
                {
                    continue;
                }

                length = distance;
                index = i;
            }
        }

        var cacheCollider = _cachedSearchCollider[index];
        if (cacheCollider == coll2D) return; // ? removed above

        #endregion

        _target = cacheCollider.GetComponentInParent<Unit>();
        if (_target != null)
        {
            if (_target is { State: EUnitState.Invalid }) // ? checked above
            {
                _target = null;
                return;
            }

            switch (_target.Type)
            {
                case EUnitType.Enemy:
                    if (_target is { State: EUnitState.Normal })//&& _countdownAttack <= 0)
                    {
                        // var distance = Math.Abs((_target.transform.localPosition.x - transform.localPosition.x));
                        // if (distance >= 250)
                        // {
                        //     PLayMove(true);
                        //     transform.DOLocalMoveX(_target.transform.position.x - 20, 0.5f).SetEase(Ease.Linear).OnComplete(() => { AfterMoveToEnemy();});
                        // }
                        // else 
                        AfterMoveToEnemy();

                        void AfterMoveToEnemy()
                        {
                            Turn = ETurn.Attacking;
                            _countdownAttack = countdownAttack;

                            // check damage
                            _flagAttack = damage > _target.Damage;

                            if (_flagAttack)
                            {
                                hasBloodEnemy = true;
                                _target.OnAttack(damage, null); // enemy attack first
                                                                // effect of the enemy attack on the player
                                if (_target as EnemyGoblin || _target as EnemyKappa)
                                {
                                    DOTween.Sequence().AppendInterval(.3f).AppendCallback(() =>
                                    {
                                        if (_target as EnemyKappa)
                                        {
                                            // effectHitKappa.gameObject.SetActive(true);
                                            // effectHitKappa.Play();
                                            effectPoisonSecretary.Play();
                                            DOTween.Sequence().AppendInterval(.01f).AppendCallback(() =>
                                            {
                                                if (effectPoisonGroundSecretary.activeSelf == false) effectPoisonGroundSecretary.SetActive(true);
                                            });
                                        }
                                        else
                                        {
                                            ParticleSystem bomb = Instantiate(effectBomb, transform.parent);
                                            bomb.transform.position = transform.position;
                                            bomb.gameObject.SetActive(true);
                                            bomb.Play();
                                            SoundController.Instance.PlayOnce(SoundType.BombGoblin);
                                        }

                                        skeleton.Play("Die2", false);

                                        SoundController.Instance.PlayOnce(SoundType.SecretaryAttack);
                                        DOTween.Sequence().AppendInterval(0.3f).AppendCallback(() =>
                                        {
                                            PlayAttack();
                                        });
                                    });

                                    var cacheDamage = Damage;
                                    Damage -= _target.Damage; // with enemy is either Kappa or Goblin
                                    _target.TxtDamage.gameObject.SetActive(true);
                                    _target.TxtDamage.transform.DOMove(TxtDamage.transform.position, .5f).SetEase(Ease.InCubic).OnComplete(() =>
                                    {
                                        TxtDamage.transform.DOPunchScale(Vector3.one * 1.1f, .3f, 0);
                                        TxtDamage.DOCounter(cacheDamage, Damage, 0);
                                        _target.TxtDamage.gameObject.SetActive(false);
                                    });
                                }
                                else
                                {
                                    DOTween.Sequence().AppendInterval(0.1f).AppendCallback(() =>
                                    {
                                        PlayAttack();
                                    });
                                }
                            }
                            else
                            {
                                hasBloodEnemy = false;
                                Turn = ETurn.Lost;
                                PlayAttack();
                                _target.OnAttack(damage, BeingAttackedCallback);
                            }
                        }
                    }

                    break;
                case EUnitType.Princess:
                    if (_target is { State: EUnitState.Normal } && _countdownAttack <= 0)
                    {
                        Turn = ETurn.SavingPrincess;
                        _countdownAttack = countdownAttack;

                        var distance = Math.Abs((_target.transform.localPosition.x - transform.localPosition.x));
                        if (distance >= 80)
                        {
                            PLayMove(true);
                            transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear).OnComplete(() => { SavePrincess(); });
                        }
                        else
                        {
                            SavePrincess();
                        }

                        void SavePrincess()
                        {
                            Princess princess = _target as Princess;
                            if (!hasKey && princess.LockObj != null) // the princess is locked
                            {
                                Turn = ETurn.Drag;
                                PlayIdle(true);
                                AddJumpAnimation();
                                return;
                            }
                            // Save the kissy
                            if (princess.LockObj == null)
                            {
                                skeleton.Play("Open", false);
                                DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
                                {
                                    princess.PlayOpen();
                                    DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
                                    {
                                        princess.PlayWin(true);
                                        GameController.Instance.OnWinLevel();
                                    });
                                    GiveFlower();
                                });
                            }
                            else
                            {
                                skeleton.Play("Yawn", false);
                                DontUseSwordAnymore();
                                keyObject.SetActive(true);
                                keyObject.transform.DOMove(princess.LockObj.transform.position, 1).OnComplete(() =>
                                {
                                    keyObject.transform.DOLocalRotate(new Vector3(0, 0, 180), .5f).OnComplete(() =>
                                    {
                                        keyObject.transform.DOScale(new Vector3(.3f, .3f, 3f), .5f).OnComplete(() =>
                                        {
                                            keyObject.gameObject.SetActive(false);
                                            princess.LockObj?.gameObject.SetActive(true);
                                            princess.LockObj2?.DOFade(0, .3f);
                                            princess.PlayOpenCage();
                                            princess.LockObj?.gameObject.SetActive(false);
                                            DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
                                            {
                                                princess.PlayWin(true);
                                                GameController.Instance.OnWinLevel();
                                            });
                                            GiveFlower();
                                        });
                                    });
                                });
                            }
                        }
                    }
                    break;
            }
        }
        else
        {
            _itemTarget = cacheCollider.GetComponentInParent<Item>();
            if (_itemTarget != null && _itemTarget.State != EUnitState.Invalid && Turn != ETurn.MoveToItem)
            {
                Turn = ETurn.MoveToItem;
                DOTween.Kill(transform); // detect enemy before item/gem => kill

                switch (_itemTarget.Type)
                {
                    case EUnitType.Item:
                        {
                            var distance = Math.Abs(_itemTarget.transform.localPosition.x - transform.localPosition.x);
                            if (distance >= 110)
                            {
                                PLayMove(true);
                                float endValue = _itemTarget.transform.position.x - 1;
                                switch (_itemTarget.EquipType)
                                {
                                    case ItemType.BrokenBrick:
                                        endValue = _itemTarget.transform.position.x - .5f;
                                        break;
                                }
                                if (_itemTarget as ItemTeleport)
                                {
                                    endValue = _itemTarget.transform.position.x;
                                }
                                transform.DOMoveX(endValue, 0.5f).SetEase(Ease.Linear).OnComplete(() => UseItem());
                            }
                            else
                            {
                                UseItem();
                            }

                            break;
                        }

                    case EUnitType.Gem:
                        {
                            var distance = Math.Abs(_itemTarget.transform.localPosition.x - cacheCollider.transform.localPosition.x);
                            if (distance >= 110)
                            {
                                PLayMove(true);
                                transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear).OnComplete(() => UseItem(ELevelCondition.CollectGold));
                            }
                            else
                            {
                                UseItem(ELevelCondition.CollectGold);
                            }

                            break;
                        }
                }

                void UseItem(ELevelCondition condition = ELevelCondition.CollectChest)
                {
                    if (_itemTarget as ItemTeleport != null)
                    {
                        //skeleton.Play("Teleport", false);
                        DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                        {
                            _itemTarget.Collect(this);
                        });
                        return;
                    }

                    if (!hasKey && _itemTarget as ItemChest != null)
                    {
                        Turn = ETurn.Drag;
                        PlayIdle(true);
                        AddJumpAnimation();
                        return;
                    }

                    Turn = ETurn.UsingItem;
                    if (_itemTarget.EquipType == ItemType.Chest)
                    {
                        DontUseSwordAnymore();
                        if (GameController.Instance.ItemLock != null) skeleton.Play("Open", false);
                        else
                        {
                            keyObject.SetActive(true);
                            skeleton.Play("See", true);
                            keyObject.transform.DOMove(_itemTarget.transform.position, 1).OnComplete(() =>
                            {
                                keyObject.transform.DOLocalRotate(Vector3.zero, .5f).OnComplete(() =>
                                {
                                    keyObject.transform.DOScale(new Vector3(.1f, .1f, 1f), .5f).OnComplete(() =>
                                    {
                                        keyObject.gameObject.SetActive(false);
                                    }
                                    );
                                });
                            });
                        }
                    }
                    else PlayUseItem(_itemTarget.EquipType);
                    float timeDelay = _itemTarget.EquipType == ItemType.Bow ||
                        _itemTarget.EquipType == ItemType.BrokenBrick ||
                        _itemTarget.EquipType == ItemType.Trap ||
                        _itemTarget.EquipType == ItemType.Bomb ||
                        _itemTarget.EquipType == ItemType.Electric ? 1.2f : .5f;
                    // Time delay after collect item
                    DOTween.Sequence().AppendInterval(timeDelay).AppendCallback(() =>
                    {
                        if (levelMap.condition == condition)
                        {
                            switch (condition)
                            {
                                case ELevelCondition.CollectChest:
                                    if (_itemTarget as ItemChest != null)
                                    {
                                        float timeDelay = 0.7f;
                                        if (GameController.Instance.ItemLock == null) timeDelay = 1.7f;
                                        DOTween.Sequence().AppendInterval(timeDelay).AppendCallback(() =>
                                        {
                                            GameController.Instance.OnWinLevel();
                                        });
                                        PlayIdle(true);
                                    }
                                    else
                                    {
                                        StartSerching();
                                    }
                                    break;
                                default:
                                    DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
                                    {
                                        GameController.Instance.OnWinLevel();
                                    });
                                    PlayWin(true);
                                    break;
                            }
                        }
                        else
                        {
                            StartSerching();
                        }
                    });

                    void StartSerching()
                    {
                        StartSearchingTurn();
                        PlayIdle(true);
                        if (levelMap.visitTower.IsClearTower() && levelMap.hasNewVisitTower)
                        {
                            levelMap.ChangeToNewVisitTower();
                        }
                    }

                    timeDelay = .5f;
                    switch (_itemTarget.EquipType)
                    {
                        case ItemType.Baseball:
                        case ItemType.SwordBlood:
                            timeDelay = .2f;
                            break;
                        case ItemType.Bow:
                            timeDelay = 0;
                            break;
                        case ItemType.Chest:
                            if (GameController.Instance.ItemLock != null) timeDelay = .5f;
                            else timeDelay = 2;
                            break;
                    }
                    DOTween.Sequence().AppendInterval(timeDelay).AppendCallback(() =>
                    {
                        _itemTarget.Collect(this);
                    });
                }
            }
            else
            {
                _itemTarget = null;
            }
        }
    }

    private void MoveLeft()
    {
        if (_hitItem.collider != null && state != EUnitState.Invalid)
        {
            rigid2D.velocity = new Vector2(0, rigid2D.velocity.y);
            if (IsStop()) return;
            PLayMove(true);
            return;
        }

        if (state != EUnitState.Invalid)
        {
            if (IsStop()) return;
            _movement = Vector2.left * moveSpeed;
            rigid2D.velocity = new Vector2(_movement.x, rigid2D.velocity.y);
            PLayMove(true);
        }
        else
        {
            rigid2D.velocity = new Vector2(0, rigid2D.velocity.y);
        }
    }

    private void MoveRight()
    {
        if (_hitItem.collider != null && state != EUnitState.Invalid)
        {
            rigid2D.velocity = new Vector2(0, rigid2D.velocity.y);
            if (IsStop()) return;
            PLayMove(true);
            return;
        }

        if (state != EUnitState.Invalid)
        {
            if (IsStop()) return;
            _movement = Vector2.right * moveSpeed;
            rigid2D.velocity = new Vector2(_movement.x, rigid2D.velocity.y);
            PLayMove(true);
        }
        else
        {
            rigid2D.velocity = new Vector2(0, rigid2D.velocity.y);
        }
    }

    private bool IsStop()
    {
        return GameController.Instance.GameState == EGameState.Lose || GameController.Instance.GameState == EGameState.Win || Turn == ETurn.UsingItem ||
                Turn == ETurn.MoveToItem;
    }

    private void OnAttackByEvent()
    {
        if (_target != null)
        {
            var _cacheTarget = _target;
            var cacheDamage = Damage;
            if (_flagAttack)
            {
                _cacheTarget.CheckTurkey();
                _cacheTarget.OnBeingAttacked();

                if (_cacheTarget as EnemyGoblin || _cacheTarget as EnemyKappa)
                {
                    if (_cacheTarget as EnemyKappa)
                    {
                        // Damage -= _cacheTarget.Damage;
                        effectPoisonGroundSecretary.SetActive(false);
                    }
                }
                else
                {
                    Damage += _cacheTarget.Damage;
                    DOTween.Sequence().AppendInterval(.05f).AppendCallback(() =>
                    {
                        effectIncreaseDamge.gameObject.SetActive(true);
                        effectIncreaseDamge.Play();
                    });

                    _cacheTarget.TxtDamage.transform.SetParent(transform);
                    _cacheTarget.TxtDamage.gameObject.SetActive(true);
                    _cacheTarget.TxtDamage.transform.DOMove(TxtDamage.transform.position, .5f).SetEase(Ease.InCubic).OnComplete(() =>
                    {
                        TxtDamage.transform.DOPunchScale(Vector3.one * 1.1f, .3f, 0);
                        TxtDamage.DOCounter(cacheDamage, Damage, 0);
                        _cacheTarget.TxtDamage.gameObject.SetActive(false);
                    });
                }

            }
        }
    }

    public bool IncreaseDamage(int damage)
    {
        var cacheDamage = Damage;
        if (damage > 0)
        {
            SoundController.Instance.PlayOnce(SoundType.HeroUpLevel);
            effectPickSword.gameObject.SetActive(true);
            effectPickSword.Play();
        }

        Damage += damage;
        TxtDamage.DOCounter(cacheDamage, Damage, 0.5f);

        if (Damage <= 0)
        {
            SoundController.Instance.PlayOnce(SoundType.HeroDownLevel);
            State = EUnitState.Invalid;
            Turn = ETurn.None;
            PlayDead();
            GameController.Instance.OnLoseLevel();
        }

        return Damage > 0;
    }

    public void ChangeSword(string swordName = "")
    {
        if (swordName != "")
        {
            if (swordName != "Key")
            {
                swordNames.Clear();
                if (hasKey && GameController.Instance.ItemLock == null) swordNames.Add("Key");
            }
            swordNames.Add(swordName);
        }
        if (swordNames.Count > 0)
        {
            Skeleton.ChangeSword(swordNames);
        }
    }

    private void DontUseSwordAnymore()
    {
        swordNames.Clear();
        Skeleton.ChangeSword(swordNames);
    }

    private void OnEndAttackByEvent()
    {
        PlayIdle(true);
        if (_target as EnemyWolfGhost)
        {
            DOTween.Sequence().AppendInterval(.3f).AppendCallback(() =>
            {
                skeleton.Play("PickUp", false);
            });

            DOTween.Sequence().AppendInterval(.8f).AppendCallback(() =>
            {
                EquipType = ItemType.Claws;

                ChangeSword(skinWolfGhost);
                PlayIdle(true);
                AttackByEvent();
            });

            DOTween.Sequence().AppendInterval(.6f).AppendCallback(() =>
            {
                effectKillWolfGhost.gameObject.SetActive(true);
                effectKillWolfGhost.Play();
            });
        }
        else
        {
            AttackByEvent();
        }
    }

    private void AttackByEvent()
    {
        if (Turn != ETurn.Lost)
        {
            DOTween.Sequence().AppendInterval(0.3f).AppendCallback(() =>
            {
                _target.gameObject.SetActive(false);
                var room = levelMap.visitTower.RoomContainPlayer(this);
                if (room != null && (!room.IsClearEnemyInRoom() || room.IsContaintItem() || room.IsContaintPrincess()))
                {
                    // If player attack very fast, enemy cannot call event "OnBullet"  
                    StartSearchingTurn();
                    SearchingTarget();
                    return;
                }
                StartDragTurn();
                if (levelMap.visitTower.IsClearTower())
                {
                    if (levelMap.hasNewVisitTower)
                    {
                        levelMap.ChangeToNewVisitTower();
                    }
                    else if (IsWinCondition(levelMap.condition))
                    {
                        PlayWin(true);
                        GameController.Instance.OnWinLevel();
                    }
                }
            });
        }
    }

    private bool IsWinCondition(ELevelCondition condition)
    {
        bool isWin = false;

        switch (condition)
        {
            case ELevelCondition.KillAll:
            case ELevelCondition.KillBear:
            case ELevelCondition.KillDemon:
            case ELevelCondition.KillDragon:
            case ELevelCondition.KillGhost:
            case ELevelCondition.KillWolf:
            case ELevelCondition.KillBone:
            case ELevelCondition.KillYeti:
            case ELevelCondition.KillPoliceStick:
            case ELevelCondition.KillSpider:
            case ELevelCondition.KillDragonGold:
            case ELevelCondition.KillFire:
                isWin = true;
                break;
        }

        return isWin;
    }

    private void BeingAttackedCallback()
    {
        if (_target as EnemyKappa)
        {
            effectPoisonSecretary.Play();
            DOTween.Sequence().AppendInterval(.01f).AppendCallback(() =>
            {
                if (effectPoisonGroundSecretary.activeSelf == false) effectPoisonGroundSecretary.SetActive(true);
            });
        }
        else if (_target as EnemyGoblin)
        {
            ParticleSystem bomb = Instantiate(effectBomb, transform.parent);
            bomb.transform.position = transform.position;
            bomb.gameObject.SetActive(true);
            bomb.Play();
            SoundController.Instance.PlayOnce(SoundType.BombGoblin);
        }
        else
        {
            effectHit.gameObject.SetActive(true);
            effectHit.Play();
        }

        var cacheDamage = damage;
        damage = 0;
        TxtDamage.DOCounter(cacheDamage, damage, 0.5f).OnComplete(() => TxtDamage.text = damage.ToString());
        State = EUnitState.Invalid;
        PlayDead();

        sequence = DOTween.Sequence().AppendInterval(.6f).AppendCallback(() =>
        {
            GameController.Instance.OnLoseLevel();
        });
    }

    public void KillSequence()
    {
        sequence.Kill();
    }

    private void CollectChest() { }

    private void OnTriggerEnter2D(Collider2D other) { }

    public override void OnAttack(int damage, Action callback) { }
    public override void OnBeingAttacked() { }

    public override void DarknessRise() { }

    public override void LightReturn() { }
    public SkeletonGraphic Skeleton => skeleton;
    public void PlayIdle(bool isLoop)
    {
        if (GameController.Instance.GameState != EGameState.Lose && !IsDie)
        {
            if (Data.CurrentSkinHero == skinLess)
            {
                //skeleton.Play("Idle2", true);
                skeleton.Play("Idle", true);
            }
            else
            {
                skeleton.Play("Idle", true);
            }
        }
    }

    private void PlayBloodEnemy(string attack = "")
    {
        if (!hasBloodEnemy) return;

        float timeDelay = attack == "AttackSword" ? .8f : .3f;
        if (!(_target as EnemyGhost))
        {
            DOTween.Sequence().AppendInterval(timeDelay).AppendCallback(() =>
            {
                var main = effectBlood.main;
                main.startColor = _target.ColorBlood;

                main = effectBlood2.main;
                main.startColor = _target.ColorBlood;

                main = effectBlood3.main;
                main.startColor = _target.ColorBlood;

                effectBlood.transform.position = _target.transform.position;
                effectBloodSaw.transform.position = _target.transform.position;
                if (_target as EnemyDragonHead)
                {
                    effectBlood.transform.localPosition += new Vector3(-250, 400, 0);
                    effectBloodSaw.transform.localPosition += new Vector3(-250, 400, 0);
                }
                else if (_target as EnemyKraken)
                {
                    effectBlood.transform.localPosition += new Vector3(-200, 50, 0);
                    effectBloodSaw.transform.localPosition += new Vector3(-200, 50, 0);
                }
                else if (_target as EnemyKraken2)
                {
                    effectBlood.transform.localPosition += new Vector3(60, -85, 0);
                    effectBloodSaw.transform.localPosition += new Vector3(60, -85, 0);
                }
                else if (_target as EnemyKraken3)
                {
                    effectBlood.transform.localPosition += new Vector3(100, 180, 0);
                    effectBloodSaw.transform.localPosition += new Vector3(100, 180, 0);
                }
                else if (_target as EnemyKraken4)
                {
                    effectBlood.transform.localPosition += new Vector3(20, -45, 0);
                    effectBloodSaw.transform.localPosition += new Vector3(20, -45, 0);
                }
                else if (_target as EnemyKraken5)
                {
                    effectBlood.transform.localPosition += new Vector3(200, -355, 0);
                    effectBloodSaw.transform.localPosition += new Vector3(200, -355, 0);
                }
                else if (_target as EnemyKraken6)
                {
                    effectBlood.transform.localPosition += new Vector3(100, -500, 0);
                    effectBloodSaw.transform.localPosition += new Vector3(100, -500, 0);
                }
                else
                {
                    effectBlood.transform.localPosition += new Vector3(0, 40, 0);
                    effectBloodSaw.transform.localPosition += new Vector3(0, 40, 0);
                }
                if (EquipType == ItemType.Saw)
                {
                    effectBloodSaw.gameObject.SetActive(true);
                    effectBloodSaw.Play();
                }
                else
                {
                    effectBlood.gameObject.SetActive(true);
                    effectBlood.Play();
                }
            });
        }
    }

    private void PlayHitEnemy()
    {
        ParticleSystem hitEnemy;
        if (EquipType == ItemType.Claws)
        {
            hitEnemy = Instantiate(effectClaws, transform.parent);
            hitEnemy.transform.position = _target.transform.position;
        }
        else
        {
            hitEnemy = Instantiate(effectHitEnemy, transform.parent);
            hitEnemy.transform.position = _target.transform.position;
        }

        if (_target as EnemyDragonHead)
        {
            hitEnemy.transform.position += new Vector3(0, 6, 0);
        }
        else if (_target as EnemyKraken)
        {
            hitEnemy.transform.position += new Vector3(-2, 1, 0);
        }
        else if (_target as EnemyKraken2)
        {
            hitEnemy.transform.position += new Vector3(0, -1, 0);
        }
        else if (_target as EnemyKraken3)
        {
            hitEnemy.transform.position += new Vector3(0, -1, 0);
        }
        else if (_target as EnemyKraken4)
        {
            hitEnemy.transform.position += new Vector3(0, -1, 0);
        }
        else if (_target as EnemyKraken5)
        {
            hitEnemy.transform.position += new Vector3(0, -6, 0);
        }
        else if (_target as EnemyKraken6)
        {
            hitEnemy.transform.position += new Vector3(0, -6, 0);
        }
        else
        {
            hitEnemy.transform.position += new Vector3(0, 1, 0);
        }
        hitEnemy.gameObject.SetActive(true);
        hitEnemy.Play();
    }
    public void PlayAttack()
    {
        switch (EquipType)
        {
            case ItemType.SwordJapan:
                {
                    // skeleton.Play("AttackKiemJapan", false);
                    skeleton.Play("Attack", false);

                    DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                    {
                        SoundController.Instance.PlayOnce(SoundType.HeroCut3);
                        PlayBloodEnemy();

                        if (hasBloodEnemy)
                        {
                            DOTween.Sequence().AppendInterval(.2f).AppendCallback(() =>
                            {
                                ParticleSystem tornado = Instantiate(effectTornado, transform.parent);
                                tornado.gameObject.SetActive(true);
                                tornado.Play();
                                tornado.transform.position = transform.position;
                                tornado.transform.DOMoveX(_target.transform.position.x, 1).OnComplete(() =>
                                {
                                    Destroy(tornado.gameObject);
                                });
                            });
                        }
                    });
                    break;
                }
            case ItemType.Sword:
                {
                    string[] attacks = { "Attack", "AttackSword", "AttackSword2" };
                    string attack = attacks[UnityEngine.Random.Range(0, attacks.Length)];
                    //skeleton.Play(attack, false);
                    skeleton.Play("Attack", false);

                    // var snowExpore = I2.Loc.ResourceManager.pInstance.LoadFromResources<UnityEngine.GameObject>("Prefabs/Effect/Snow_Explosion");


                    SoundType[] soundTypes = { SoundType.HeroCut, SoundType.HeroCut2, SoundType.HeroCut3 };
                    SoundType soundType = soundTypes[UnityEngine.Random.Range(0, soundTypes.Length)];
                    float timeDelay = attack == "AttackSword" ? .5f : 0;
                    DOTween.Sequence().AppendInterval(timeDelay).AppendCallback(() =>
                    {

                        // DOTween.Sequence().SetDelay(.5f).OnComplete(() =>
                        // {
                        //     var go = Instantiate(snowExpore);
                        //     go.transform.parent = transform;
                        //     go.transform.localScale = new Vector3(200, 200, 200);
                        //     go.transform.localPosition = new Vector3(121, 71, 10);
                        // });

                        SoundController.Instance.PlayOnce(soundType);
                    });

                    PlayBloodEnemy(attack);
                    break;
                }
            case ItemType.Ice:
            case ItemType.Fire:
            case ItemType.Electric:
            case ItemType.Poison:
                {
                    //skeleton.Play("AttackElemental", false);
                    skeleton.Play("Attack2", false);
                    SoundController.Instance.PlayOnce(SoundType.Axe1);

                    if (hasBloodEnemy)
                    {
                        DOTween.Sequence().AppendInterval(1f).AppendCallback(() =>
                        {
                            ParticleSystem temp;
                            switch (EquipType)
                            {
                                case ItemType.Ice:
                                    temp = effectIce;
                                    break;
                                case ItemType.Fire:
                                    temp = effectFire;
                                    break;
                                case ItemType.Electric:
                                    temp = effectElectric;
                                    break;
                                default:
                                    temp = effectPoison;
                                    break;
                            }
                            ParticleSystem elemental = Instantiate(temp, transform.parent);
                            elemental.transform.position = transform.position + Vector3.up * 2f + Vector3.right * 1f;
                            elemental.gameObject.SetActive(true);
                            elemental.Play();
                            elemental.transform.DOMove(_target.transform.position + Vector3.up * .5f + Vector3.right * .5f, .2f).OnComplete(() =>
                            {
                                DOTween.Sequence().AppendInterval(.2f).AppendCallback(() =>
                                {
                                    Destroy(elemental.gameObject);
                                });
                            });
                            SoundController.Instance.PlayOnce(SoundType.Elemental);
                        });
                    }
                    break;
                }
            case ItemType.Bow2:
                //skeleton.Play("AttackBow", false);
                skeleton.Play("Attack", false);
                SoundController.Instance.PlayOnce(SoundType.Bow2);
                DOTween.Sequence().AppendInterval(.3f).AppendCallback(() =>
                {
                    PlayBloodEnemy();
                });
                DOTween.Sequence().AppendInterval(.8f).AppendCallback(() =>
                {
                    GameObject bow1 = Instantiate(bow, transform.parent);
                    bow1.transform.position = transform.position + new Vector3(1, 1, 0);
                    Vector3 endValue = bow1.transform.position + new Vector3(5, 0, 0);
                    bow1.transform.DOMove(endValue, .5f).OnComplete(() => Destroy(bow1));
                });
                break;
            case ItemType.Polllaxe:
                //skeleton.Play("AttackPollaxe", false);
                skeleton.Play("Attack", false);
                SoundController.Instance.PlayOnce(SoundType.Pollaxe);
                PlayBloodEnemy();
                break;
            case ItemType.Mace:
                {
                    //skeleton.Play("AttackMace", false);
                    skeleton.Play("Attack", false);
                    SoundController.Instance.PlayOnce(SoundType.Mace);
                    break;
                }
            case ItemType.SwordBlood:
                //skeleton.Play("AttackSword3", false);
                skeleton.Play("Attack", false);
                DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                {
                    SoundController.Instance.PlayOnce(SoundType.HeroCut3);
                });
                DOTween.Sequence().AppendInterval(.2f).AppendCallback(() =>
                {
                    PlayBloodEnemy();
                });
                break;
            case ItemType.Shuriken:
                //skeleton.Play("Shuriken", false);
                skeleton.Play("AttackPipe", false);
                SoundController.Instance.PlayOnce(SoundType.Knife);
                PlayHitEnemy();
                // DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                // {
                //     GameObject shuriken1 = Instantiate(shuriken, transform.parent);
                //     shuriken1.transform.position = transform.position + new Vector3(1, 1, 0);
                //     Vector3 endValue = shuriken1.transform.position + new Vector3(5, 0, 0);
                //     shuriken1.transform.DOMove(endValue, 1f).OnComplete(() => Destroy(shuriken1));
                // });
                // DOTween.Sequence().AppendInterval(.2f).AppendCallback(() =>
                // {
                //     PlayBloodEnemy();
                // });
                break;
            case ItemType.Saw:
                SoundController.Instance.PlayOnce(SoundType.HuggyAttackSaw);
                skeleton.Play("AttackSaw", false);
                DOTween.Sequence().AppendInterval(.1f).AppendCallback(() =>
                {
                    PlayBloodEnemy();
                });
                break;
            case ItemType.Shield:
                SoundController.Instance.PlayOnce(SoundType.HeroCut);
                skeleton.Play("AttackAxe", false);
                DOTween.Sequence().AppendInterval(.1f).AppendCallback(() =>
                {
                    PlayBloodEnemy();
                });
                break;
            case ItemType.Claws:
                SoundController.Instance.PlayOnce(SoundType.HuggyAttackClaws);
                DOTween.Sequence().AppendInterval(.2f).AppendCallback(() =>
                {
                    PlayHitEnemy();
                    PlayBloodEnemy();
                });

                DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                {
                    SoundController.Instance.PlayOnce(SoundType.HeroHit3);
                });
                skeleton.Play("AttackClaws", false);
                break;
            default:
                {
                    string[] attacks;
                    switch (EquipType)
                    {
                        case ItemType.Hammer:
                            DOTween.Sequence().AppendInterval(.2f).AppendCallback(() =>
                            {
                                SoundController.Instance.PlayOnce(SoundType.HuggyAttackBaseball);
                            });
                            attacks = new string[] { "AttackHammer" };
                            break;
                        case ItemType.Baseball:
                            SoundController.Instance.PlayOnce(SoundType.HuggyAttackBaseball);
                            attacks = new string[] { "AttackBaseball" };
                            break;
                        case ItemType.Axe:
                            {
                                SoundType[] soundTypes = { SoundType.HeroHit, SoundType.HeroHit2, SoundType.HeroHit3 };
                                SoundType soundType = soundTypes[UnityEngine.Random.Range(0, soundTypes.Length)];
                                SoundController.Instance.PlayOnce(soundType);
                                attacks = new string[] { "Attack", "Attack2" };
                                break;
                            }
                        default:
                            {
                                attacks = new string[] { "Attack", "Attack2" };
                                break;
                            }
                    }

                    string attack = attacks[UnityEngine.Random.Range(0, attacks.Length)];
                    if(attack == "Attack") SoundController.Instance.PlayOnce(SoundType.HuggyAttackNormal);
                    else SoundController.Instance.PlayOnce(SoundType.HuggyAttackNormal2);
                
                    skeleton.Play(attack, false);
                    if (hasBloodEnemy)
                    {
                        DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                        {
                            PlayHitEnemy();
                        });
                    }

                    switch (EquipType)
                    {
                        case ItemType.Axe:
                            {
                                float delay = .7f;
                                ParticleSystem particlePrefab = effectThunder1;
                                Vector3 offset = Vector3.zero;

                                if (attack == "AttackAxe2")
                                {
                                    delay = .3f;
                                    particlePrefab = effectThunder2;
                                    offset = new Vector3(0, .5f, 0);
                                }

                                DOTween.Sequence().AppendInterval(delay).AppendCallback(() =>
                                {
                                    ParticleSystem particle = Instantiate(particlePrefab);
                                    particle.transform.position = _target.transform.position + offset;

                                    if (attack == "AttackAxe")
                                    {
                                        SoundController.Instance.PlayOnce(SoundType.Axe1);
                                    }
                                    else
                                    {
                                        SoundController.Instance.PlayOnce(SoundType.Axe2);
                                    }
                                });
                                break;
                            }

                        case ItemType.Knife:
                            PlayBloodEnemy(attack);
                            break;
                    }

                    break;
                }
        }


        if (Data.VibrateState)
        {
            DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
            {
                Vibration.VibratePeek();
            });
        }
    }

    public void PLayMove(bool isLoop)
    {
        switch (EquipType)
        {
            case ItemType.Sword:
            case ItemType.Knife:
            case ItemType.Axe:
            case ItemType.Shuriken:
            case ItemType.SwordJapan:
            case ItemType.SwordBlood:
            case ItemType.Electric:
            case ItemType.Fire:
            case ItemType.Ice:
            case ItemType.Poison:
            case ItemType.Mace:
            case ItemType.Bow2:
            case ItemType.Polllaxe:
                //skeleton.Play("RunKiem", true);
                skeleton.Play("Run", true);
                break;
            default:
                skeleton.Play("Walk", true);
                break;
        }
    }

    public void PlayDead()
    {
        if (_target as EnemyIceDragon)
        {
            //skeleton.Play("DieIce", false);
            skeleton.Play("Die", false);
        }
        else if (_target as EnemyDragonHead)
        {
            //skeleton.Play("DieFire2", false);
            skeleton.Play("Die", false);
        }
        else
        {
            SoundController.Instance.PlayOnce(SoundType.HuggyDie);
            var dies = new string[] { "Die", "Die3" };
            string die = dies[UnityEngine.Random.Range(0, dies.Length)];
            //skeleton.Play(die, false);
            skeleton.Play("Die", false);
            IsDie3 = die == "Die3";
        }

        IsDie = true;
    }

    public void PlayWin(bool isLoop)
    {
        // string[] wins = { "Win", "Win2", "Win3" };
        // skeleton.Play(wins[UnityEngine.Random.Range(0, wins.Length)], true);
        // SoundController.Instance.PlayOnce(SoundType.HeroYeah);
        PlayIdle(true);
    }

    public void GiveFlower()
    {
        // Chưa có animation
        // if (Data.TimeToRescueParty.TotalMilliseconds > 0)
        // {
        //     skeleton.Play("GetMedal", true);
        // }
        // else
        // {
        //     if (Data.CurrentSkinHero == skinLess)
        //     {
        //         skeleton.Play("Sit2", true);
        //     }
        //     else
        //     {
        //         skeleton.Play("Sit", true);
        //     }
        // }
        PlayWin(true);
    }

    public void PlayLose(bool isLoop) { skeleton.Play("Die", true); }

    public void PlayUseItem(ItemType type)
    {
        switch (type)
        {
            case ItemType.Sword:
            case ItemType.Knife:
            case ItemType.Axe:
            case ItemType.Shuriken:
            case ItemType.SwordJapan:
            case ItemType.SwordBlood:
            case ItemType.Fire:
            case ItemType.Ice:
            case ItemType.Poison:
            case ItemType.Mace:
            case ItemType.Bow2:
            case ItemType.Polllaxe:
                skeleton.Play("PickUp", false);
                SoundController.Instance.PlayOnce(SoundType.PickWeapon);
                break;
            case ItemType.Gloves:
                skeleton.Play("PickUp", false);
                SoundController.Instance.PlayOnce(SoundType.PickWeapon);
                break;
            case ItemType.Food:
                skeleton.Play("PickUp", false);
                SoundController.Instance.PlayOnce(SoundType.PickWeapon);
                break;
            case ItemType.Shield:
                skeleton.Play("PickUp", false);
                SoundController.Instance.PlayOnce(SoundType.PickWeapon);
                break;
            case ItemType.Key:
                hasKey = true;
                skeleton.Play("PickUp", false);
                SoundController.Instance.PlayOnce(SoundType.PickKey);

                var _cacheItemTarget = _itemTarget;
                var itemLock = GameController.Instance.ItemLock;
                if (itemLock == null)
                {
                    DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                    {
                        _cacheItemTarget.gameObject.SetActive(false);
                    });
                }
                else
                {
                    DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
                    {
                        _cacheItemTarget.transform.DOMove(itemLock.LockPosition.transform.position, 1).OnComplete(() =>
                        {
                            _cacheItemTarget.gameObject.SetActive(false);
                            itemLock.PlayWin();
                        });
                    });
                }
                break;

            case ItemType.BrokenBrick:
                SoundController.Instance.PlayOnce(SoundType.HeroPushWall);
                //skeleton.Play("HitWall", false);
                DOTween.Sequence().AppendInterval(.4f).AppendCallback(() =>
                {
                    ParticleSystem effectHitWall = Instantiate(this.effectHitWall, transform.parent);
                    effectHitWall.transform.position = this.effectHitWall.transform.position;
                    effectHitWall.gameObject.SetActive(true);
                    effectHitWall.Play();
                });
                break;

            case ItemType.Electric:
                DOTween.Sequence().AppendInterval(.3f).AppendCallback(() =>
                    {
                        SoundController.Instance.PlayOnce(SoundType.ElectricTrap);
                        effectElectricHuggy.gameObject.SetActive(true);
                        skeleton.Play("LoseElectric", false);
                    });
                break;

            case ItemType.Trap:
                DOTween.Sequence().AppendInterval(.3f).AppendCallback(() =>
                {
                    skeleton.Play("LoseTrap", false);
                });
                break;
            case ItemType.Bomb:
                DOTween.Sequence().AppendInterval(.3f).AppendCallback(() =>
               {
                   skeleton.Play("LoseBomb", false);
               });
                break;
            case ItemType.Bow:
                break;
                DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                {
                    skeleton.Play("Die2", false);
                });
                break;
            default:
                {
                    switch (EquipType)
                    {
                        case ItemType.Sword:
                        case ItemType.Knife:
                        case ItemType.Axe:
                        case ItemType.Shuriken:
                        case ItemType.SwordJapan:
                        case ItemType.SwordBlood:
                        // case ItemType.Electric:
                        case ItemType.Fire:
                        case ItemType.Ice:
                        case ItemType.Poison:
                        case ItemType.Mace:
                        case ItemType.Bow2:
                        case ItemType.Polllaxe:
                            skeleton.Play("PickUp", false);
                            break;
                        default:
                            skeleton.Play("PickUp", false);
                            break;
                    }
                    break;
                }
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(Player))]
public class PlayerEditor : UnityEditor.Editor
{
    private Player _player;

    private void OnEnable() { _player = (Player)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _player.TxtDamage.text = _player.Damage.ToString();

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif