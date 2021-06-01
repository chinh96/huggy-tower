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

    public bool isUsingSword;

    [SerializeField] private ParticleSystem effectIncreaseDamge;
    [SerializeField] private ParticleSystem effectBlood;
    [SerializeField] private ParticleSystem effectBlood2;
    [SerializeField] private ParticleSystem effectBlood3;
    [SerializeField] private ParticleSystem effectHitWall;
    [SerializeField] private ParticleSystem effectPickSword;
    [SerializeField] private ParticleSystem effectHit;
    [SerializeField] private ParticleSystem effectFingerPress;

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
    private Vector2 _movement; // vector move player
    private RaycastHit2D _hitItem; // check hit item to stop moving

    private bool _isMouseUpDragDetected;
    private RoomTower _parentRoom;
    private bool _dragValidateRoomFlag;
    private string swordName = "";

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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public (bool, int) CheckCorrectArea()
    {
        bool check = false;
        int indexSlot = 0;
        var tower = GameController.Instance.Root.LevelMap.visitTower;
        for (int i = 0; i < tower.slots.Count; i++)
        {
            check = GetComponent<RectTransform>().Overlaps(tower.slots[i].GetComponent<RectTransform>());
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
            if (currentRoom != null && GameController.Instance.Root.LevelMap.visitTower.slots.Contains(currentRoom) && currentRoom.IsClearEnemyInRoom() &&
                !currentRoom.IsContaintItem())
            {
                cache = currentRoom;
            }

            transform.SetParent(_parentRoom.transform, false);
            transform.localPosition = _parentRoom.spawnPoint.localPosition;
            UpdateDefault();

            if (cache != null)
            {
                GameController.Instance.Root.LevelMap.visitTower.slots.Remove(cache);
                var fitter = GameController.Instance.Root.LevelMap.visitTower.fitter;
                cache.transform.DOScale(Vector3.zero, 0.5f)
                    .SetEase(Ease.OutQuad)
                    .OnUpdate(() =>
                    {
                        fitter.enabled = false;
                        // ReSharper disable once Unity.InefficientPropertyAccess
                        fitter.enabled = true;
                    })
                    .OnComplete(() => Destroy(cache.gameObject));
                var newRoom = Instantiate(GameController.Instance.RoomPrefab, GameController.Instance.Root.LevelMap.homeTower.transform, false);
                newRoom.transform.localScale = Vector3.zero;
                newRoom.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InQuad);
                GameController.Instance.Root.LevelMap.homeTower.slots.Add(newRoom);
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
            // display effect
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
        if (_countdownAttack <= 0)
        {
            SearchingTarget();
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
                        }
                        else
                        {
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
                            PlayUseItem(ItemType.None);
                            DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
                            {
                                (_target as Princess)?.PlayWin(true);
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
                        transform.DOLocalMoveX(_itemTarget.ItemType == ItemType.Sword ? 25 : 0, 0.5f).SetEase(Ease.Linear).OnComplete(() => UseItem());
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
                    Turn = ETurn.UsingItem;
                    PlayUseItem(_itemTarget.ItemType);
                    float timeDelay = _itemTarget.ItemType == ItemType.BrokenBrick ? .5f : 1.2f;
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

                    timeDelay = _itemTarget.ItemType == ItemType.Sword ? 0.2f : .5f;
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

    #region movement

    /// <summary>
    /// move to left
    /// </summary>
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

    /// <summary>
    /// move to right
    /// </summary>
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

    #endregion

    /// <summary>
    /// method call directly by event in anim attack
    /// </summary>
    private void OnAttackByEvent()
    {
        if (_target != null)
        {
            var cacheDamage = Damage;
            if (_flagAttack)
            {
                Damage += _target.Damage;
                if (damage > 0)
                {
                    effectIncreaseDamge.gameObject.SetActive(true);
                    effectIncreaseDamge.Play();
                }

                _target.OnBeingAttacked();
            }

            TxtDamage.DOCounter(cacheDamage, Damage, 0.5f).OnComplete(() => TxtDamage.text = Damage.ToString());
        }
    }

    public void IncreaseDamage(int damage)
    {
        var cacheDamage = Damage;
        if (damage > 0)
        {
            SoundController.Instance.PlayOnce(SoundType.HeroUpLevel);
            effectPickSword.gameObject.SetActive(true);
            effectPickSword.Play();
        }

        Damage += damage;
        TxtDamage.DOCounter(cacheDamage, Damage, 0.5f).OnComplete(() => TxtDamage.text = Damage.ToString());

        if (Damage <= 0)
        {
            SoundController.Instance.PlayOnce(SoundType.HeroDownLevel);
            State = EUnitState.Invalid;
            Turn = ETurn.None;
            PlayDead();
            GameController.Instance.OnLoseLevel();
        }
    }

    public void ChangeSword(string swordName = "")
    {
        if (swordName != "")
        {
            this.swordName = swordName;
        }

        if (this.swordName != "")
        {
            Skeleton.ChangeSword(this.swordName);
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
        if (GameController.Instance.Root.LevelMap.visitTower.IsClearTower() && GameController.Instance.Root.LevelMap.condition == ELevelCondition.KillAll)
        {
            PlayWin(true);
            GameController.Instance.OnWinLevel();
        }
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
        if (isUsingSword)
        {
            SoundController.Instance.PlayOnce(SoundType.HeroCut);
            string[] attacks = { "Attack", "AttackSword" };
            string attack = attacks[UnityEngine.Random.Range(0, attacks.Length)];
            skeleton.Play(attack, false);
            float timeDelay = attack == "Attack" ? .5f : .8f;
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
        else
        {
            SoundController.Instance.PlayOnce(SoundType.HeroHit);
            string[] attacks = { "Attack2", "AttackHit", "AttackHit2" };
            skeleton.Play(attacks[UnityEngine.Random.Range(0, attacks.Length)], false);
        }
    }

    public void PLayMove(bool isLoop)
    {
        if (isUsingSword)
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
                skeleton.Play("Pick", false);
                break;
            case ItemType.BrokenBrick:
                skeleton.Play("HitWall", false);
                DOTween.Sequence().AppendInterval(.4f).AppendCallback(() =>
                {
                    effectHitWall.gameObject.SetActive(true);
                    effectHitWall.Play();
                });
                break;
            default:
                if (isUsingSword)
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