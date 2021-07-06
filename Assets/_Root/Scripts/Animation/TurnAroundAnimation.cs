using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurnAroundAnimation : MonoBehaviour
{
    Sequence run;

    private void Start()
    {
        run = DOTween.Sequence();
        Tween rot = transform.DORotate(new Vector3(0, 0, 360), .1f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
        run.Append(rot).SetLoops(-1);
    }

    private void OnDestroy()
    {
        run.Kill();
    }
}
