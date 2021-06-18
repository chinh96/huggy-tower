using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using UnityEngine.UI;

public class FighterOverlay : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private float duration = 1;
    [SerializeField] private Ease ease;

    private void OnEnable()
    {
        skeletonGraphic.color = new Color(1, 1, 1, 0);
        skeletonGraphic.transform.localScale = Vector3.one * 5;
    }

    private void Start()
    {
        skeletonGraphic.DOColor(new Color(1, 1, 1, 1), duration).SetEase(ease);
        skeletonGraphic.transform.DOScale(Vector3.one, duration).SetEase(ease).OnComplete(() =>
        {
            skeletonGraphic.Play("Idle", false);
            DOTween.Sequence().AppendInterval(duration).AppendCallback(() =>
            {
                image.DOColor(new Color(1, 1, 1, 0), duration / 2).SetEase(ease);
                skeletonGraphic.DOColor(new Color(1, 1, 1, 0), duration / 2).SetEase(ease);
                skeletonGraphic.transform.DOScale(Vector3.one * 5, duration / 2).SetEase(ease).OnComplete(() =>
                {
                    Destroy(gameObject);
                });
            });
        });
    }
}
