using System;
using Spine.Unity;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Princess : Unit, IAnim
{
    [SerializeField] private SkeletonGraphic skeleton;
    [SerializeField] private Rigidbody2D rigid2D;
    [SerializeField] private Collider2D coll2D;
    [SerializeField] private Image lockObj;
    [SerializeField] private Image lockObj2;
    [SerializeField] private Canvas canvas;

    private void Start()
    {
        SoundController.Instance.PlayOnce(SoundType.PrincessStart);
    }

    public override EUnitType Type { get; protected set; } = EUnitType.Princess;

    public override void OnBeingAttacked() { }

    public override void OnAttack(int damage, Action callback) { }

    public override void DarknessRise() { }

    public override void LightReturn() { }
    public SkeletonGraphic Skeleton => skeleton;

    public Image LockObj { get => lockObj; set => lockObj = value; }
    public Image LockObj2 { get => lockObj2; set => lockObj2 = value; }

    public void PlayIdle(bool isLoop) { skeleton.Play("Idle", true); }

    public void PlayAttack() { }

    public void PLayMove(bool isLoop) { }

    public void PlayDead() { skeleton.Play("Die", true); }

    public void PlayWin(bool isLoop)
    {
        canvas.sortingOrder = 130;
        // if (Data.TimeToRescueParty.TotalMilliseconds > 0)
        // {
        //     Data.TotalGoldMedal++;
        //     skeleton.Play("GiveCandy", false);
        //     DOTween.Sequence().AppendInterval(1f).AppendCallback(() =>
        //     {
        //         skeleton.Play("GiveCandy2", true);
        //     });
        // }
        // else
        // {
        skeleton.Play("Win", false);
        DOTween.Sequence().AppendInterval(.7f).AppendCallback(() =>
        {
            skeleton.Play("Win", true);
            //skeleton.Play("win 2", true);
        });
        //}

        SoundController.Instance.PlayOnce(SoundType.RescuePrincess);
        ResourcesController.Achievement.IncreaseByType(AchievementType.Princess);
        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.Princess);
    }

    public void PlayLose(bool isLoop) { }
}