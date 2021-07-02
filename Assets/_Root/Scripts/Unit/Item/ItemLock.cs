using Spine.Unity;
using UnityEngine;
using DG.Tweening;

public class ItemLock : Item, IAnim
{
    [SerializeField] private SkeletonGraphic skeleton;
    [SerializeField] private GameObject lockPosition;

    public SkeletonGraphic Skeleton => skeleton;
    public GameObject LockPosition => lockPosition;

    private void Start()
    {
        State = EUnitState.Invalid;
    }

    public void PlayIdle(bool isLoop) { }

    public void PlayAttack() { }

    public void PLayMove(bool isLoop) { }

    public void PlayDead() { }

    public void PlayWin(bool isLoop = false)
    {
        skeleton.Play("Open", isLoop);
        SoundController.Instance.PlayOnce(SoundType.OpenChest);
        DOTween.Sequence().AppendInterval(1.5f).AppendCallback(() =>
        {
            skeleton.DOColor(new Color(0, 0, 0, 0), .5f).OnComplete(() =>
            {
                State = EUnitState.Normal;
                gameObject.SetActive(false);
            });
        });
    }

    public void PlayLose(bool isLoop = false) { skeleton.Play("Fire", isLoop); }

    public override void Collect(IUnit affectTarget) { }
}