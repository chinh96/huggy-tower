using System;
using Spine.Unity;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using DG.Tweening;

public class EnemyWendigo : Unit, IAnim
{
    public SkeletonGraphic skeleton;
    public Rigidbody2D rigid;
    public Collider2D coll2D;
    public SpineAttackHandle attackHandle;
    public override EUnitType Type { get; protected set; } = EUnitType.Boss;

    private Action _callbackAttackPlayer;

    private void Start()
    {
        attackHandle.Initialize(OnAttackByEvent, OnEndAttackByEvent);
        DOTween.Sequence().AppendInterval(UnityEngine.Random.Range(0, .5f)).AppendCallback(() =>
        {
            skeleton.Play("Idle3", true);
        });
    }

    public override void OnAttack(int damage, Action callback)
    {
        _callbackAttackPlayer = callback;
        PlayAttack();
    }

    public override void OnBeingAttacked() {
        isAttacking = false;
        isAttacked = true;
        //OnDead(); 
    }

    private void OnAttackByEvent() {
        _callbackAttackPlayer?.Invoke();
        GameController.Instance.Player.Skeleton.Play("Die2", false);
        GameController.Instance.UpdateBlood(true);
    }

    private void OnEndAttackByEvent() {
        GameController.Instance.Player.isAttacked = false;
        isAttacking = false;
        PlayIdle(true);
        GameController.Instance.Player.PlayIdle(true);
    }

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

    public void PlayAttack() {
        if (!isAttacking && !isAttacked)
        {
            GameController.Instance.Player.OnBeingAttacked();
            isAttacking = true;
            skeleton.Play("Attack", false);
            SoundController.Instance.PlayOnce(SoundType.BossAttack);
        }
    }

    public void PLayMove(bool isLoop) { skeleton.Play("Run", true); }

    public void PlayDead()
    {
        skeleton.Play("Die", false);
    }

    public void PlayWin(bool isLoop) { }

    public void PlayLose(bool isLoop) { }
    public override void PlayHurt()
    {
        skeleton.Play("Hurt", false);
    }
    public override void PlayDie()
    {
        skeleton.Play("Die", false);
        SoundController.Instance.PlayOnce(SoundType.DragonDie);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyWendigo))]
public class EnemyWendigoEditor : UnityEditor.Editor
{
    private EnemyWendigo _enemy;

    private void OnEnable() { _enemy = (EnemyWendigo)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _enemy.TxtDamage.text = _enemy.Damage.ToString();

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif