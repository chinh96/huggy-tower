using UnityEngine;
using Spine.Unity;

public class GoblinIntro : MonoBehaviour, IHasSkeletonDataAsset
{
    public SkeletonAnimation SkeletonAnimation;
    [SerializeField] private SkeletonDataAsset skeletonDataAsset;
    public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;

    [SpineAnimation] public string Idle;
    [SpineAnimation] public string Attack;
    [SpineAnimation] public string Die;

    public void PlayIdle()
    {
        PlayAnim(Idle, true);
    }

    public void PlayAttack()
    {
        PlayAnim(Attack, false);
    }

    public void PlayDie()
    {
        PlayAnim(Die, false);
    }

    private void PlayAnim(string name, bool isLoop)
    {
        SkeletonAnimation.AnimationState.SetAnimation(0, name, isLoop);
    }
}
