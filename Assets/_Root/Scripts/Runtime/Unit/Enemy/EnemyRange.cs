using System;
using Lance.Common;
using Spine.Unity;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Lance.TowerWar.Unit
{
    using LevelBase;

    public class EnemyRange : Unit, IAnim
    {
        public SkeletonGraphic skeleton;
        public Rigidbody2D rigid;
        public Collider2D coll2D;
        public SpineAttackHandle attackHandle;
        public override EUnitType Type { get; protected set; } = EUnitType.Enemy;

        private Action _callbackAttackPlayer;

        private void Start()
        {
            attackHandle.Initialize(OnAttackByEvent, OnEndAttackByEvent);
        }

        private void OnEndAttackByEvent()
        {
            PlayIdle(true);
        }

        private void OnAttackByEvent()
        {
            _callbackAttackPlayer?.Invoke();
        }

        public override void OnBeingAttacked() { OnDead(); }

        public override void OnAttack(int damage, Action callback)
        {
            _callbackAttackPlayer = callback;
            PlayAttack();
        }
        
        public void OnDead()
        {
            State = EUnitState.Invalid;
            coll2D.enabled = false;
            rigid.simulated = false;
            TxtDamage.gameObject.SetActive(false);
            PlayDead();
        }

        public override void DarknessRise() { }

        public override void LightReturn() { }
        
        public SkeletonGraphic Skeleton => skeleton;
        public void PlayIdle(bool isLoop) { skeleton.Play("idle", true); }

        public void PlayAttack() { skeleton.Play("attack (cung)", false); }

        public void PLayMove(bool isLoop) { skeleton.Play("run", true); }

        public void PlayDead() { skeleton.Play("die", false); }

        public void PlayWin(bool isLoop) { }

        public void PlayLose(bool isLoop) { }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(EnemyRange))]
    public class EnemyRangeEditor : UnityEditor.Editor
    {
        private EnemyRange _enemyRange;

        private void OnEnable() { _enemyRange = (EnemyRange) target; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _enemyRange.TxtDamage.text = _enemyRange.damage.ToString();

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}