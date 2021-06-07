using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class MoveOutAnimation : MonoBehaviour
{
    [SerializeField] private DirectionMoveOut direction = DirectionMoveOut.TOP;
    [SerializeField] private Ease ease = Ease.InBack;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float offset = 100f;
    [SerializeField] private float delay = 0f;
    private Vector2 positionOrigin = Vector2.zero;

    public void Reset()
    {
        transform.DOKill();
        if (positionOrigin != Vector2.zero)
        {
            transform.localPosition = positionOrigin;
        }
    }

    public void Play()
    {
        if (positionOrigin == Vector2.zero)
        {
            positionOrigin = transform.localPosition;
        }
        Vector2 positionMoved = new Vector2();
        switch (direction)
        {
            case DirectionMoveOut.TOP:
                positionMoved = new Vector2(transform.localPosition.x, transform.localPosition.y + offset);
                break;
            case DirectionMoveOut.RIGHT:
                positionMoved = new Vector2(transform.localPosition.x + offset, transform.localPosition.y);
                break;
            case DirectionMoveOut.BOTTOM:
                positionMoved = new Vector2(transform.localPosition.x, transform.localPosition.y - offset);
                break;
            case DirectionMoveOut.LEFT:
                positionMoved = new Vector2(transform.localPosition.x - offset, transform.localPosition.y);
                break;
        }

        DOTween.Sequence().AppendInterval(delay).AppendCallback(() =>
        {
            transform.DOLocalMove(positionMoved, duration).SetEase(ease);
        });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}

public enum DirectionMoveOut
{
    TOP,
    RIGHT,
    BOTTOM,
    LEFT
}
