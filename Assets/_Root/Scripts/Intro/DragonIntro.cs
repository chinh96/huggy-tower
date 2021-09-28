using Spine.Unity;
using UnityEngine;

public class DragonIntro : MonoBehaviour, IHasSkeletonDataAsset
{
    public SkeletonAnimation SkeletonAnimation;
    [SerializeField] private SkeletonDataAsset skeletonDataAsset;
    public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;

    [SpineAnimation] public string Fly;
    [SpineAnimation] public string Attack;

    public void PlayFly()
    {
        PlayAnim(Fly, true);
    }

    public void PlayAttack()
    {
        PlayAnim(Attack, true);
    }

    private void PlayAnim(string name, bool isLoop)
    {
        SkeletonAnimation.AnimationState.SetAnimation(0, name, isLoop);
    }
}
