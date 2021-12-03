using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Spine.Unity;
public class FeatureHeadSkin : MonoBehaviour, IObserver<SkinData>
{
    // Start is called before the first frame update
    private IDisposable cancellation;
    [SerializeField] private SkeletonGraphic skeleton;
    [SerializeField] private SubjectSkinChange provider;
    public virtual void Subscribe(SubjectSkinChange provider)
    {
        cancellation = provider.Subscribe(this);
    }
    public virtual void Unsubscribe()
    {
        if (cancellation != null)
            cancellation.Dispose();
    }

    public virtual void OnCompleted()
    {

    }
    public virtual void OnNext(SkinData info)
    {
        if (info.FeatureHead != "")
        {
            if (!skeleton) return;
            skeleton.gameObject.SetActive(true);
            skeleton.ChangeSkin2(info.FeatureHead);
            // skeleton.Skeleton.SetSkin(info.FeatureHead);
        }
        else
        {
            if (skeleton)
                skeleton.gameObject.SetActive(false);
        }

    }
    public virtual void OnError(Exception e)
    {
        // No implementation.
    }


    void OnEnable()
    {
        if (!provider) provider = transform.GetComponent<SubjectSkinChange>();
        if (provider != null)
            Subscribe(provider);
    }
    // private void OnDestroy()
    // {
    //     Unsubscribe();
    // }





}
