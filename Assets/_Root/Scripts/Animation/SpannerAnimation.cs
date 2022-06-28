using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpannerAnimation : MonoBehaviour
{
    [SerializeField] private float durationDo = 1f;
    [SerializeField] private float durationUndo = 1f;
    [SerializeField] private Ease easeDo = Ease.Linear;
    [SerializeField] private Ease easeUndo = Ease.Linear;
    [SerializeField] private float angleDo = 45f;
    [SerializeField] private float angleUndo = 0f;

    private int countSpanner = 1;

    private float duration = 1f;
    private Ease ease = Ease.Linear;
    private float angle = 0;
    private int direction = 1;
    private Quaternion rotationOrigin;

    private void OnEnable()
    {
        rotationOrigin = transform.rotation;
        Debug.Log(rotationOrigin);
        FetchSetting();
        DoRotation();
    }

    private void OnDisable()
    {
        transform.DOKill(this);
        countSpanner = 1;
        transform.rotation = rotationOrigin;
    }

    private void FetchSetting()
    {
        if (direction > 0)
        {
            duration = durationDo;
            ease = easeDo;
            angle = angleDo + transform.rotation.eulerAngles.z;
        }
        else
        {
            duration = durationUndo;
            ease = easeUndo;
            angle = angleUndo + transform.rotation.eulerAngles.z - angleDo;
        }
    }

    private void ChangeRotation()
    {
        if (gameObject.activeSelf)
        {
            direction *= -1;
            if (direction == 1)
            {
                countSpanner += 1;
                if (countSpanner % 2 == 0)
                {
                    transform.DORotate(new Vector3(0, 0, transform.rotation.eulerAngles.z + 90), 0.1f).SetEase(ease).OnComplete(() =>
                    {
                        FetchSetting();
                        DoRotation();
                    });
                }
                else
                {
                    FetchSetting();
                    DoRotation();
                }
            }
            else
                {
                    FetchSetting();
                    DoRotation();
                }
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
