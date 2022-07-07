package com.applovin.mediation.adapters;

import android.app.Activity;
import android.content.Context;
import android.graphics.drawable.Drawable;
import android.net.Uri;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.view.View;
import android.widget.ImageView;

import com.alxad.api.AlxAdSDK;
import com.alxad.api.AlxBannerAD;
import com.alxad.api.AlxBannerADListener;
import com.alxad.api.AlxImage;
import com.alxad.api.AlxInterstitialAD;
import com.alxad.api.AlxInterstitialADListener;
import com.alxad.api.AlxNativeAD;
import com.alxad.api.AlxNativeAdListener;
import com.alxad.api.AlxNativeAdLoadListener;
import com.alxad.api.AlxRewardVideoAD;
import com.alxad.api.AlxRewardVideoADListener;
import com.alxad.api.AlxSdkInitCallback;
import com.alxad.api.IAlxNativeInfo;
import com.applovin.impl.sdk.utils.BundleUtils;
import com.applovin.mediation.MaxAdFormat;
import com.applovin.mediation.adapter.MaxAdViewAdapter;
import com.applovin.mediation.adapter.MaxAdapterError;
import com.applovin.mediation.adapter.MaxInterstitialAdapter;
import com.applovin.mediation.adapter.MaxNativeAdAdapter;
import com.applovin.mediation.adapter.MaxRewardedAdapter;
import com.applovin.mediation.adapter.listeners.MaxAdViewAdapterListener;
import com.applovin.mediation.adapter.listeners.MaxInterstitialAdapterListener;
import com.applovin.mediation.adapter.listeners.MaxNativeAdAdapterListener;
import com.applovin.mediation.adapter.listeners.MaxRewardedAdapterListener;
import com.applovin.mediation.adapter.parameters.MaxAdapterInitializationParameters;
import com.applovin.mediation.adapter.parameters.MaxAdapterResponseParameters;
import com.applovin.mediation.nativeAds.MaxNativeAd;
import com.applovin.mediation.nativeAds.MaxNativeAdView;
import com.applovin.sdk.AppLovinSdk;
import com.applovin.sdk.AppLovinSdkUtils;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Future;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicBoolean;

/**
 * Applovin ads AlgoriX Adapter
 */
public class AlgorixMediationAdapter extends MediationAdapterBase implements MaxAdViewAdapter, MaxInterstitialAdapter, MaxRewardedAdapter, MaxNativeAdAdapter {
    private static final String TAG = "AlgorixMediationAdapter";
    private static final String ADAPTER_VERSION = "3.1.2";

    private static final int DEFAULT_IMAGE_TASK_TIMEOUT_SECONDS = 10;

    private static final AtomicBoolean initialized = new AtomicBoolean();
    private static InitializationStatus status;

    private AlxBannerAD bannerAD;
    private AlxInterstitialAD interstitialAD;
    private AlxRewardVideoAD rewardVideoAD;
    private IAlxNativeInfo nativeAD;

    public AlgorixMediationAdapter(AppLovinSdk appLovinSdk) {
        super(appLovinSdk);
    }

    @Override
    public void initialize(MaxAdapterInitializationParameters parameters, Activity activity, final OnCompletionListener onCompletionListener) {
        Log.d(TAG, "initialize alx sdk……");
        Log.d(TAG, "alx-applovin-adapter-version:" + ADAPTER_VERSION);

        try {
            Context context = (activity != null) ? activity.getApplicationContext() : getApplicationContext();

            Bundle bundle = parameters.getCustomParameters();
            String appid = bundle.getString("appid");
            String sid = bundle.getString("sid");
            String token = bundle.getString("token");

            Log.d(TAG, "alx-applovin-init:token=" + token + "  sid=" + sid + " appid=" + appid);

            if (TextUtils.isEmpty(appid) || TextUtils.isEmpty(sid) || TextUtils.isEmpty(token)) {
                Log.d(TAG, "initialize alx params: appid or sid or token is null");
                status = InitializationStatus.DOES_NOT_APPLY;
                onCompletionListener.onCompletion(status, null);
                return;
            } else {
                AlxAdSDK.init(context, token, sid, appid, new AlxSdkInitCallback() {
                    @Override
                    public void onInit(boolean isOk, String msg) {
                        status = InitializationStatus.INITIALIZED_SUCCESS;
                        onCompletionListener.onCompletion(status, null);
                    }
                });
            }
        } catch (Exception e) {
            Log.d(TAG, "initialize alx error:" + e.getMessage());
            status = InitializationStatus.INITIALIZED_FAILURE;
            onCompletionListener.onCompletion(status, null);
        }
    }

    @Override
    public String getSdkVersion() {
        return AlxAdSDK.getNetWorkVersion();
    }

    @Override
    public String getAdapterVersion() {
        return ADAPTER_VERSION;
    }

    @Override
    public void onDestroy() {
        Log.d(TAG, "onDestroy");
        if (bannerAD != null) {
            bannerAD.destroy();
            bannerAD = null;
        }

        if (interstitialAD != null) {
            interstitialAD.destroy();
            interstitialAD = null;
        }

        if (rewardVideoAD != null) {
            rewardVideoAD.destroy();
            rewardVideoAD = null;
        }

        if (nativeAD != null) {
            nativeAD.destroy();
            nativeAD = null;
        }
    }

    //banner广告
    @Override
    public void loadAdViewAd(MaxAdapterResponseParameters parameters, MaxAdFormat maxAdFormat, Activity activity, final MaxAdViewAdapterListener listener) {
        String adId = parameters.getThirdPartyAdPlacementId();
        Log.d(TAG, "loadAdViewAd ad id:" + adId);
        if (TextUtils.isEmpty(adId)) {
            listener.onAdViewAdLoadFailed(MaxAdapterError.INVALID_CONFIGURATION);
            return;
        }
        bannerAD = new AlxBannerAD(activity);
        bannerAD.setRefresh(0);
        final AlxBannerADListener alxBannerADListener = new AlxBannerADListener() {
            @Override
            public void onAdLoaded(AlxBannerAD banner) {
                if (listener != null) {
                    listener.onAdViewAdLoaded(banner);
                }
            }

            @Override
            public void onAdError(AlxBannerAD banner, int errorCode, String errorMsg) {
                Log.e(TAG, "onAdError: errCode=" + errorCode + ";errMsg=" + errorMsg);
                if (listener != null) {
                    listener.onAdViewAdLoadFailed(MaxAdapterError.NO_FILL);
                }
            }

            @Override
            public void onAdClicked(AlxBannerAD banner) {
                if (listener != null) {
                    listener.onAdViewAdClicked();
                }
            }

            @Override
            public void onAdShow(AlxBannerAD banner) {
                if (listener != null) {
                    listener.onAdViewAdDisplayed();
                }
            }

            @Override
            public void onAdClose() {
                if (listener != null) {
                    listener.onAdViewAdHidden();
                }
            }
        };
        bannerAD.load(activity, adId, alxBannerADListener);
    }

    //插屏广告加载
    @Override
    public void loadInterstitialAd(MaxAdapterResponseParameters parameters, Activity activity, final MaxInterstitialAdapterListener listener) {
        String adId = parameters.getThirdPartyAdPlacementId();
        Log.d(TAG, "loadInterstitialAd ad id:" + adId);
        if (TextUtils.isEmpty(adId)) {
            listener.onInterstitialAdLoadFailed(MaxAdapterError.INVALID_CONFIGURATION);
            return;
        }
        interstitialAD = new AlxInterstitialAD();
        interstitialAD.load(activity, adId, new AlxInterstitialADListener() {
            @Override
            public void onInterstitialAdLoaded() {
                if (listener != null) {
                    listener.onInterstitialAdLoaded();
                }
            }

            @Override
            public void onInterstitialAdLoadFail(int errorCode, String errorMsg) {
                Log.e(TAG, "onInterstitialAdLoadFail: errCode=" + errorCode + ";errMsg=" + errorMsg);
                if (listener != null) {
                    listener.onInterstitialAdLoadFailed(MaxAdapterError.NO_FILL);
                }
            }

            @Override
            public void onInterstitialAdClicked() {
                if (listener != null) {
                    listener.onInterstitialAdClicked();
                }
            }

            @Override
            public void onInterstitialAdShow() {
                if (listener != null) {
                    listener.onInterstitialAdDisplayed();
                }
            }

            @Override
            public void onInterstitialAdClose() {
                if (listener != null) {
                    listener.onInterstitialAdHidden();
                }
            }

            @Override
            public void onInterstitialAdVideoStart() {

            }

            @Override
            public void onInterstitialAdVideoEnd() {

            }

            @Override
            public void onInterstitialAdVideoError(int errorCode, String errorMsg) {

            }
        });
    }

    //插屏广告展示
    @Override
    public void showInterstitialAd(MaxAdapterResponseParameters parameters, Activity activity, MaxInterstitialAdapterListener listener) {
        Log.d(TAG, "showInterstitialAd");
        if (interstitialAD != null && interstitialAD.isReady()) {
            interstitialAD.show(activity);
        } else {
            Log.d(TAG, "showInterstitialAd: ad no ready");
            listener.onInterstitialAdDisplayFailed(MaxAdapterError.AD_NOT_READY);
        }
    }

    //激励视频广告加载
    @Override
    public void loadRewardedAd(MaxAdapterResponseParameters parameters, Activity activity, final MaxRewardedAdapterListener listener) {
        String adId = parameters.getThirdPartyAdPlacementId();
        Log.d(TAG, "loadRewardedAd ad id:" + adId);
        if (TextUtils.isEmpty(adId)) {
            listener.onRewardedAdLoadFailed(MaxAdapterError.INVALID_CONFIGURATION);
            return;
        }
        rewardVideoAD = new AlxRewardVideoAD();
        rewardVideoAD.load(activity, adId, new AlxRewardVideoADListener() {
            @Override
            public void onRewardedVideoAdLoaded(AlxRewardVideoAD var1) {
                if (listener != null) {
                    listener.onRewardedAdLoaded();
                }
            }

            @Override
            public void onRewardedVideoAdFailed(AlxRewardVideoAD var1, int errCode, String errMsg) {
                Log.e(TAG, "onRewardedVideoAdFailed: errCode=" + errCode + ";errMsg=" + errMsg);
                if (listener != null) {
                    listener.onRewardedAdLoadFailed(MaxAdapterError.NO_FILL);
                }
            }

            @Override
            public void onRewardedVideoAdPlayStart(AlxRewardVideoAD var1) {
                if (listener != null) {
                    listener.onRewardedAdDisplayed();
                    listener.onRewardedAdVideoStarted();
                }
            }

            @Override
            public void onRewardedVideoAdPlayEnd(AlxRewardVideoAD var1) {
                if (listener != null) {
                    listener.onRewardedAdVideoCompleted();
                }
            }

            @Override
            public void onRewardedVideoAdPlayFailed(AlxRewardVideoAD var2, int errCode, String errMsg) {
                if (listener != null) {
                    listener.onRewardedAdDisplayFailed(new MaxAdapterError(errCode, errMsg));
                }
            }

            @Override
            public void onRewardedVideoAdClosed(AlxRewardVideoAD var1) {
                if (listener != null) {
                    listener.onRewardedAdHidden();
                }
            }

            @Override
            public void onRewardedVideoAdPlayClicked(AlxRewardVideoAD var1) {
                if (listener != null) {
                    listener.onRewardedAdClicked();
                }
            }

            @Override
            public void onReward(AlxRewardVideoAD var1) {
                if (listener != null) {
                    listener.onUserRewarded(getReward());
                }
            }
        });
    }

    //激励视频广告展示
    @Override
    public void showRewardedAd(MaxAdapterResponseParameters parameters, Activity activity, final MaxRewardedAdapterListener listener) {
        Log.d(TAG, "showRewardedAd");
        if (rewardVideoAD != null && rewardVideoAD.isReady()) {
            rewardVideoAD.showVideo(activity);
        } else {
            Log.d(TAG, "showRewardedAd: ad no ready");
            listener.onRewardedAdDisplayFailed(MaxAdapterError.AD_NOT_READY);
        }
    }

    //原生广告
    @Override
    public void loadNativeAd(MaxAdapterResponseParameters parameters, Activity activity, final MaxNativeAdAdapterListener listener) {
        String adId = parameters.getThirdPartyAdPlacementId();
        Log.d(TAG, "loadNativeAd ad id:" + adId);
        if (TextUtils.isEmpty(adId)) {
            listener.onNativeAdLoadFailed(MaxAdapterError.INVALID_CONFIGURATION);
            return;
        }
        //原生广告，不用模版
        AlxNativeAD.AlxAdSlot adSlot = new AlxNativeAD.AlxAdSlot()
                .setAdStyleType(AlxNativeAD.AlxAdSlot.TYPE_NATIVE_AD);// **必传，表示请求的模板广告还是原生广告
        AlxNativeAD nativeAD = new AlxNativeAD(activity, adId);

        NativeAdListener nativeAdListener = new NativeAdListener(parameters, activity, listener);
        nativeAD.load(adSlot, nativeAdListener);
    }

    private class NativeAdListener implements AlxNativeAdLoadListener {
        private MaxAdapterResponseParameters parameters;
        private Activity activity;
        private MaxNativeAdAdapterListener listener;

        public NativeAdListener(MaxAdapterResponseParameters parameters, Activity activity, MaxNativeAdAdapterListener listener) {
            this.parameters = parameters;
            this.activity = activity;
            this.listener = listener;
        }

        @Override
        public void onAdLoadedFail(int errorCode, String errorMsg) {
            Log.e(TAG, "native-onAdLoadedFail: errCode=" + errorCode + ";errMsg=" + errorMsg);
            if (listener != null) {
                listener.onNativeAdLoadFailed(MaxAdapterError.NO_FILL);
            }
        }

        @Override
        public void onAdLoaded(List<IAlxNativeInfo> ads) {
            if (ads == null || ads.isEmpty()) {
                if (listener != null) {
                    listener.onNativeAdLoadFailed(MaxAdapterError.NO_FILL);
                }
                return;
            }
            nativeAD = ads.get(0);
            if (nativeAD == null) {
                if (listener != null) {
                    listener.onNativeAdLoadFailed(MaxAdapterError.NO_FILL);
                }
                return;
            }

            nativeAD.setAlxNativeAdListener(new AlxNativeAdListener() {
                @Override
                public void onAdClick() {
                    if (listener != null) {
                        listener.onNativeAdClicked();
                    }
                }

                @Override
                public void onAdShow() {
                    if (listener != null) {
                        listener.onNativeAdDisplayed(null);
                    }
                }

                @Override
                public void onAdClose() {

                }
            });

            getCachingExecutorService().execute(new Runnable() {
                @Override
                public void run() {
                    Future<Drawable> iconDrawableFuture = null;
                    try {
                        if (nativeAD.getIcon() != null && !TextUtils.isEmpty(nativeAD.getIcon().getImageUrl()) && activity != null) {
                            iconDrawableFuture = createDrawableFuture(nativeAD.getIcon().getImageUrl(), activity.getResources());
                        }
                    } catch (Throwable th) {
                        e("Image fetching tasks failed", th);
                    }

                    Future<Drawable> imageDrawableFuture = null;
                    try {
                        if (nativeAD.getImageList() != null && nativeAD.getImageList().size() > 0) {
                            AlxImage image = nativeAD.getImageList().get(0);
                            if (image != null && !TextUtils.isEmpty(image.getImageUrl()) && activity != null) {
                                imageDrawableFuture = createDrawableFuture(image.getImageUrl(), activity.getResources());
                            }
                        }
                    } catch (Throwable th) {
                        e("Image fetching tasks failed", th);
                    }

                    Drawable iconDrawable = null;
                    Drawable mediaViewImageDrawable = null;
                    try {
                        // Execute and timeout tasks if incomplete within the given time
                        int imageTaskTimeoutSeconds = BundleUtils.getInt("image_task_timeout_seconds", DEFAULT_IMAGE_TASK_TIMEOUT_SECONDS, parameters.getServerParameters());
                        if (iconDrawableFuture != null) {
                            iconDrawable = iconDrawableFuture.get(imageTaskTimeoutSeconds, TimeUnit.SECONDS);
                        }
                        if (imageDrawableFuture != null) {
                            mediaViewImageDrawable = imageDrawableFuture.get(imageTaskTimeoutSeconds, TimeUnit.SECONDS);
                        }
                    } catch (Throwable th) {
                        e("Image fetching tasks failed", th);
                    }


                    final MaxNativeAd.MaxNativeAdImage icon = iconDrawable != null ? new MaxNativeAd.MaxNativeAdImage(iconDrawable) : null;
                    final Drawable finalMediaViewImageDrawable = mediaViewImageDrawable;

                    AppLovinSdkUtils.runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            View mediaView = null;
                            if (nativeAD == null) {
                                if (listener != null) {
                                    listener.onNativeAdLoadFailed(MaxAdapterError.NO_FILL);
                                }
                                return;
                            }
                            if (nativeAD.isExpressAd()) {
                                mediaView = nativeAD.getExpressView();
                            } else {
                                if (finalMediaViewImageDrawable != null && activity != null) {
                                    mediaView = new ImageView(activity);
                                    ((ImageView) mediaView).setImageDrawable(finalMediaViewImageDrawable);
                                }
                            }

                            if (mediaView == null) {
                                if (listener != null) {
                                    listener.onNativeAdLoadFailed(new MaxAdapterError(-5400, "Missing Native Ad Assets"));
                                }
                                return;
                            }

                            ImageView logoView = null;
                            if (nativeAD.getAdLogo() != null && activity != null) {
                                logoView = new ImageView(activity);
                                logoView.setImageBitmap(nativeAD.getAdLogo());
                            }

                            MaxNativeAd.Builder maxBuilder = new MaxNativeAd.Builder()
                                    .setAdFormat(MaxAdFormat.NATIVE)
                                    .setTitle(nativeAD.getTitle())
                                    .setBody(nativeAD.getDescription())
                                    .setCallToAction(nativeAD.getInteractionText())
                                    .setAdvertiser(AlxAdSDK.getNetWorkName())
                                    .setIcon(icon)
                                    .setMediaView(mediaView)
                                    .setOptionsView(logoView);

                            MaxNativeAd maxNativeAd = new MaxAlgorixNativeAd(maxBuilder);

                            if (listener != null) {
                                listener.onNativeAdLoaded(maxNativeAd, null);
                            }
                        }
                    });
                }
            });
        }
    }

    private class MaxAlgorixNativeAd extends MaxNativeAd {

        public MaxAlgorixNativeAd(Builder builder) {
            super(builder);
        }

        @Override
        public void prepareViewForInteraction(MaxNativeAdView maxNativeAdView) {
            if (nativeAD == null) {
                e("Failed to register native ad view for interaction. Native ad is null");
                return;
            }

            List<View> clickableViews = new ArrayList<>();
            if (AppLovinSdkUtils.isValidString(getTitle()) && maxNativeAdView.getTitleTextView() != null) {
                clickableViews.add(maxNativeAdView.getTitleTextView());
            }

            if (AppLovinSdkUtils.isValidString(getBody()) && maxNativeAdView.getBodyTextView() != null) {
                clickableViews.add(maxNativeAdView.getBodyTextView());
            }

            if (getIcon() != null && maxNativeAdView.getIconImageView() != null) {
                clickableViews.add(maxNativeAdView.getIconImageView());
            }

            if (getMediaView() != null && maxNativeAdView.getMediaContentViewGroup() != null) {
                clickableViews.add(maxNativeAdView.getMediaContentViewGroup());
            }

            // CTA button is considered a creative view
            if (AppLovinSdkUtils.isValidString(getCallToAction()) && maxNativeAdView.getCallToActionButton() != null) {
                clickableViews.add(maxNativeAdView.getCallToActionButton());
            }

            if (!nativeAD.isExpressAd()) {
                nativeAD.registerViewForInteraction(maxNativeAdView, clickableViews);
            }
        }
    }

}