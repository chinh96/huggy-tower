using System;
using System.Collections.Generic;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Lean.Touch;
using UnityEngine;
using Spine.Unity;

public class Player : Unit, IAnim
{
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

    public ItemType EquipType;

    [SerializeField] private ParticleSystem effectIncreaseDamge;
    [SerializeField] private ParticleSystem effectBlood;
    [SerializeField] private ParticleSystem effectBlood2;
    [SerializeField] private ParticleSystem effectBlood3;
    [SerializeField] private ParticleSystem effectHitWall;
    [SerializeField] private ParticleSystem effectPickSword;
    [SerializeField] private ParticleSystem effectHit;
    [SerializeField] private ParticleSystem effectFingerPress;
    [SerializeField] private ParticleSystem effectThunder1;
    [SerializeField] private ParticleSystem effectThunder2;
    [SerializeField] private ParticleSystem effectBomb;
    [SerializeField] private GameObject shuriken;

    public override EUnitType Type { get; protected set; } = EUnitType.Hero;
    public bool FirstTurn { get; set; }
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
        var tower = GameController.Instance.Root.LevelMap.visitTower;
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

                var temp = GameController.Instance.Root.LevelMap.visitTower.slots[result.Item2];
                temp.UpdateStatusSelectRoom(true, true);
            }
        }
        else
        {
            if (_dragValidateRoomFlag)
            {
                _dragValidateRoomFlag = false;
                GameController.Instance.Root.LevelMap.ResetSelectVisitTower();
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

    private void OnMouseDown()
    {
        if (GameController.Instance.GameState != EGameState.Playing)
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

    private void OnMouseUp()
    {
        if (GameController.Instance.GameState != EGameState.Playing)
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
        GameController.Instance.Root.LevelMap.ResetSelectVisitTower();
        var checkArea = CheckCorrectArea();
        if (checkArea.Item1)
        {
            RoomTower cache = null;
            _parentRoom = GameController.Instance.Root.LevelMap.visitTower.slots[checkArea.Item2];
            var currentRoom = transform.parent.GetComponent<RoomTower>();
            if (currentRoom != null && GameController.Instance.Root.LevelMap.visitTower.slots.Contains(currentRoom) && currentRoom.IsClearEnemyInRoom() && !currentRoom.IsContaintItem() && !currentRoom.IsContaintPrincess())
            {
                cache = currentRoom;
            }

            transform.SetParent(_parentRoom.transform, false);
            transform.localPosition = _parentRoom.spawnPoint.localPosition;
            UpdateDefault();

            if (cache != null)
            {
                GameController.Instance.Root.LevelMap.visitTower.RemoveSlot(cache);

                GameController.Instance.Root.LevelMap.homeTower.AddSlot();
            }

            if (!FirstTurn) FirstTurn = true;

            GameController.Instance.Root.LevelMap.visitTower.RefreshRoom();
            GameController.Instance.Root.LevelMap.homeTower.RefreshRoom();
            OnDeSelected();
            leanSelectableByFinger.Deselect();

            _isMouseUpDragDetected = true;
        }
        else
        {
            _isMouseUpDragDetected = false;
            ResetPlayerState();
            OnDeSelected();
            leanSelectableByFinger.Deselect();
        }
    }

    #region turn

    public void StartDragTurn()
    {
        Turn = ETurn.Drag;
        _countdownAttack = countdownAttack;
    }

    private void StartSearchingTurn() { Turn = ETurn.Searching; }

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
        foreach (var slot in GameController.Instance.Root.LevelMap.visitTower.slots)
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
                            PlayAttack();
                            _target.OnAttack(damage, null);
                        }
                        else
                        {
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

                if (_itemTarget.Type == EUnitType.Item)
                {
                    var distance = Math.Abs((_itemTarget.transform.localPosition.x - transform.localPosition.x));
                    if (distance >= 110)
                    {
                        PLayMove(true);
                        float endValue = 0;
                        switch (_itemTarget.EquipType)
                        {
                            case ItemType.Sword:
                                endValue = 25;
                                break;
                            case ItemType.BrokenBrick:
                                endValue = -40;
                                break;
                        }
                        transform.DOLocalMoveX(endValue, 0.5f).SetEase(Ease.Linear).OnComplete(() => UseItem());
                    }
                    else
                    {
                        UseItem();
                    }
                }
                else if (_itemTarget.Type == EUnitType.Gem)
                {
                    var distance = Math.Abs((_itemTarget.transform.localPosition.x - cacheCollider.transform.localPosition.x));
                    if (distance >= 110)
                    {
                        PLayMove(true);
                        transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear).OnComplete(() => UseItem(ELevelCondition.CollectGold));
                    }
                    else
                    {
                        UseItem(ELevelCondition.CollectGold);
                    }
                }

                void UseItem(ELevelCondition condition = ELevelCondition.CollectChest)
                {
                    if (!hasKey && _itemTarget as ItemChest != null)
                    {
                        Turn = ETurn.Drag;
                        PlayIdle(true);
                        AddJumpAnimation();
                        return;
                    }

                    Turn = ETurn.UsingItem;
                    PlayUseItem(_itemTarget.EquipType);
                    float timeDelay = _itemTarget.EquipType == ItemType.BrokenBrick ? .5f : 1.2f;
                    DOTween.Sequence().AppendInterval(timeDelay).AppendCallback(() =>
                    {
                        if (GameController.Instance.Root.LevelMap.condition == condition)
                        {
                            if (condition == ELevelCondition.CollectChest)
                            {
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
                                    StartSearchingTurn();
                                    PlayIdle(true);
                                }

                            }
                            else
                            {
                                DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
                                {
                                    GameController.Instance.OnWinLevel();
                                });
                                PlayWin(true);
                            }
                        }
                        else
                        {
                            StartSearchingTurn();
                            PlayIdle(true);
                        }
                    });

                    timeDelay = .5f;
                    switch (_itemTarget.EquipType)
                    {
                        case ItemType.Sword:
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

                if (_cacheTarget as EnemyGoblin)
                {
                    Damage -= _cacheTarget.Damage;
                }
                else
                {
                    Damage += _cacheTarget.Damage;
                }

                effectIncreaseDamge.gameObject.SetActive(true);
                effectIncreaseDamge.Play();

                _cacheTarget.TxtDamage.gameObject.SetActive(true);
                _cacheTarget.TxtDamage.transform.DOMove(TxtDamage.transform.position, .5f).SetEase(Ease.InCubic).OnComplete(() =>
                {
                    TxtDamage.transform.DOPunchScale(Vector3.one * 1.1f, .3f, 0);
                    TxtDamage.DOCounter(cacheDamage, Damage, 0);
                    _cacheTarget.TxtDamage.gameObject.SetActive(false);
                });
            }

            if (_target as EnemyGoblin)
            {
                effectBomb.gameObject.SetActive(true);
                effectBomb.Play();
                SoundController.Instance.PlayOnce(SoundType.BombGoblin);
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

        var room = GameController.Instance.Root.LevelMap.visitTower.RoomContainPlayer(this);
        if (room != null && !room.IsClearEnemyInRoom() || room.IsContaintItem())
        {
            StartSearchingTurn();
            SearchingTarget();
            return;
        }

        StartDragTurn();
        if (GameController.Instance.Root.LevelMap.visitTower.IsClearTower() && IsWinCondition(GameController.Instance.Root.LevelMap.condition))
        {
            PlayWin(true);
            GameController.Instance.OnWinLevel();
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
        effectHit.gameObject.SetActive(true);
        effectHit.Play();
        var cacheDamage = damage;
        damage = 0;
        TxtDamage.DOCounter(cacheDamage, damage, 0.5f).OnComplete(() => TxtDamage.text = damage.ToString());
        State = EUnitState.Invalid;
        PlayDead();

        DOTween.Sequence().AppendInterval(.6f).AppendCallback(() =>
        {
            GameController.Instance.OnLoseLevel();
        });
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
        if (GameController.Instance.GameState != EGameState.Lose)
        {
            skeleton.Play("Idle", true);
        }
    }

    public void PlayAttack()
    {
        void PlayBloodEnemy(string attack = "")
        {
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
                    effectBlood.transform.localPosition += new Vector3(0, 40, 0);
                    effectBlood.gameObject.SetActive(true);
                    effectBlood.Play();
                });
            }
        }
        switch (EquipType)
        {
            case ItemType.Sword:
            case ItemType.SwordJapan:
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
                                attacks = new string[] { "AttackHit", "AttackHit2" };
                                break;
                            }
                    }

                    string attack = attacks[UnityEngine.Random.Range(0, attacks.Length)];
                    skeleton.Play(attack, false);

                    if (EquipType == ItemType.Axe)
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
                    }

                    break;
                }
        }

        DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
        {
            Vibration.Vibrate();
        });
    }

    public void PLayMove(bool isLoop)
    {
        if (EquipType == ItemType.Sword)
        {
            skeleton.Play("RunKiem", true);
        }
        else
        {
            skeleton.Play("Run", true);
        }
    }

    public void PlayDead()
    {
        SoundController.Instance.PlayOnce(SoundType.HeroDie);
        skeleton.Play("Die", false);
    }

    public void PlayWin(bool isLoop)
    {
        string[] wins = { "Win", "Win2" };
        skeleton.Play(wins[UnityEngine.Random.Range(0, wins.Length)], true);
        SoundController.Instance.PlayOnce(SoundType.HeroYeah);
    }

    public void GiveFlower()
    {
        skeleton.Play("Sit", true);
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
                skeleton.Play("Die2", false);
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
                if (EquipType == ItemType.Sword)
                {
                    skeleton.Play("Open1", false);
                }
                else
                {
                    skeleton.Play("Open2", false);
                }
                break;
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