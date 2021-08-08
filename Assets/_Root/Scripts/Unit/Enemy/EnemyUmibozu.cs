using System;
using Spine.Unity;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using DG.Tweening;

public class EnemyUmibozu : Unit, IAnim
{
    public SkeletonGraphic skeleton;
    public Rigidbody2D rigid;
    public Collider2D coll2D;
    public SpineAttackHandle attackHandle;
    public override EUnitType Type { get; protected set; } = EUnitType.Enemy;
    public Transform FxTransform;
    public ParticleSystem Fx;
    public ParticleSystem FxWater;

    private Action _callbackAttackPlayer;
    private bool isDead;

    private void Start()
    {
        attackHandle.Initialize(OnAttackByEvent, OnEndAttackByEvent);
        SoundController.Instance.PlayOnce(SoundType.BearStart);
    }

    public override void OnAttack(int damage, Action callback)
    {
        _callbackAttackPlayer = callback;
        PlayAttack();
    }

    public override void OnBeingAttacked() { OnDead(); }

    private void OnAttackByEvent() { _callbackAttackPlayer?.Invoke(); }

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

    public void PlayAttack()
    {
        DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
        {
            if (!isDead)
            {
                ParticleSystem fx = Instantiate(Fx);
                fx.transform.position = FxTransform.position;
                fx.transform.DOMove(GameController.Instance.Player.transform.position + Vector3.up, .3f).OnComplete(() =>
                {
                    ParticleSystem fxWater = Instantiate(FxWater);
                    fxWater.transform.position = fx.transform.position;
                    DestroyImmediate(fx.gameObject);
                });
            }
        });
        skeleton.Play("Attack", false);
        SoundController.Instance.PlayOnce(SoundType.DemonAttack);
    }

    public void PLayMove(bool isLoop) { skeleton.Play("Run", true); }

    public void PlayDead()
    {
        skeleton.Play("Die", false);
        isDead = true;

        SoundController.Instance.PlayOnce(SoundType.BearDie);
    }

    public void PlayWin(bool isLoop) { }

    public void PlayLose(bool isLoop) { }
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