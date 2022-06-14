using System;
using Spine.Unity;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class EnemyRange : Unit, IAnim
{
    public SkeletonGraphic skeleton;
    public Rigidbody2D rigid;
    public Collider2D coll2D;
    public SpineAttackHandle attackHandle;
    public RectTransform arrowSpawnPosition;
    public override EUnitType Type { get; protected set; } = EUnitType.Enemy;

    private Action _callbackAttackPlayer;

    private void Start() { 
        attackHandle.Initialize(OnAttackByEvent, OnEndAttackByEvent); 
    }

    private void OnEndAttackByEvent() { PlayIdle(true); }

    private void OnAttackByEvent()
    {
        _callbackAttackPlayer?.Invoke();
    }

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
        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.NormalEnemy);
    }

    public override void DarknessRise() { }

    public override void LightReturn() { }

    public SkeletonGraphic Skeleton => skeleton;
    public void PlayIdle(bool isLoop) { skeleton.Play("Idle", true); }

    public void PlayAttack() { skeleton.Play("Attack", false); SoundController.Instance.PlayOnce(SoundType.EnemyShoot); }

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
[CustomEditor(typeof(EnemyRange))]
public class EnemyRangeEditor : UnityEditor.Editor
{
    private EnemyRange _enemyRange;

    private void OnEnable() { _enemyRange = (EnemyRange)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _enemyRange.TxtDamage.text = _enemyRange.Damage.ToString();

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif