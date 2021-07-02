using Spine.Unity;
using UnityEngine;

public class ItemLock : Item, IAnim
{
    [SerializeField] private SkeletonGraphic skeleton;

    public SkeletonGraphic Skeleton => skeleton;
    public void PlayIdle(bool isLoop) { }

    public void PlayAttack() { }

    public void PLayMove(bool isLoop) { }

    public void PlayDead() { }

    public void PlayWin(bool isLoop = false) { skeleton.Play("Open", isLoop); SoundController.Instance.PlayOnce(SoundType.OpenChest); }

    public void PlayLose(bool isLoop = false) { skeleton.Play("Fire", isLoop); }

    public override void Collect(IUnit affectTarget) { }
}