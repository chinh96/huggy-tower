using Lance.Common;
using Spine.Unity;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Lance.TowerWar.Unit
{
    using LevelBase;

    public class EnemyMelee : Unit, IAnim
    {
        public SkeletonGraphic skeleton;
        public Rigidbody2D rigid;
        public Collider2D coll2D;
        public override EUnitType Type { get; protected set; } = EUnitType.Enemy;
        public override void DarknessRise() { }

        public override void LightReturn() { }

        public override void BeingAttacked(bool attack, int damage)
        {
            if (!attack)
            {
                PlayAttack();
                Timer.Register(1.16f, () => PlayIdle(true));
            }
            else
            {
                OnDead();
            }
        }

        public void OnDead()
        {
            State = EUnitState.Invalid;
            coll2D.enabled = false;
            rigid.simulated = false;
            TxtDamage.gameObject.SetActive(false);
            PlayDead();
        }

        public SkeletonGraphic Skeleton => skeleton;
        public void PlayIdle(bool isLoop) { skeleton.Play("idle", true); }

        public void PlayAttack() { skeleton.Play("attack", false); }

        public void PLayMove(bool isLoop) { skeleton.Play("run", true); }

        public void PlayDead() { skeleton.Play("die", false); }

        public void PlayWin(bool isLoop) { }

        public void PlayLose(bool isLoop) { }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EnemyMelee))]
    public class EnemyMeleeEditor : UnityEditor.Editor
    {
        private EnemyMelee _enemyMelee;

        private void OnEnable() { _enemyMelee = (EnemyMelee) target; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _enemyMelee.TxtDamage.text = _enemyMelee.damage.ToString();

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}