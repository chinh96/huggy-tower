using System.Collections.Generic;

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

        public bool CanGoBackHome { get; set; } = false;
        public bool FirstTurn { get; set; }
        public ETurn Turn { get; private set; } = ETurn.None;

        private Vector3 _defaultPosition;
        private float _countdownAttack = 0f;
        private List<Collider2D> _cachedSearchCollider = new List<Collider2D>();

        private void Start()
        {
            attackHandle.Initialize(Attack);
            UpdateDefaultPosition();
            StartMoveTurn();
        }

        public void UpdateDefaultPosition() { _defaultPosition = transform.localPosition; }

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
                    indexSlot = i;
                    break;
                }
            }

            return (check, indexSlot);
        }

        public void ResetPlayerState() { transform.localPosition = _defaultPosition; }

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
                UpdateDefaultPosition();
                if (!FirstTurn)
                {
                    FirstTurn = true;
                }

                StartAttackTurn();
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
            searchTargetCollider.OverlapCollider(new ContactFilter2D() {layerMask = searchTargetMark.value, useTriggers = true, useLayerMask = true}, _cachedSearchCollider);

            _cachedSearchCollider.RemoveAll(_ => _.gameObject.CompareTag("Ground") || _ == coll2D || _ == groundCollider);
            if (_cachedSearchCollider.Count == 0) return;
            float length = 1000;
            int index = 0;
            for (int i = 0; i < _cachedSearchCollider.Count; i++)
            {
                var coll = _cachedSearchCollider[i];
                float distance = Mathf.Abs(coll.transform.parent.position.x - transform.position.x);

                if (distance < length)
                {
                    var enemy = coll.GetComponentInParent<IEnemy>();
                    if (enemy is {State: EUnitState.Die})
                    {
                        continue;
                    }

                    length = distance;
                    index = i;
                }
            }

            var cacheCollider = _cachedSearchCollider[index];
            if (cacheCollider == coll2D) return;

            var localEnemy = cacheCollider.GetComponentInParent<IEnemy>();
            if (localEnemy is {State: EUnitState.Die}) return;

            if (localEnemy is {State: EUnitState.Normal} && _countdownAttack <= 0)
            {
                _countdownAttack = countdownAttack;
                
                PlayAttack();
                return;
            }
        }

        private void Attack()
        {
            Debug.Log("Attack enemy");

            StartMoveTurn();

            Timer.Register(0.6f, () => PlayIdle(true));
        }


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
}