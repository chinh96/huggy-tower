using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class JumpAnimation : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private float offset = 20;
    [SerializeField] private float duration = .7f;
    [SerializeField] private float offsetScale = 1.2f;
    [SerializeField] private Ease easeOut = Ease.OutQuart;
    [SerializeField] private Ease easeIn = Ease.InOutBack;

    private Sequence sequence;

    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        sequence = DOTween.Sequence().AppendInterval(delay).AppendCallback(() =>
        {
            MoveUp();
        });
    }

    private void MoveUp()
    {
        transform.DOLocalMoveY(offset, duration / 2).SetEase(easeOut).OnComplete(() => MoveDown());
        transform.DOScale(offsetScale * Vector3.one, duration / 2);
    }

    private void MoveDown()
    {
        transform.DOLocalMoveY(0, duration / 2).SetEase(easeIn).OnComplete(() => MoveDelay());
        transform.DOScale(Vector3.one, duration / 2);
    }

    private void MoveDelay()
    {
        sequence = DOTween.Sequence().AppendInterval(duration).AppendCallback(() =>
        {
            MoveUp();
        });
    }

    private void OnDisable()
    {
        transform.DOKill();
        sequence.Kill();
    }
}
