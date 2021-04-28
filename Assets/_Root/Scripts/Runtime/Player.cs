using System.Collections.Generic;
using TMPro;
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
        [SerializeField] private LayerMask searchTargetMark;
        [SerializeField] private PlayerAttackHandle attackHandle;
        [SerializeField] private float countdownAttack = 1.25f;
        [SerializeField, ReadOnly] private ETurn turn = ETurn.None;

        public override EUnitType Type { get; protected set; } = EUnitType.Player;
        public bool FirstTurn { get; set; }
        public ETurn Turn { get => turn; private set => turn = value; }

        private Vector3 _defaultPosition;
        private RoomTower _defaultRoom = null;
        private float _countdownAttack = 0f;
        private List<Collider2D> _cachedSearchCollider = new List<Collider2D>();
        private Unit _enemyTarget;
        private bool _flagAttack;

        private void Start()
        {
            attackHandle.Initialize(Attack, OnEndAttack);
            UpdateDefaultPosition();
            StartMoveTurn();
        }

        public void UpdateDefaultPosition()
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
            for (int i = 0; i < tower.slots.Length; i++)
            {
                check = GetComponent<RectTransform>().Overlaps(tower.slots[i].GetComponent<RectTransform>());
                if (check)
                {
                    var isEmptyRoom = tower.slots[i].IsClearRoom();
                    if (isEmptyRoom) return (false, 0);
                    indexSlot = i;
                    break;
                }
            }

            return (check, indexSlot);
        }

        public void ResetPlayerState()
        {
            transform.SetParent(_defaultRoom.transform, false);
            transform.localPosition = _defaultPosition;
        }

        public void OnSelected()
        {
            rigid2D.gravityScale = 0;
            coll2D.isTrigger = true;
            coll2D.enabled = false;
            groundCollider.enabled = false;
            searchTargetCollider.enabled = false;
        }

        public void OnDeSelected()
        {
            rigid2D.gravityScale = 1;
            coll2D.isTrigger = false;
            coll2D.enabled = true;
            groundCollider.enabled = true;
            searchTargetCollider.enabled = true;
        }

        private void OnMouseDown()
        {
            if (Turn == ETurn.Move)
            {
                OnSelected();
                leanSelectableByFinger.SelfSelected = true;
            }
        }

        private void OnMouseUp()
        {
            var checkArea = CheckCorrectArea();

            if (checkArea.Item1)
            {
                var room = Gamemanager.Instance.Root.LevelMap.visitTower.slots[checkArea.Item2];
                transform.SetParent(room.transform, false);
                transform.localPosition = room.spawnPoint.localPosition;
                UpdateDefaultPosition();
                if (!FirstTurn) FirstTurn = true;

                Gamemanager.Instance.Root.LevelMap.visitTower.RefreshRoom();
                Gamemanager.Instance.Root.LevelMap.homeTower.RefreshRoom();
                Timer.Register(0.2f, StartAttackTurn);
                OnDeSelected();
                leanSelectableByFinger.Deselect();
            }
            else
            {
                ResetPlayerState();
                OnDeSelected();
                leanSelectableByFinger.Deselect();
                // display effect
            }
        }

        #region turn

        public void StartMoveTurn() { Turn = ETurn.Move; }

        public void StartAttackTurn() { Turn = ETurn.Attack; }

        public void StartAwaitTurn() { Turn = ETurn.None; }

        #endregion

        private void Update()
        {
            if (Gamemanager.Instance.GameState != EGameState.Playing) return;

            _countdownAttack = Mathf.Max(0, _countdownAttack - Time.deltaTime);
            if (_countdownAttack <= 0)
            {
                SearchingTarget();
            }
        }

        private void SearchingTarget()
        {
            if (Turn != ETurn.Attack) return;

            _cachedSearchCollider = new List<Collider2D>();
            searchTargetCollider.OverlapCollider(new ContactFilter2D() {layerMask = searchTargetMark.value, useTriggers = true, useLayerMask = true},
                _cachedSearchCollider);

            _cachedSearchCollider.RemoveAll(_ => _.gameObject.CompareTag("Ground") || _ == coll2D || _ == groundCollider);
            if (_cachedSearchCollider.Count == 0)
            {
                StartMoveTurn();
                return;
            }

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

            _enemyTarget = cacheCollider.GetComponentInParent<Unit>();
            if (_enemyTarget is {State: EUnitState.Invalid})
            {
                _enemyTarget = null;
                return;
            }

            if (_enemyTarget is {State: EUnitState.Normal} && _countdownAttack <= 0)
            {
                Turn = ETurn.Attacking;
                _countdownAttack = countdownAttack;

                // check damage
                _flagAttack = damage > _enemyTarget.Damage;
                if (_flagAttack)
                {
                    PlayAttack();
                    damage += _enemyTarget.Damage;
                }
                else
                {
                    damage = 0;
                }

                TxtDamage.text = damage.ToString();

                return;
            }
        }

        private void Attack()
        {
            if (_enemyTarget != null)
            {
                _enemyTarget.BeingAttacked(_flagAttack, damage);
            }
        }

        private void OnEndAttack()
        {
            StartMoveTurn();
            PlayIdle(true);
        }

        public override void BeingAttacked(bool attack, int damage) { }

        public override void DarknessRise() { }

        public override void LightReturn() { }
        public SkeletonGraphic Skeleton => skeleton;
        public void PlayIdle(bool isLoop) { skeleton.Play("idle", true); }

        public void PlayAttack() { skeleton.Play("attack (kiem)", false); }

        public void PLayMove(bool isLoop) { skeleton.Play("run", true); }

        public void PlayDead() { skeleton.Play("die", false); }

        public void PlayWin(bool isLoop) { skeleton.Play("win", true); }

        public void PlayLose(bool isLoop) { skeleton.Play("fail 1", true); }
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

            _player.TxtDamage.text = _player.damage.ToString();

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}