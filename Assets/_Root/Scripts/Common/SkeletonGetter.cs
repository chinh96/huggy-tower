using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SkeletonGetter : MonoBehaviour, IHasSkeletonDataAsset
{
    [SerializeField] private SkeletonDataAsset skeletonDataAsset;
    public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;
}
