using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TGTurkey : MonoBehaviour
{
    public float Duration = .5f;
    public Ease Ease = Ease.InBack;
    public float JumpPower = 4f;
    public ParticleSystem FX;

    private TGTurkeyTotal tGTurkeyTotal;

    private void Start()
    {
        tGTurkeyTotal = FindObjectOfType<TGTurkeyTotal>();
    }

    public void Move()
    {
        TGDatas.TotalTurkeyText++;
        Vector3 endValue = tGTurkeyTotal.Target.transform.position;
        transform.DOScale(Vector3.one * 1.5f, Duration / 2).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, Duration / 2).OnComplete(() =>
            {
                transform.DOJump(endValue, JumpPower, 0, Duration).OnComplete(() => Destroy(gameObject));
            });
        });
        SoundController.Instance.PlayOnce(SoundType.TurkeyJump);
        FX.Play();
    }
}