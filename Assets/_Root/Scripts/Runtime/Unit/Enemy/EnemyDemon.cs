using System;
using Spine.Unity;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class EnemyDemon : Unit, IAnim
{
    public SkeletonGraphic skeleton;
    public Rigidbody2D rigid;
    public Collider2D coll2D;
    public SpineAttackHandle attackHandle;
    public override EUnitType Type { get; protected set; } = EUnitType.Enemy;

    private Action _callbackAttackPlayer;

    private void Start() { attackHandle.Initialize(OnAttackByEvent, OnEndAttackByEvent); }

    public override void OnAttack(int damage, Action callback)
    {
        _callbackAttackPlayer = callback;
        PlayAttack();
    }

    public override void OnBeingAttacked() { OnDead(); }

    /// <summary>
    /// call by event attack of anim attack
    /// </summary>
    private void OnAttackByEvent() { _callbackAttackPlayer?.Invoke(); }

    /// <summary>
    /// call by event end attack of anim attack
    /// </summary>
    private void OnEndAttackByEvent() { PlayIdle(true); }

    public override void DarknessRise() { }

    public override void LightReturn() { }

    public void OnDead()
    {
        State = EUnitState.Invalid;
        coll2D.enabled = false;
        rigid.simulated = false;
        TxtDamage.gameObject.SetActive(false);
        PlayDead();
    }

    public SkeletonGraphic Skeleton => skeleton;
    public void PlayIdle(bool isLoop) { skeleton.Play("Idle", true); }

    public void PlayAttack() { skeleton.Play("Attack", false); SoundController.Instance.PlayOnce(SoundType.EnemyHit); }

    public void PLayMove(bool isLoop) { skeleton.Play("Run", true); }

    public void PlayDead() { skeleton.Play("Die", false); SoundController.Instance.PlayOnce(SoundType.EnemyDie); }

    public void PlayWin(bool isLoop) { }

    public void PlayLose(bool isLoop) { }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyDemon))]
public class EnemyDemonEditor : UnityEditor.Editor
{
    private EnemyDemon _enemy;

    private void OnEnable() { _enemy = (EnemyDemon)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _enemy.TxtDamage.text = _enemy.Damage.ToString();

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif