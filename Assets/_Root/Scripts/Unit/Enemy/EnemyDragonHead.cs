using System;
using Spine.Unity;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using DG.Tweening;
using Spine;

public class EnemyDragonHead : Unit, IAnim
{
    public SkeletonGraphic skeleton;
    public Collider2D coll2D;
    public SpineAttackHandle attackHandle;
    public override EUnitType Type { get; protected set; } = EUnitType.Boss;
    public ParticleSystem fire;

    private Action _callbackAttackPlayer;
    private bool isDead;

    private void Start()
    {
        attackHandle.Initialize(OnAttackByEvent, OnEndAttackByEvent);
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

    public override void StopAnimation()
    {
        skeleton.AnimationState.ClearTracks();
    }

    public override void OnAttack(int damage, Action callback)
    {
        _callbackAttackPlayer = callback;
        PlayAttack();
    }

    public override void OnBeingAttacked()
    {
        isAttacking = false;
        isAttacked = true;
        //OnDead(); 
    }

    private void OnAttackByEvent()
    {
        _callbackAttackPlayer?.Invoke();
        GameController.Instance.Player?.Skeleton.Play("Die2", false);
        GameController.Instance.UpdateBlood(true);
    }

    private void OnEndAttackByEvent()
    {
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
            SoundController.Instance.PlayOnce(SoundType.BossAttack);
        }


        //DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
        //{
        //    if (!isDead)
        //    {
        //        fire.gameObject.SetActive(true);
        //        fire.Play();
        //        DOTween.Sequence().AppendInterval(2.2f).AppendCallback(() =>
        //        {
        //            fire.gameObject.SetActive(false);
        //            fire.Stop();
        //        });
        //    }
        //});
    }

    public void PLayMove(bool isLoop) { skeleton.Play("Run", true); }

    public void PlayDead()
    {
        //isDead = true;
        //fire.gameObject.SetActive(false);

        //ItemType equipType = GameController.Instance.Player.EquipType;
        //if (equipType == ItemType.Sword || equipType == ItemType.SwordBlood)
        //{
        //    skeleton.Play("Die2", false);
        //    // DOTween.Sequence().AppendInterval(.75f).AppendCallback(() =>
        //    // {
        //    //     skeleton.DOFade(0, .2f);
        //    // });
        //}
        //else
        //{
        //    skeleton.Play("Die", false);
        //    DOTween.Sequence().AppendInterval(1.85f).AppendCallback(() =>
        //    {
        //        skeleton.DOFade(0, .2f);
        //    });
        //}
        skeleton.Play("Swoon", false);
        SoundController.Instance.PlayOnce(SoundType.DragonDie);
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
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyDragonHead))]
public class EnemyDragonHeadEditor : UnityEditor.Editor
{
    private EnemyDragonHead _enemy;

    private void OnEnable() { _enemy = (EnemyDragonHead)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif