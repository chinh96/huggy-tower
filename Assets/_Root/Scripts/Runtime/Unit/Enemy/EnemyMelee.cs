using System;
using Spine.Unity;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class EnemyMelee : Unit, IAnim
{
    public bool IsSword;
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

    public void PlayAttack() { skeleton.Play(IsSword ? "AttackSword" : "Attack", false); SoundController.Instance.PlayOnce(SoundType.EnemyCut); }

    public void PLayMove(bool isLoop) { skeleton.Play("Run", true); }

    public void PlayDead()
    {
        skeleton.Play("Die", false);

        SoundType[] soundTypes = { SoundType.EnemyDie, SoundType.EnemyDie2, SoundType.EnemyDie3 };
        SoundType soundType = soundTypes[UnityEngine.Random.Range(0, soundTypes.Length)];
        SoundController.Instance.PlayOnce(soundType);
    }

    public void PlayWin(bool isLoop) { }

    public void PlayLose(bool isLoop) { }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyMelee))]
public class EnemyMeleeEditor : UnityEditor.Editor
{
    private EnemyMelee _enemyMelee;

    private void OnEnable() { _enemyMelee = (EnemyMelee)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _enemyMelee.TxtDamage.text = _enemyMelee.Damage.ToString();

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif