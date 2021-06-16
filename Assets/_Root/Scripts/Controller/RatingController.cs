using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using Google.Play.Review;
using Google.Play.Common;
#elif UNITY_IOS
using UnityEngine.iOS;
#endif
using System;

public class RatingController : Singleton<RatingController>
{
#if UNITY_ANDROID
    private ReviewManager reviewManager;
    private PlayReviewInfo playReviewInfo;

    private void Start()
    {
#if !UNITY_EDITOR
        reviewManager = new ReviewManager();
#endif
    }

    public void RequestReview()
    {
        StartCoroutine(RequestReviewFlow());
    }

    public void LaunchReview()
    {
        StartCoroutine(LaunchReviewFlow());
    }

    private IEnumerator RequestReviewFlow()
    {
        PlayAsyncOperation<PlayReviewInfo, ReviewErrorCode> requestFlowOperation = reviewManager.RequestReviewFlow();

        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }
        playReviewInfo = requestFlowOperation.GetResult();
    }

    private IEnumerator LaunchReviewFlow()
    {
        if (playReviewInfo == null)
        {
            Application.OpenURL("market://details?id=com.gamee.herotowerwar");
            yield break;
        }

        PlayAsyncOperation<VoidResult, ReviewErrorCode> launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);

        yield return launchFlowOperation;
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }
    }
#endif

    public void LinkToStore()
    {
#if UNITY_ANDROID
        LaunchReview();
#elif UNITY_IOS
        if (!Device.RequestStoreReview())
        {
            Application.OpenURL("itms-apps://itunes.apple.com/app/id1570840391");
        }
#endif
    }
}
