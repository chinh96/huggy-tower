using System;
using DG.Tweening;
using Spine.Unity;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public class EnemyDragonGold : Unit, IAnim
{
    public SkeletonGraphic skeleton;
    public Rigidbody2D rigid;
    public Collider2D coll2D;
    public SpineAttackHandle attackHandle;
    public override EUnitType Type { get; protected set; } = EUnitType.Enemy;
    public ParticleSystem fire;
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
        //ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.DragonGold);
    }

    public SkeletonGraphic Skeleton => skeleton;
    public void PlayIdle(bool isLoop) {
        string[] idleList = {"Idle", "Idle2", "Idle3" };
        skeleton.Play(idleList[UnityEngine.Random.Range(0, idleList.Length)], true); 
    }

    public void PlayAttack()
    {
        skeleton.Play("Attack", false);
        SoundController.Instance.PlayOnce(SoundType.DragonGoldAttack);

        //DOTween.Sequence().AppendInterval(.1f).AppendCallback(() =>
        //{
        //    fire.gameObject.SetActive(true);
        //    fire.Play();
        //    fire.transform.DOMove(GameController.Instance.Player.transform.position + new Vector3(-1, .5f, 0), .3f).SetEase(Ease.Linear).OnComplete(() =>
        //    {
        //        Destroy(fire.gameObject);
        //    });
        //});
    }

    public void PLayMove(bool isLoop) { skeleton.Play("Run", true); }

    public void PlayDead()
    {
        skeleton.Play("Swoon", false);
        SoundController.Instance.PlayOnce(SoundType.DragonGoldDie);
    }

    public void PlayWin(bool isLoop) { }

    public void PlayLose(bool isLoop) { }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyDragonGold))]
public class EnemyDragonGoldEditor : UnityEditor.Editor
{
    private EnemyDragonGold _enemy;

    private void OnEnable() { _enemy = (EnemyDragonGold)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _enemy.TxtDamage.text = _enemy.Damage.ToString();

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif