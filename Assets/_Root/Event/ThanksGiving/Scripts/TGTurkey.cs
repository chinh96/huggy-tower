using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TGTurkey : MonoBehaviour
{
    public float Duration = .5f;
    public Ease Ease = Ease.InBack;
    public float JumpPower = 4f;

    private TGTurkeyTotal tGTurkeyTotal;

    private void Start()
    {
        tGTurkeyTotal = FindObjectOfType<TGTurkeyTotal>();
    }

    public void Move()
    {
        TGDatas.TotalTurkeyText++;
        Vector3 endValue = tGTurkeyTotal.Target.transform.position;
        transform.DOJump(endValue, JumpPower, 0, Duration).SetEase(Ease).OnComplete(() => Destroy(gameObject));
        transform.DOScale(Vector3.one, Duration);
        SoundController.Instance.PlayOnce(SoundType.TurkeyJump);
    }
}