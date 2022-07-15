using DG.Tweening;
using System;
using System.Collections.Generic;
using Spine.Unity;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class EnemyIceDragon : Unit, IAnim
{
    public SkeletonGraphic skeleton;
    public Rigidbody2D rigid;
    public Collider2D coll2D;
    public SpineAttackHandle attackHandle;
    public override EUnitType Type { get; protected set; } = EUnitType.Boss;
    public List<ParticleSystem> particleSystems;

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

    private void OnAttackByEvent()
    {
        //ParticleSystem particleSystem1 = Instantiate(particleSystems[0], transform);
        //ParticleSystem particleSystem2 = Instantiate(particleSystems[1], transform);
        //particleSystem1.gameObject.SetActive(true);
        //particleSystem2.gameObject.SetActive(true);
        //particleSystem1.transform.DOMove(GameController.Instance.Player.transform.position + Vector3.left / 2 + Vector3.up / 2, .3f).OnComplete(() => DestroyImmediate(particleSystem1.gameObject));
        //particleSystem2.transform.DOMove(GameController.Instance.Player.transform.position + Vector3.left / 2 + Vector3.up / 2, .3f).OnComplete(() => DestroyImmediate(particleSystem2.gameObject));
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
        skeleton.Play("Swoon", false);

        SoundController.Instance.PlayOnce(SoundType.BearDie);
    }

    public void PlayWin(bool isLoop) { }

    public void PlayLose(bool isLoop) { }
    public override void PlayHurt()
    {
        skeleton.Play("Hurt", false);
    }
    public override void PlayDie()
    {
        skeleton.Play("Swoon", false);
        SoundController.Instance.PlayOnce(SoundType.DragonDie);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyIceDragon))]
public class EnemyIceDragonEditor : UnityEditor.Editor
{
    private EnemyIceDragon _enemy;

    private void OnEnable() { _enemy = (EnemyIceDragon)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _enemy.TxtDamage.text = _enemy.Damage.ToString();

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif