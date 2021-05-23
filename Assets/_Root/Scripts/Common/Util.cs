using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

public static partial class Util
{
    public static bool Overlaps(this RectTransform a, RectTransform b) { return a.WorldRect().Overlaps(b.WorldRect()); }
    public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse) { return a.WorldRect().Overlaps(b.WorldRect(), allowInverse); }

    public static Rect WorldRect(this RectTransform rectTransform)
    {
        Vector2 sizeDelta = rectTransform.sizeDelta;
        float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
        float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

        Vector3 position = rectTransform.position;
        return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f, rectTransformWidth, rectTransformHeight);
    }


    public static void Play(this SkeletonAnimation skeleton, string nameAnimation, bool isLoop)
    {
        if (!skeleton.AnimationName.Equals(nameAnimation)) skeleton.AnimationState.SetAnimation(0, nameAnimation, isLoop);
    }

    public static void Play(this SkeletonGraphic skeleton, string nameAnimation, bool isLoop)
    {
        skeleton.AnimationState.SetAnimation(0, nameAnimation, loop: isLoop);
    }

    public static void GetSpineInterfaces(
        this Component component,
        out IAnimationStateComponent iAnimationState,
        out ISkeletonAnimation iSkeletonAnimation,
        out ISkeletonComponent iSkeletonComponent)
    {
        iAnimationState = null;
        iSkeletonAnimation = null;
        iSkeletonComponent = null;

        var skeletonAnimation = component.GetComponent<SkeletonAnimation>();
        if (skeletonAnimation)
        {
            iSkeletonAnimation = skeletonAnimation as ISkeletonAnimation;
            iAnimationState = skeletonAnimation as IAnimationStateComponent;
            iSkeletonComponent = skeletonAnimation as ISkeletonComponent;
        }

        var skeletonGraphics = component.GetComponent<SkeletonGraphic>();
        if (skeletonGraphics)
        {
            iSkeletonAnimation = skeletonGraphics as ISkeletonAnimation;
            iAnimationState = skeletonGraphics as IAnimationStateComponent;
            iSkeletonComponent = skeletonGraphics as ISkeletonComponent;
        }
    }

    public static IEnumerator SubscribeEvents(
        this IAnimationStateComponent i,
        Spine.AnimationState.TrackEntryDelegate onStart,
        Spine.AnimationState.TrackEntryEventDelegate onEvent,
        Spine.AnimationState.TrackEntryDelegate onComplete,
        Spine.AnimationState.TrackEntryDelegate onEnd)
    {
        if (i == null)
        {
            Debug.LogError("IAnimationStateComponent is NULL!!!");
            yield break;
        }

        if (i.AnimationState == null) yield return new WaitWhile(() => i.AnimationState == null);

        if (onStart != null) i.AnimationState.Start += onStart;
        if (onEvent != null) i.AnimationState.Event += onEvent;
        if (onComplete != null) i.AnimationState.Complete += onComplete;
        if (onEnd != null) i.AnimationState.End += onEnd;
    }

    public static void UnsubscribeEvents(
        this IAnimationStateComponent i,
        Spine.AnimationState.TrackEntryDelegate onStart,
        Spine.AnimationState.TrackEntryEventDelegate onEvent,
        Spine.AnimationState.TrackEntryDelegate onComplete,
        Spine.AnimationState.TrackEntryDelegate onEnd)
    {
        if (i == null || i.AnimationState == null) return;

        if (onStart != null) i.AnimationState.Start -= onStart;
        if (onEvent != null) i.AnimationState.Event -= onEvent;
        if (onComplete != null) i.AnimationState.Complete -= onComplete;
        if (onEnd != null) i.AnimationState.End -= onEnd;
    }

    public static void ChangeSkin(this SkeletonGraphic skeletonGraphic, string skinName)
    {
        skeletonGraphic.Skeleton.SetSkin(skinName);
        skeletonGraphic.Skeleton.SetSlotsToSetupPose();
    }

    public static void ChangeSword(this SkeletonGraphic skeletonGraphic, string swordName)
    {
        Skin skin = new Skin("skin");
        skin.AddSkin(skeletonGraphic.Skeleton.Data.FindSkin(swordName));
        skin.AddSkin(skeletonGraphic.Skeleton.Data.FindSkin(skeletonGraphic.initialSkinName));
        skeletonGraphic.Skeleton.SetSkin(skin);
        skeletonGraphic.Skeleton.SetSlotsToSetupPose();
    }

    public static void Shuffle<T>(this IList<T> source)
    {
        var n = source.Count;
        while (n > 1)
        {
            n--;
            var k = UnityEngine.Random.Range(0, n);
            var value = source[k];
            source[k] = source[n];
            source[n] = value;
        }
    }
}