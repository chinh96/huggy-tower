using System;
using Spine.Unity;
#if UNITY_EDITOR
using UnityEditor;

#endif

using UnityEngine;

public class EnemyGhost : Unit, IAnim
{
    public SkeletonGraphic skeleton;
    public Rigidbody2D rigid;
    public Collider2D coll2D;
    public SpineAttackHandle attackHandle;

    public override EUnitType Type { get; protected set; } = EUnitType.Enemy;
    private Action _callbackAttackPlayer;

    private void Start() { attackHandle.Initialize(OnAttackByEvent, OnEndAttackByEvent); }

    private void OnEndAttackByEvent() { PlayIdle(true); }

    private void OnAttackByEvent() { _callbackAttackPlayer?.Invoke(); }

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
    public void PlayIdle(bool isLoop) { skeleton.Play("Idle", isLoop); }

    public void PlayAttack() { skeleton.Play("Attack", false); SoundController.Instance.PlayOnce(SoundType.EnemyHit); }

    public void PLayMove(bool isLoop) { }

    public void PlayDead() { skeleton.Play("Die", false); }

    public void PlayWin(bool isLoop) { }

    public void PlayLose(bool isLoop) { }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyGhost))]
public class EnemyGhostEditor : UnityEditor.Editor
{
    private EnemyGhost _enemy;

    private void OnEnable() { _enemy = (EnemyGhost)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _enemy.TxtDamage.text = _enemy.Damage.ToString();

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif