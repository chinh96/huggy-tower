using UnityEngine;
using Spine.Unity;
using Spine;
using DG.Tweening;

public class HeroIntro : MonoBehaviour, IHasSkeletonDataAsset
{
    public SkeletonAnimation SkeletonAnimation;
    [SerializeField] private SkeletonDataAsset skeletonDataAsset;
    public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;

    [SpineAnimation] public string Idle;
    [SpineAnimation] public string Run;
    [SpineAnimation] public string Hurt;
    [SpineAnimation] public string Attack;
    [SpineAnimation] public string Jump;
    [SpineAnimation] public string Wait;
    [SpineAnimation] public string Fall;
    [SpineAnimation] public string FallInLove;

    private void Awake()
    {
        Skin skin = new Skin("skin");
        skin.AddSkin(SkeletonAnimation.Skeleton.Data.FindSkin("Pollaxe"));
        skin.AddSkin(SkeletonAnimation.Skeleton.Data.FindSkin("Hero1"));
        SkeletonAnimation.Skeleton.SetSkin(skin);
        SkeletonAnimation.Skeleton.SetSlotsToSetupPose();
    }

    private void Start()
    {
        PlayRun();
    }

    public void PlayIdle()
    {
        PlayAnim(Idle, true);
    }

    public void PlayRun()
    {
        PlayAnim(Run, true, .15f);
    }

    public void PlayHurt()
    {
        PlayAnim(Hurt, false);
    }

    public void PlayAttack()
    {
        PlayAnim(Attack, false);
    }

    public void PlayJump()
    {
        PlayAnim(Jump, false);
    }

    public void PlayWait()
    {
        PlayAnim(Wait, true);
    }

    public void PlayFall()
    {
        PlayAnim(Fall, true);
    }

    public void PlayFallInLove()
    {
        PlayAnim(FallInLove, false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayFallInLove();
        IntroController.Instance.PlayExploison();
        SoundController.Instance.PlayOnce(SoundType.IntroHeroFallInLove);
        DOTween.Sequence().AppendInterval(1.5f).AppendCallback(() =>
        {
            IntroController.Instance.LookBackToDragon();
        });
    }

    private void PlayAnim(string name, bool isLoop, float mixDuration = .2f)
    {
        SkeletonAnimation.AnimationState.SetAnimation(0, name, isLoop).MixDuration = mixDuration;
    }
}
