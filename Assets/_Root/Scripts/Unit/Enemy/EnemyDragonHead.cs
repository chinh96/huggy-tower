using System;
using Spine.Unity;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using DG.Tweening;

public class EnemyDragonHead : Unit, IAnim
{
    public SkeletonGraphic skeleton;
    public Collider2D coll2D;
    public SpineAttackHandle attackHandle;
    public override EUnitType Type { get; protected set; } = EUnitType.Enemy;
    public ParticleSystem fire;

    private Action _callbackAttackPlayer;
    private bool isDead;

    private void Start()
    {
        attackHandle.Initialize(OnAttackByEvent, OnEndAttackByEvent);
        SoundController.Instance.PlayOnce(SoundType.DragonStart);
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
        TxtDamage.gameObject.SetActive(false);
        PlayDead();
        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.DragonEnemy);
        ResourcesController.Achievement.IncreaseByType(AchievementType.DragonEnemy);
    }

    public SkeletonGraphic Skeleton => skeleton;
    public void PlayIdle(bool isLoop) { skeleton.Play("Idle", true); }

    public void PlayAttack()
    {
        skeleton.Play("Attack", false);
        SoundController.Instance.PlayOnce(SoundType.DemonAttack);

        DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
        {
            if (!isDead)
            {
                fire.gameObject.SetActive(true);
                fire.Play();
                DOTween.Sequence().AppendInterval(2.2f).AppendCallback(() =>
                {
                    fire.gameObject.SetActive(false);
                    fire.Stop();
                });
            }
        });
    }

    public void PLayMove(bool isLoop) { skeleton.Play("Run", true); }

    public void PlayDead()
    {
        isDead = true;
        fire.gameObject.SetActive(false);

        ItemType equipType = GameController.Instance.Player.EquipType;
        if (equipType == ItemType.Sword || equipType == ItemType.SwordBlood)
        {
            skeleton.Play("Die2", false);
            DOTween.Sequence().AppendInterval(.75f).AppendCallback(() =>
            {
                skeleton.DOFade(0, .2f);
            });
        }
        else
        {
            skeleton.Play("Die", false);
            DOTween.Sequence().AppendInterval(1.85f).AppendCallback(() =>
            {
                skeleton.DOFade(0, .2f);
            });
        }

        SoundController.Instance.PlayOnce(SoundType.DragonDie);
    }

    public void PlayWin(bool isLoop) { }

    public void PlayLose(bool isLoop) { }
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

        _enemy.TxtDamage.text = _enemy.Damage.ToString();

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif