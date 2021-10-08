using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveTo : MonoBehaviour
{
    public Vector3 Offset;
    public float Duration;
    public float Delay;

    private void Start()
    {
        DOTween.Sequence().AppendInterval(Delay).AppendCallback(() =>
        {
            Vector3 targetPosition = transform.localPosition + Offset;
            transform.DOLocalMove(targetPosition, Duration).SetEase(Ease.Linear);
        });
    }
}
