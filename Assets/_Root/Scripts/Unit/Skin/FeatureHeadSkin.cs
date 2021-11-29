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

    public virtual void Subscribe(SubjectSkinChange provider)
    {
        cancellation = provider.Subscribe(this);
    }
    public virtual void Unsubscribe()
    {
        cancellation.Dispose();
    }

    public virtual void OnCompleted()
    {

    }
    public virtual void OnNext(SkinData info)
    {
        if (info.FeatureHead != "")
        {
            skeleton.gameObject.SetActive(true);
            skeleton.Skeleton.SetSkin(info.FeatureHead);
        }
        else
        {
            skeleton.gameObject.SetActive(false);
        }

    }
    public virtual void OnError(Exception e)
    {
        // No implementation.
    }


    void Start()
    {
        var provider = transform.GetComponent<SubjectSkinChange>();
        Subscribe(provider);
    }
    private void OnDestroy()
    {
        Unsubscribe();
    }



}
