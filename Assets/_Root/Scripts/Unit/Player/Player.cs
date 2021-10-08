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
    [SerializeField] private GameObject shuriken;
    [SerializeField] private GameObject bow;

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
            check = tower.slots[i].GetComponent<RectTransform>().Contains(Input.mousePosition, Camera.main);
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
        coll2D.enabled = false;
        groundCollider.enabled = false;
        searchTargetCollider.enabled = false;
        GameController.Instance.SetSlicerActive(true);
    }

    public void OnDeSelected()
    {
        rigid2D.gravityScale = 1;
        coll2D.enabled = true;
        groundCollider.enabled = true;
        searchTargetCollider.enabled = true;
        GameController.Instance.SetSlicerActive(false);
    }

    public void OnMouseDown()
    {
        if (GameController.Instance.GameState != EGameState.Playing || Turn == ETurn.None)
        {
            return;
        }

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

    public void OnMouseUp()
    {
        if (GameController.Instance.GameState != EGameState.Playing || Turn == ETurn.None)
        {
            return;
        }

        SoundController.Instance.PlayOnce(SoundType.HeroDrop);
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
        }
        else
        {
            _isMouseUpDragDetected = false;
            ResetPlayerState();
            OnDeSelected();
            leanSelectableByFinger.Deselect();
        }
    }

    public void FlashToSlot(RoomTower parentRoom)
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

    public void StartSearchingTurn() { Turn = ETurn.Searching; }

    public void StartMoveToItemTurn() { Turn = ETurn.MoveToItem; }

    private void StartAwaitTurn() { Turn = ETurn.None; }

    #endregion

    private void Update()
    {
        if (GameController.Instance.GameState != EGameState.Playing && (Turn == ETurn.Drag || Turn == ETurn.None) || state == EUnitState.Invalid) return;

        _countdownAttack = Mathf.Max(0, _countdownAttack - Time.deltaTime);
        if (_countdownAttack <= 0 && !GameController.Instance.IsOnboarding)
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
        if (cacheCollider == coll2D) return;

        #endregion

        _target = cacheCollider.GetComponentInParent<Unit>();
        if (_target != null)
        {
            if (_target is { State: EUnitState.Invalid })
            {
                _target = null;
                return;
            }

            switch (_target.Type)
            {
                case EUnitType.Enemy:
                    if (_target is { State: EUnitState.Normal } && _countdownAttack <= 0)
                    {
                        Turn = ETurn.Attacking;
                        _countdownAttack = countdownAttack;

                        // check damage
                        _flagAttack = damage > _target.Damage;
                        if (_flagAttack)
                        {
                            hasBloodEnemy = true;
                            _target.OnAttack(damage, null);
                            if (_target as EnemyGoblin || _target as EnemyKappa)
                            {
                                DOTween.Sequence().AppendInterval(.3f).AppendCallback(() =>
                                {
                                    if (_target as EnemyKappa)
                                    {
                                        effectHitKappa.gameObject.SetActive(true);
                                        effectHitKappa.Play();
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
                                    SoundController.Instance.PlayOnce(SoundType.GoblinKappaAttack);
                                    DOTween.Sequence().AppendInterval(1.5f).AppendCallback(() =>
                                    {
                                        PlayAttack();
                                    });
                                });

                                var cacheDamage = Damage;
                                Damage -= _target.Damage;
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
                                PlayAttack();
                            }
                        }
                        else
                        {
                            hasBloodEnemy = false;
                            PlayAttack();
                            _target.OnAttack(damage, BeingAttackedCallback);
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
                            if (!hasKey && princess.LockObj != null)
                            {
                                Turn = ETurn.Drag;
                                PlayIdle(true);
                                AddJumpAnimation();
                                return;
                            }

                            PlayUseItem(ItemType.None);
                            DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
                            {
                                princess.LockObj?.DOFade(0, .3f);
                                princess.LockObj2?.DOFade(0, .3f);
                                princess.PlayWin(true);
                                DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
                                {
                                    GameController.Instance.OnWinLevel();
                                });
                                GiveFlower();
                            });
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
                DOTween.Kill(transform);

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
                        skeleton.Play("Teleport", false);
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
                    PlayUseItem(_itemTarget.EquipType);
                    float timeDelay = _itemTarget.EquipType == ItemType.Bow || _itemTarget.EquipType == ItemType.BrokenBrick || _itemTarget.EquipType == ItemType.Trap ? 1.2f : .5f;
                    DOTween.Sequence().AppendInterval(timeDelay).AppendCallback(() =>
                    {
                        if (levelMap.condition == condition)
                        {
                            switch (condition)
                            {
                                case ELevelCondition.CollectChest:
                                    if (_itemTarget as ItemChest != null)
                                    {
                                        DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
                                        {
                                            GameController.Instance.OnWinLevel();
                                        });
                                        PlayWin(true);
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
                        case ItemType.Sword:
                        case ItemType.SwordBlood:
                            timeDelay = .2f;
                            break;
                        case ItemType.Bow:
                            timeDelay = 0;
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
                _cacheTarget.OnBeingAttacked();

                if (_cacheTarget as EnemyGoblin || _cacheTarget as EnemyKappa)
                {
                    // Damage -= _cacheTarget.Damage;
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
            swordNames.Add(swordName);
        }

        if (swordNames.Count > 0)
        {
            Skeleton.ChangeSword(swordNames);
        }
    }

    private void OnEndAttackByEvent()
    {
        PlayIdle(true);

        if (_target as EnemyWolfGhost)
        {
            DOTween.Sequence().AppendInterval(.3f).AppendCallback(() =>
            {
                skeleton.Play("PickClaws", false);
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
        var room = levelMap.visitTower.RoomContainPlayer(this);
        if (room != null && (!room.IsClearEnemyInRoom() || room.IsContaintItem() || room.IsContaintPrincess()))
        {
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
            effectHitKappa.gameObject.SetActive(true);
            effectHitKappa.Play();
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
                skeleton.Play("Idle2", true);
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

        float timeDelay = attack == "AttackSword" ? .8f : .5f;
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
                if (_target as EnemyDragonHead)
                {
                    effectBlood.transform.localPosition += new Vector3(-250, 400, 0);
                }
                else if (_target as EnemyKraken)
                {
                    effectBlood.transform.localPosition += new Vector3(-200, 50, 0);
                }
                else if (_target as EnemyKraken2)
                {
                    effectBlood.transform.localPosition += new Vector3(60, -85, 0);
                }
                else if (_target as EnemyKraken3)
                {
                    effectBlood.transform.localPosition += new Vector3(100, 180, 0);
                }
                else if (_target as EnemyKraken4)
                {
                    effectBlood.transform.localPosition += new Vector3(20, -45, 0);
                }
                else if (_target as EnemyKraken5)
                {
                    effectBlood.transform.localPosition += new Vector3(200, -355, 0);
                }
                else if (_target as EnemyKraken6)
                {
                    effectBlood.transform.localPosition += new Vector3(100, -500, 0);
                }
                else
                {
                    effectBlood.transform.localPosition += new Vector3(0, 40, 0);
                }
                effectBlood.gameObject.SetActive(true);
                effectBlood.Play();
            });
        }
    }

    public void PlayAttack()
    {
        switch (EquipType)
        {
            case ItemType.SwordJapan:
                {
                    skeleton.Play("AttackKiemJapan", false);

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
                    skeleton.Play(attack, false);

                    SoundType[] soundTypes = { SoundType.HeroCut, SoundType.HeroCut2, SoundType.HeroCut3 };
                    SoundType soundType = soundTypes[UnityEngine.Random.Range(0, soundTypes.Length)];
                    float timeDelay = attack == "AttackSword" ? .5f : 0;
                    DOTween.Sequence().AppendInterval(timeDelay).AppendCallback(() =>
                    {
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
                    skeleton.Play("AttackElemental", false);
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
                skeleton.Play("AttackBow", false);
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
                skeleton.Play("AttackPollaxe", false);
                SoundController.Instance.PlayOnce(SoundType.Pollaxe);
                PlayBloodEnemy();
                break;
            case ItemType.Mace:
                {
                    skeleton.Play("AttackMace", false);
                    SoundController.Instance.PlayOnce(SoundType.Mace);
                    break;
                }
            case ItemType.SwordBlood:
                skeleton.Play("AttackSword3", false);
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
                skeleton.Play("Shuriken", false);
                SoundController.Instance.PlayOnce(SoundType.Knife);
                DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                {
                    GameObject shuriken1 = Instantiate(shuriken, transform.parent);
                    shuriken1.transform.position = transform.position + new Vector3(1, 1, 0);
                    Vector3 endValue = shuriken1.transform.position + new Vector3(5, 0, 0);
                    shuriken1.transform.DOMove(endValue, 1f).OnComplete(() => Destroy(shuriken1));
                });
                PlayBloodEnemy();
                break;
            default:
                {
                    string[] attacks;
                    switch (EquipType)
                    {
                        case ItemType.Claws:
                            DOTween.Sequence().AppendInterval(.3f).AppendCallback(() =>
                            {
                                PlayBloodEnemy();
                            });

                            DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                            {
                                SoundController.Instance.PlayOnce(SoundType.HeroHit3);
                            });
                            attacks = new string[] { "AttackClaws2" };
                            break;
                        case ItemType.Gloves:
                            DOTween.Sequence().AppendInterval(.2f).AppendCallback(() =>
                            {
                                SoundController.Instance.PlayOnce(SoundType.Gloves);
                            });
                            attacks = new string[] { "AttackGlove2" };
                            break;
                        case ItemType.Knife:
                            SoundController.Instance.PlayOnce(SoundType.Knife);
                            attacks = new string[] { "AttackKnife", "AttackKnife2" };
                            break;
                        case ItemType.Axe:
                            {
                                SoundType[] soundTypes = { SoundType.HeroHit, SoundType.HeroHit2, SoundType.HeroHit3 };
                                SoundType soundType = soundTypes[UnityEngine.Random.Range(0, soundTypes.Length)];
                                SoundController.Instance.PlayOnce(soundType);
                                attacks = new string[] { "AttackAxe", "AttackAxe2" };
                                break;
                            }
                        default:
                            {
                                SoundType[] soundTypes = { SoundType.HeroHit, SoundType.HeroHit2, SoundType.HeroHit3 };
                                SoundType soundType = soundTypes[UnityEngine.Random.Range(0, soundTypes.Length)];
                                SoundController.Instance.PlayOnce(soundType);
                                attacks = new string[] { "AttackHit", "AttackHit2" };
                                if (hasBloodEnemy)
                                {
                                    DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                                    {
                                        ParticleSystem hitEnemy = Instantiate(effectHitEnemy, transform.parent);
                                        hitEnemy.transform.position = _target.transform.position;
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

                                    });
                                }
                                break;
                            }
                    }

                    string attack = attacks[UnityEngine.Random.Range(0, attacks.Length)];
                    skeleton.Play(attack, false);

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
                skeleton.Play("RunKiem", true);
                break;
            default:
                skeleton.Play("Run", true);
                break;
        }
    }

    public void PlayDead()
    {
        if (_target as EnemyIceDragon)
        {
            skeleton.Play("DieIce", false);
        }
        else if (_target as EnemyDragonHead)
        {
            skeleton.Play("DieFire2", false);
        }
        else
        {
            SoundController.Instance.PlayOnce(SoundType.HeroDie);
            var dies = new string[] { "Die", "Die3" };
            string die = dies[UnityEngine.Random.Range(0, dies.Length)];
            skeleton.Play(die, false);
            IsDie3 = die == "Die3";
        }

        IsDie = true;
    }

    public void PlayWin(bool isLoop)
    {
        string[] wins = { "Win", "Win2", "Win3" };
        skeleton.Play(wins[UnityEngine.Random.Range(0, wins.Length)], true);
        SoundController.Instance.PlayOnce(SoundType.HeroYeah);
    }

    public void GiveFlower()
    {
        if (Data.TimeToRescueParty.TotalMilliseconds > 0)
        {
            skeleton.Play("GetMedal", true);
        }
        else
        {
            if (Data.CurrentSkinHero == skinLess)
            {
                skeleton.Play("Sit2", true);
            }
            else
            {
                skeleton.Play("Sit", true);
            }
        }
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
            case ItemType.Electric:
            case ItemType.Fire:
            case ItemType.Ice:
            case ItemType.Poison:
            case ItemType.Mace:
            case ItemType.Bow2:
            case ItemType.Polllaxe:
                skeleton.Play("Pick", false);
                SoundController.Instance.PlayOnce(SoundType.HeroPickSword);
                break;
            case ItemType.Gloves:
                skeleton.Play("Pick", false);
                SoundController.Instance.PlayOnce(SoundType.PickGloves);
                break;
            case ItemType.Food:
                skeleton.Play("Pick", false);
                SoundController.Instance.PlayOnce(SoundType.PickFood);
                break;
            case ItemType.Shield:
                skeleton.Play("Pick", false);
                SoundController.Instance.PlayOnce(SoundType.PickShield);
                break;
            case ItemType.Key:
                hasKey = true;
                skeleton.Play("Pick", false);
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
                skeleton.Play("HitWall", false);
                DOTween.Sequence().AppendInterval(.4f).AppendCallback(() =>
                {
                    ParticleSystem effectHitWall = Instantiate(this.effectHitWall, transform.parent);
                    effectHitWall.transform.position = this.effectHitWall.transform.position;
                    effectHitWall.gameObject.SetActive(true);
                    effectHitWall.Play();
                });
                break;
            case ItemType.Trap:
                DOTween.Sequence().AppendInterval(.3f).AppendCallback(() =>
                {
                    skeleton.Play("Die2", false);
                });
                break;
            case ItemType.Bomb:
                skeleton.Play("DieFire", false);
                break;
            case ItemType.Bow:
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
                        case ItemType.Electric:
                        case ItemType.Fire:
                        case ItemType.Ice:
                        case ItemType.Poison:
                        case ItemType.Mace:
                        case ItemType.Bow2:
                        case ItemType.Polllaxe:
                            skeleton.Play("Open1", false);
                            break;
                        default:
                            skeleton.Play("Open2", false);
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