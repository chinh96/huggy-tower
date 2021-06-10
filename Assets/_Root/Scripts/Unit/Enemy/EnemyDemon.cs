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
        ResourcesController.Achievement.IncreaseByType(AchievementType.DemonEnemy);
    }

    public SkeletonGraphic Skeleton => skeleton;
    public void PlayIdle(bool isLoop) { skeleton.Play("Idle", true); }

    public void PlayAttack() { skeleton.Play("Attack", false); SoundController.Instance.PlayOnce(SoundType.DemonAttack); }

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