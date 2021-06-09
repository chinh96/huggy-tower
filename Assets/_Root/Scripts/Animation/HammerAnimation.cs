using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HammerAnimation : MonoBehaviour
{
    [SerializeField] private float durationForward = 1f;
    [SerializeField] private float durationBackward = 1f;
    [SerializeField] private Ease easeForward = Ease.Linear;
    [SerializeField] private Ease easeBackward = Ease.Linear;
    [SerializeField] private float angleForward = 90f;
    [SerializeField] private float angleBackward = 0f;
    private float duration = 1f;
    private Ease ease = Ease.Linear;
    private float angle = 90;
    private int direction = 1;
    private Quaternion rotationOrigin;

    private void OnEnable()
    {
        rotationOrigin = transform.rotation;
        FetchSetting();
        DoRotation();
    }

    private void OnDisable()
    {
        transform.DOKill(this);
        transform.rotation = rotationOrigin;
    }

    private void FetchSetting()
    {
        if (direction > 0)
        {
            duration = durationForward;
            ease = easeForward;
            angle = angleForward;
        }
        else
        {
            duration = durationBackward;
            ease = easeBackward;
            angle = angleBackward;
        }
    }

    private void ChangeRotation()
    {
        if (gameObject.activeSelf)
        {
            direction *= -1;
            FetchSetting();
            DoRotation();
        }
    }

    private void DoRotation()
    {
        if (gameObject.activeSelf)
        {
            transform.DORotate(new Vector3(0, 0, angle), duration).SetEase(ease).OnComplete(() => ChangeRotation());
        }
    }
}
