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

    private void Start() { attackHandle.Initialize(OnAttackByEvent, OnEndAttackByEvent); }

    private void OnEndAttackByEvent() { PlayIdle(true); }

    private void OnAttackByEvent()
    {
        var pool = GameController.Instance.poolArrow;

        var arrow = pool.Spawn(pool.transform, false);
        var arowHandle = arrow.GetComponent<ArrowHandle>();
        arowHandle.Initialize((go) =>
        {
            _callbackAttackPlayer?.Invoke();
            GameController.Instance.poolArrow.Despawn(go);
        });
        arrow.transform.position = arrowSpawnPosition.position;
        arrow.transform.localEulerAngles = new Vector3(0, 0, 180);
        arrow.SetActive(true);
        arrow.GetComponent<Rigidbody2D>().velocity = Vector2.left * 12;
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
    }

    public override void DarknessRise() { }

    public override void LightReturn() { }

    public SkeletonGraphic Skeleton => skeleton;
    public void PlayIdle(bool isLoop) { skeleton.Play("Idle", true); }

    public void PlayAttack() { skeleton.Play("AttackArchery", false); SoundController.Instance.PlayOnce(SoundType.EnemyShoot); }

    public void PLayMove(bool isLoop) { skeleton.Play("Run", true); }

    public void PlayDead() { skeleton.Play("Die", false); SoundController.Instance.PlayOnce(SoundType.EnemyDie); }

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