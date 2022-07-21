using System;
using Spine.Unity;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using DG.Tweening;
using Spine;
public class EnemyUmibozu : Unit, IAnim
{
    public SkeletonGraphic skeleton;
    public Rigidbody2D rigid;
    public Collider2D coll2D;
    public SpineAttackHandle attackHandle;
    public override EUnitType Type { get; protected set; } = EUnitType.Boss;

    private Action _callbackAttackPlayer;
    private bool isDead;

    private void Start()
    {
        attackHandle.Initialize(OnAttackByEvent, OnEndAttackByEvent);
    }

    public override void OnAttack(int damage, Action callback)
    {
        _callbackAttackPlayer = callback;
        PlayAttack();
    }

    public override void OnBeingAttacked() {
        isAttacking = false;
        isAttacked = true;
    }

    private void OnAttackByEvent() {
        _callbackAttackPlayer?.Invoke();
        GameController.Instance.Player?.Skeleton.Play("Die2", false);
        GameController.Instance.UpdateBlood(true);
    }

    private void OnEndAttackByEvent() {
        if(GameController.Instance.Player) GameController.Instance.Player.isAttacked = false;
        isAttacking = false;
        PlayIdle(true);
        GameController.Instance.Player?.PlayIdle(true);
    }

    public override void DarknessRise() { }

    public override void LightReturn() { }

    public void OnDead()
    {
        State = EUnitState.Invalid;
        coll2D.enabled = false;
        rigid.simulated = false;
        TxtDamage?.gameObject.SetActive(false);
        PlayDead();
    }

    public SkeletonGraphic Skeleton => skeleton;
    public void PlayIdle(bool isLoop) { skeleton.Play("Idle", true); }

    public void PlayAttack()
    {
        if (!isAttacking && !isAttacked)
        {
            GameController.Instance.Player?.OnBeingAttacked();
            isAttacking = true;
            skeleton.Play("Attack", false);
            SoundController.Instance.PlayOnce(SoundType.OctaviusAttack);
        }
    }

    public void PLayMove(bool isLoop) { skeleton.Play("Run", true); }

    public void PlayDead()
    {
        skeleton.Play("Die", false);
        isDead = true;
        SoundController.Instance.PlayOnce(SoundType.OctaviusDie);
    }

    public void PlayWin(bool isLoop) { }

    public void PlayLose(bool isLoop) { }

    public override void PlayHurt()
    {
        skeleton.Play("Hurt", false);
    }

    public override void PlayDie()
    {
        OnDead();
    }

    private int _cnt = 1;
    public override void PlayChainIdle()
    {
        string idleName = "Idle";
        if (_cnt == 4) idleName = "Idle3";
        else if (_cnt == 2) idleName = "Idle2";
        TrackEntry state = skeleton.AnimationState.SetAnimation(0, idleName, true);
        state.MixDuration = 0;
        state.Complete += HandleComplete;
        _cnt++;
    }

    void HandleComplete(TrackEntry trackEntry)
    {
        PlayChainIdle();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyUmibozu))]
public class EnemyUmibozuEditor : UnityEditor.Editor
{
    private EnemyUmibozu _enemy;

    private void OnEnable() { _enemy = (EnemyUmibozu)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _enemy.TxtDamage.text = _enemy.Damage.ToString();

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif