using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;

public class OnboardingHand : MonoBehaviour, IHasSkeletonDataAsset
{
    [SerializeField] private SkeletonDataAsset skeletonDataAsset;
    public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;
    public RectTransform StartObject;
    public RectTransform EndObject;
    public SkeletonGraphic HandObject;
    [SpineAnimation] public string HandAnim;
    public float duration = 1;
    public ParticleSystem FX;
    public RectTransform Arrows;

    private Sequence sequence;
    private Vector2 sizeDelta;

    private void Awake()
    {
        sizeDelta = Arrows.sizeDelta;
        Arrows.sizeDelta = Vector2.zero;
    }

    private void Start()
    {
        BeforeMove();
    }

    private void Reset()
    {
        HandObject.transform.position = StartObject.position;
    }

    private void BeforeMove()
    {
        HandObject.Play(HandAnim, false);
        HandObject.AnimationState.Complete += DoneHandAnim;
    }

    private void DoneHandAnim(TrackEntry args)
    {
        HandObject.AnimationState.Complete -= DoneHandAnim;
        FX.Play();
        sequence = DOTween.Sequence().AppendInterval(FX.main.duration).AppendCallback(() =>
        {
            Move();
        });
    }

    private void Move()
    {
        HandObject.transform.DOMove(EndObject.position, duration).OnComplete(EndMove);
        Arrows.DOSizeDelta(sizeDelta, duration).OnComplete(() =>
        {
            Arrows.sizeDelta = Vector2.zero;
        });
    }

    private void EndMove()
    {
        Reset();
        BeforeMove();
    }

    private void OnDisable()
    {
        HandObject.transform.DOKill();
        sequence.Kill();
    }
}
