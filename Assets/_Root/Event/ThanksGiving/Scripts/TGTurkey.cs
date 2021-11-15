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
        transform.DOScale(Vector3.one * 1.5f, .25f).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, .25f).OnComplete(() =>
            {
                transform.DOJump(endValue, JumpPower, 0, .5f).OnComplete(() => Destroy(gameObject));
            });
        });
        SoundController.Instance.PlayOnce(SoundType.TurkeyJump);
        FX.Play();
    }
}