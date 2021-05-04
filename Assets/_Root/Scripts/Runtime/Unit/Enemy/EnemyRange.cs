using System;
using Lance.Common;
using Spine.Unity;
using TMPro;
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
           // attackHandle.Initialize(OnAttackByEvent, OnEndAttackByEvent);
        }

        private void OnAttackByEvent()
        {
            
        }

        public override void OnBeingAttacked() {  }

        public override void OnAttack(int damage, Action callback) {  }

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
}