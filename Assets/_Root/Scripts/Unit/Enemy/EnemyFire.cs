using System;
using Spine.Unity;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class EnemyFire : Unit, IAnim
{
    public SkeletonGraphic skeleton;
    public Rigidbody2D rigid;
    public Collider2D coll2D;
    public SpineAttackHandle attackHandle;
    public override EUnitType Type { get; protected set; } = EUnitType.Enemy;

    private Action _callbackAttackPlayer;

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
        ResourcesController.Achievement.IncreaseByType(AchievementType.BearEnemy);
        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.BearEnemy);
    }

    public SkeletonGraphic Skeleton => skeleton;
    public void PlayIdle(bool isLoop) { skeleton.Play("Idle2", true); }

    public void PlayAttack() { skeleton.Play("Attack2", false); SoundController.Instance.PlayOnce(SoundType.DemonAttack); }

    public void PLayMove(bool isLoop) { skeleton.Play("Run", true); }

    public void PlayDead()
    {
        skeleton.Play("Die2", false);

        SoundController.Instance.PlayOnce(SoundType.BearDie);
    }

    public void PlayWin(bool isLoop) { }

    public void PlayLose(bool isLoop) { }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyFire))]
public class EnemyFireEditor : UnityEditor.Editor
{
    private EnemyFire _enemy;

    private void OnEnable() { _enemy = (EnemyFire)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _enemy.TxtDamage.text = _enemy.Damage.ToString();

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif