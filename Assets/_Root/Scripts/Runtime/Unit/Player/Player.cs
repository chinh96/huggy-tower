using System;
using System.Collections.Generic;
using DG.Tweening;
using Lance.TowerWar.Helper;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Lance.TowerWar.Unit
{
    using Common;
    using Controller;
    using LevelBase;
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
        [SerializeField, ReadOnly] private ETurn turn = ETurn.None;
        [SerializeField] private MixAndMatchSkin mixAndMatchSkin;

        [ReadOnly] public bool isUsingSword;

        public override EUnitType Type { get; protected set; } = EUnitType.Player;
        public bool FirstTurn { get; set; }
        public ETurn Turn { get => turn; private set => turn = value; }
        public MixAndMatchSkin MixAndMatchSkin => mixAndMatchSkin;

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
            var tower = Gamemanager.Instance.Root.LevelMap.visitTower;
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

                    var temp = Gamemanager.Instance.Root.LevelMap.visitTower.slots[result.Item2];
                    temp.UpdateStatusSelectRoom(true, true);
                }
            }
            else
            {
                if (_dragValidateRoomFlag)
                {
                    _dragValidateRoomFlag = false;
                    Gamemanager.Instance.Root.LevelMap.ResetSelectVisitTower();
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
        }

        public void OnDeSelected()
        {
            rigid2D.gravityScale = 1;
            coll2D.enabled = true;
            groundCollider.enabled = true;
            searchTargetCollider.enabled = true;
        }

        private void OnMouseDown()
        {
            _isMouseUpDragDetected = false;
            if (Turn == ETurn.Drag)
            {
                OnSelected();
                leanSelectableByFinger.SelfSelected = true;
            }
            // else
            // {
            //     OnDeSelected();
            //     leanSelectableByFinger.SelfSelected = false;
            // }
        }

        private void OnMouseUp()
        {
            if (!dragTranslate.DragTranslateFlag)
            {
                _isMouseUpDragDetected = false;
                OnDeSelected();
                leanSelectableByFinger.Deselect();
                return;
            }

            dragTranslate.DragTranslateFlag = false;
            Gamemanager.Instance.Root.LevelMap.ResetSelectVisitTower();
            var checkArea = CheckCorrectArea();
            if (checkArea.Item1)
            {
                RoomTower cache = null;
                _parentRoom = Gamemanager.Instance.Root.LevelMap.visitTower.slots[checkArea.Item2];
                var currentRoom = transform.parent.GetComponent<RoomTower>();
                if (currentRoom != null && Gamemanager.Instance.Root.LevelMap.visitTower.slots.Contains(currentRoom) && currentRoom.IsClearEnemyInRoom() &&
                    !currentRoom.IsContaintItem())
                {
                    cache = currentRoom;
                }

                transform.SetParent(_parentRoom.transform, false);
                transform.localPosition = _parentRoom.spawnPoint.localPosition;
                UpdateDefault();

                if (cache != null)
                {
                    Gamemanager.Instance.Root.LevelMap.visitTower.slots.Remove(cache);
                    var fitter = Gamemanager.Instance.Root.LevelMap.visitTower.fitter;
                    cache.transform.DOScale(Vector3.zero, 0.5f)
                        .SetEase(Ease.OutQuad)
                        .OnUpdate(() =>
                        {
                            fitter.enabled = false;
                            // ReSharper disable once Unity.InefficientPropertyAccess
                            fitter.enabled = true;
                        })
                        .OnComplete(() => cache.gameObject.Destroy());
                    var newRoom = Instantiate(Gamemanager.Instance.RoomPrefab, Gamemanager.Instance.Root.LevelMap.homeTower.transform, false);
                    newRoom.transform.localScale = Vector3.zero;
                    newRoom.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InQuad);
                    Gamemanager.Instance.Root.LevelMap.homeTower.slots.Add(newRoom);
                }

                if (!FirstTurn) FirstTurn = true;

                Gamemanager.Instance.Root.LevelMap.visitTower.RefreshRoom();
                Gamemanager.Instance.Root.LevelMap.homeTower.RefreshRoom();
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
            if (Gamemanager.Instance.GameState != EGameState.Playing && (Turn == ETurn.Drag || Turn == ETurn.None) || state == EUnitState.Invalid) return;

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
            searchTargetCollider.OverlapCollider(new ContactFilter2D() {layerMask = searchTargetMark.value, useTriggers = true, useLayerMask = true},
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
                    if (unit is {State: EUnitState.Invalid})
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
                if (_target is {State: EUnitState.Invalid})
                {
                    _target = null;
                    return;
                }

                switch (_target.Type)
                {
                    case EUnitType.Enemy:
                        if (_target is {State: EUnitState.Normal} && _countdownAttack <= 0)
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
                        if (_target is {State: EUnitState.Normal} && _countdownAttack <= 0)
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
                                PlayUseItem();
                                (_target as Princess)?.PlayWin(true);
                                Timer.Register(1f,
                                    () =>
                                    {
                                        Timer.Register(1f, () => { Gamemanager.Instance.OnWinLevel(); });
                                        PlayWin(true);
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
                        if (distance >= 80)
                        {
                            PLayMove(true);
                            transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear).OnComplete(() => UseItem());
                        }
                        else
                        {
                            UseItem();
                        }
                    }
                    else if (_itemTarget.Type == EUnitType.Gem)
                    {
                        var distance = Math.Abs((_itemTarget.transform.localPosition.x - cacheCollider.transform.localPosition.x));
                        if (distance >= 80)
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
                        PlayUseItem();
                        Timer.Register(1.2f,
                            () =>
                            {
                                if (Gamemanager.Instance.Root.LevelMap.condition == condition)
                                {
                                    Timer.Register(1f, () => { Gamemanager.Instance.OnWinLevel(); });
                                    PlayWin(true);
                                }
                                else
                                {
                                    StartSearchingTurn();
                                    PlayIdle(true);
                                }
                            });
                        Timer.Register(0.5f, () => _itemTarget.Collect(this));
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
            return Gamemanager.Instance.GameState == EGameState.Lose || Gamemanager.Instance.GameState == EGameState.Win || Turn == ETurn.UsingItem ||
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
                    _target.OnBeingAttacked();
                }

                TxtDamage.DOCounter(cacheDamage, Damage, 0.5f).OnComplete(() => TxtDamage.text = Damage.ToString());
            }
        }

        public void IncreaseDamage(int damage)
        {
            var cacheDamage = Damage;
            Damage += damage;
            TxtDamage.DOCounter(cacheDamage, Damage, 0.5f).OnComplete(() => TxtDamage.text = Damage.ToString());
        }

        /// <summary>
        /// method call directly by event in anim attack
        /// </summary>
        private void OnEndAttackByEvent()
        {
            PlayIdle(true);

            var room = Gamemanager.Instance.Root.LevelMap.visitTower.RoomContainPlayer(this);
            if (room != null && !room.IsClearEnemyInRoom() || room.IsContaintItem())
            {
                StartSearchingTurn();
                SearchingTarget();
                return;
            }

            StartDragTurn();
            if (Gamemanager.Instance.Root.LevelMap.visitTower.IsClearTower() && Gamemanager.Instance.Root.LevelMap.condition == ELevelCondition.KillAll)
            {
                Gamemanager.Instance.OnWinLevel();
            }
        }

        private void BeingAttackedCallback()
        {
            var cacheDamage = damage;
            damage = 0;
            TxtDamage.DOCounter(cacheDamage, damage, 0.5f).OnComplete(() => TxtDamage.text = damage.ToString());
            State = EUnitState.Invalid;
            PlayDead();

            Timer.Register(0.6f, Gamemanager.Instance.OnLoseLevel);
        }

        private void CollectChest() { }

        private void OnTriggerEnter2D(Collider2D other) { }

        public override void OnAttack(int damage, Action callback) { }
        public override void OnBeingAttacked() { }

        public override void DarknessRise() { }

        public override void LightReturn() { }
        public SkeletonGraphic Skeleton => skeleton;
        public void PlayIdle(bool isLoop) { skeleton.Play("Idle", true); }

        public void PlayAttack()
        {
            if (isUsingSword)
            {
                skeleton.Play("Attack", false);
            }
            else
            {
                skeleton.Play("Attack2", false);
            }
        }

        public void PLayMove(bool isLoop) { skeleton.Play("RunKiem", true); }

        public void PlayDead() { skeleton.Play("Die", false); }

        public void PlayWin(bool isLoop) { skeleton.Play("Win", true); }

        public void PlayLose(bool isLoop) { skeleton.Play("Die", true); }

        public void PlayUseItem() { skeleton.Play("Open1", false); }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(Player))]
    public class PlayerEditor : UnityEditor.Editor
    {
        private Player _player;

        private void OnEnable() { _player = (Player) target; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _player.TxtDamage.text = _player.Damage.ToString();

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}