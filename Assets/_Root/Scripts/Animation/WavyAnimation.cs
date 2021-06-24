using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WavyAnimation : MonoBehaviour
{
    [SerializeField] private float offset = 5;
    [SerializeField] private float duration = 0.5f;

    private float originY;

    private void Start()
    {
        originY = transform.localPosition.y;
        MoveUp();
    }

    private void MoveUp()
    {
        transform.DOLocalMoveY(originY + offset, duration).OnComplete(() => MoveDown());
    }

    private void MoveDown()
    {
        transform.DOLocalMoveY(originY, duration).OnComplete(() => MoveUp());
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
