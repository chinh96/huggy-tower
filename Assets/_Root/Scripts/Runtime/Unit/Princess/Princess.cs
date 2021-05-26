using System;
using Spine.Unity;
using DG.Tweening;
using UnityEngine;

public class Princess : Unit, IAnim
{
    [SerializeField] private SkeletonGraphic skeleton;
    [SerializeField] private Rigidbody2D rigid2D;
    [SerializeField] private Collider2D coll2D;
    public override EUnitType Type { get; protected set; } = EUnitType.Princess;
    public override void OnBeingAttacked() { }

    public override void OnAttack(int damage, Action callback) { }

    public override void DarknessRise() { }

    public override void LightReturn() { }
    public SkeletonGraphic Skeleton => skeleton;
    public void PlayIdle(bool isLoop) { skeleton.Play("idle", true); }

    public void PlayAttack() { }

    public void PLayMove(bool isLoop) { }

    public void PlayDead() { skeleton.Play("die", true); }

    public void PlayWin(bool isLoop)
    {
        SoundController.Instance.PlayOnce(SoundType.RescuePrincess);
        skeleton.Play("win", false);
        DOTween.Sequence().AppendInterval(.7f).AppendCallback(() =>
        {
            skeleton.Play("win 2", true);
        });
    }

    public void PlayLose(bool isLoop) { }
}