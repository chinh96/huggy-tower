using Spine.Unity;
using UnityEngine;

public class PrincessIntro : MonoBehaviour, IHasSkeletonDataAsset
{
    public SkeletonAnimation SkeletonAnimation;
    [SerializeField] private SkeletonDataAsset skeletonDataAsset;
    public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;

    [SpineAnimation] public string Talk;

    public void PlayTalk()
    {
        PlayAnim(Talk, true);
    }

    private void PlayAnim(string name, bool isLoop)
    {
        SkeletonAnimation.AnimationState.SetAnimation(0, name, isLoop);
    }
}
