package com.admob.custom.adapter;


import android.content.Context;
import android.graphics.drawable.Drawable;
import android.net.Uri;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;

import com.alxad.api.AlxAdSDK;
import com.alxad.api.AlxNativeAD;
import com.alxad.api.AlxNativeAdListener;
import com.alxad.api.AlxNativeAdLoadListener;
import com.alxad.api.AlxNativeExpressAdListener;
import com.alxad.api.AlxSdkInitCallback;
import com.alxad.api.IAlxNativeInfo;
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.formats.NativeAd;
import com.google.android.gms.ads.mediation.Adapter;
import com.google.android.gms.ads.mediation.InitializationCompleteCallback;
import com.google.android.gms.ads.mediation.MediationConfiguration;
import com.google.android.gms.ads.mediation.NativeMediationAdRequest;
import com.google.android.gms.ads.mediation.UnifiedNativeAdMapper;
import com.google.android.gms.ads.mediation.VersionInfo;
import com.google.android.gms.ads.mediation.customevent.CustomEventNative;
import com.google.android.gms.ads.mediation.customevent.CustomEventNativeListener;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

/**
 * Google AdMob 平台 信息流广告自定义事件适配器
 */
public class AlxNativeAdapter extends Adapter implements CustomEventNative {

    private static final String TAG = "AlxNativeAdapter";
    private final int AD_STYLE_TYPE_EXPRESS = 0;
    private final int AD_STYLE_TYPE_NATIVE = 1;

    private String unitid = "";
    private String appid = "";
    private String sid = "";
    private String token = "";
    private Boolean isdebug = false;
    private int mImageWidth; //请求广告图的宽度：单位px  | Native AD Width
    private int mImageHeight; //请求广告图的高度: 单位px | Native AD Height
    private int mAdStyleType = AD_STYLE_TYPE_NATIVE; //广告样式类型： 0：是模版广告，1是原生广告

    private AlxNativeAD mNativeAD;

    @Override
    public void requestNativeAd(Context context, CustomEventNativeListener customEventNativeListener, String s, NativeMediationAdRequest nativeMediationAdRequest, Bundle bundle) {
        Log.i(TAG, "requestNativeAd");
        if (context == null) {
            throw new NullPointerException("context is empty object");
        }
        parseServer(s);
        if (TextUtils.isEmpty(sid)) {
            if (customEventNativeListener != null) {
                Log.i(TAG, "alx appid is empty");
                AdError error = new AdError(1, "alx appid is empty.", AlxAdSDK.getNetWorkName());
                customEventNativeListener.onAdFailedToLoad(error);
            }
            return;
        }

        if (TextUtils.isEmpty(appid)) {
            if (customEventNativeListener != null) {
                Log.i(TAG, "alx appkey is empty");
                AdError error = new AdError(1, "alx appkey is empty.", AlxAdSDK.getNetWorkName());
                customEventNativeListener.onAdFailedToLoad(error);
            }
            return;
        }

        if (TextUtils.isEmpty(unitid)) {
            if (customEventNativeListener != null) {
                Log.i(TAG, "alx unitid is empty");
                AdError error = new AdError(1, "alx unitid is empty.", AlxAdSDK.getNetWorkName());
                customEventNativeListener.onAdFailedToLoad(error);
            }
            return;
        }

        if (TextUtils.isEmpty(token)) {
            if (customEventNativeListener != null) {
                Log.i(TAG, "alx license is empty");
                AdError error = new AdError(1, "alx license is empty.", AlxAdSDK.getNetWorkName());
                customEventNativeListener.onAdFailedToLoad(error);
            }
            return;
        }
        initSdk(context, customEventNativeListener);
    }

    private void initSdk(final Context context, final CustomEventNativeListener customEventNativeListener) {
        try {
            Log.i(TAG, "alx ver:" + AlxAdSDK.getNetWorkVersion() + " alx license: " + token + " alx appkey: " + appid + " alx appid: " + sid);

            AlxAdSDK.setDebug(isdebug);
//            // set GDPR
            AlxAdSDK.setSubjectToGDPR(true);
//            // set COPPA
            AlxAdSDK.setBelowConsentAge(true);
            AlxAdSDK.setUserConsent("0");
//            // set CCPA
            AlxAdSDK.subjectToUSPrivacy("1YYY");
            // init
            AlxAdSDK.init(context, token, sid, appid, new AlxSdkInitCallback() {
                @Override
                public void onInit(boolean isOk, String msg) {
                    loadAds(context, customEventNativeListener);
                }
            });
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    @Override
    public void onDestroy() {
        if (mNativeAD != null) {
            mNativeAD.destroy();
        }
    }

    @Override
    public void onPause() {

    }

    @Override
    public void onResume() {

    }

    private void parseServer(String s) {
        if (TextUtils.isEmpty(s)) {
            Log.d(TAG, "serviceString  is empty ");
            return;
        }
        Log.d(TAG, "serviceString   " + s);
        try {
            JSONObject json = new JSONObject(s);
            token = json.getString("token");
            sid = json.getString("sid");
            appid = json.getString("appid");
            unitid = json.getString("unitid");
            isdebug = json.optBoolean("isdebug");
        } catch (Exception e) {
            Log.e(TAG, e.getMessage() + "");
        }
        if (TextUtils.isEmpty(appid) || TextUtils.isEmpty(sid) || TextUtils.isEmpty(token)) {
            try {
                JSONObject json = new JSONObject(s);
                token = json.getString("license");
                sid = json.getString("appkey");
                appid = json.getString("appid");
                unitid = json.getString("unitid");
                isdebug = json.optBoolean("isdebug");
            } catch (Exception e) {
                Log.e(TAG, e.getMessage() + "");
            }
        }
    }

    private void loadAds(Context context, final CustomEventNativeListener listener) {
        AlxNativeAdLoadListener loadListener = new AlxNativeAdLoadListener() {
            @Override
            public void onAdLoadedFail(int errorCode, String errorMsg) {
                Log.i(TAG, "onAdLoadedFail:" + errorCode + ";" + errorMsg);
                if (listener != null) {
                    AdError adError = new AdError(errorCode, errorMsg, AlxAdSDK.getNetWorkName());
                    listener.onAdFailedToLoad(adError);
                }
            }

            @Override
            public void onAdLoaded(final List<IAlxNativeInfo> ads) {
                Log.i(TAG, "onAdLoaded:");

                if (ads == null || ads.isEmpty()) {
                    if (listener != null) {
                        AdError adError = new AdError(100, "no data ads", AlxAdSDK.getNetWorkName());
                        listener.onAdFailedToLoad(adError);
                    }
                    return;
                }

                try {
                    IAlxNativeInfo bean = ads.get(0);
                    if (bean == null) {
                        if (listener != null) {
                            AdError adError = new AdError(100, "no data ads", AlxAdSDK.getNetWorkName());
                            listener.onAdFailedToLoad(adError);
                        }
                        return;
                    }
                    TransformData result = new TransformData(bean, listener);
                    result.setData();
                    if (listener != null) {
                        Log.i(TAG, "onAdLoaded:listener-ok");
                        listener.onAdLoaded(result);
                    }
                } catch (Exception e) {
                    Log.e(TAG, e.getMessage());
                    e.printStackTrace();
                    if (listener != null) {
                        Log.i(TAG, "onAdFailedToLoad:listener-error");
                        AdError adError = new AdError(101, e.getMessage(), AlxAdSDK.getNetWorkName());
                        listener.onAdFailedToLoad(adError);
                    }
                }

            }
        };

        int adStyleType = AlxNativeAD.AlxAdSlot.TYPE_NATIVE_AD;
        if (AD_STYLE_TYPE_EXPRESS == mAdStyleType) {
            adStyleType = AlxNativeAD.AlxAdSlot.TYPE_EXPRESS_AD;
        }
        mNativeAD = new AlxNativeAD(context, unitid);
        AlxNativeAD.AlxAdSlot adSlot = new AlxNativeAD.AlxAdSlot();
        adSlot.setAdStyleType(adStyleType);
        if (mImageWidth > 1 && mImageHeight > 1) {
            adSlot.setAdImageSize(mImageWidth, mImageHeight);
        }
        mNativeAD.load(adSlot, loadListener);
    }

    @Override
    public void initialize(Context context, InitializationCompleteCallback initializationCompleteCallback, List<MediationConfiguration> list) {
        if (context == null) {
            initializationCompleteCallback.onInitializationFailed(
                    "Initialization Failed: Context is null.");
            return;
        }
        initializationCompleteCallback.onInitializationSucceeded();
    }

    @Override
    public VersionInfo getVersionInfo() {
        String versionString = AlxAdSDK.getNetWorkVersion();
        VersionInfo result = getAdapterVersionInfo(versionString);
        if (result != null) {
            return result;
        }
        return new VersionInfo(0, 0, 0);
    }

    @Override
    public VersionInfo getSDKVersionInfo() {
        String versionString = AlxAdSDK.getNetWorkVersion();
        VersionInfo result = getAdapterVersionInfo(versionString);
        if (result != null) {
            return result;
        }
        return new VersionInfo(0, 0, 0);
    }

    private class TransformData extends UnifiedNativeAdMapper {
        private IAlxNativeInfo mAlxAd;
        private CustomEventNativeListener mListener;

        public static final double SAMPLE_SDK_IMAGE_SCALE = 1.0;

        public TransformData(IAlxNativeInfo obj, CustomEventNativeListener listener) {
            mAlxAd = obj;
            mListener = listener;
        }

        public void setData() {
            bindListener();

            setHeadline(mAlxAd.getTitle());
            setBody(mAlxAd.getDescription());
            setCallToAction(mAlxAd.getInteractionText());
            setAdvertiser(AlxAdSDK.getNetWorkName());
            setIcon(new SampleNativeMappedImage(null, mAlxAd.getIconUrl(), SAMPLE_SDK_IMAGE_SCALE));

            List<NativeAd.Image> imageList = new ArrayList<>();
            imageList.add(new SampleNativeMappedImage(null, mAlxAd.getImageUrl(), SAMPLE_SDK_IMAGE_SCALE));
            setImages(imageList);

            setOverrideClickHandling(true);
            setOverrideImpressionRecording(true);

            if (mAlxAd.isExpressAd()) {
                setMediaView(mAlxAd.getExpressView());
            }
        }

        @Override
        public void recordImpression() {
            Log.d(TAG, "recordImpression");
        }

        @Override
        public void handleClick(View view) {
            Log.d(TAG, "handleClick:" + view.getId());
        }

        @Override
        public void trackViews(View view, Map<String, View> clickableAssetViews, Map<String, View> nonClickableAssetViews) {
            super.trackViews(view, clickableAssetViews, nonClickableAssetViews);
            Log.d(TAG, "trackViews");
            try {
                List<View> childViews = new ArrayList<>();
                if (clickableAssetViews != null && !clickableAssetViews.isEmpty()) {
                    for (Map.Entry<String, View> entry : clickableAssetViews.entrySet()) {
                        childViews.add(entry.getValue());
                    }
                }
                mAlxAd.registerViewForInteraction((ViewGroup) view, childViews);

                if (mAlxAd.isExpressAd()) {
                    mAlxAd.render();
                }
            } catch (Exception e) {
                e.printStackTrace();
                Log.e(TAG, e.getMessage());
            }
        }

        private void bindListener() {
            if (mAlxAd == null) {
                return;
            }
            if (mAlxAd.isExpressAd()) {
                mAlxAd.setAlxNativeAdListener(new AlxNativeExpressAdListener() {
                    @Override
                    public void onAdClose() {
                        if (mListener != null) {
                            mListener.onAdClosed();
                        }
                    }

                    @Override
                    public void onRenderFail(int code, String msg) {

                    }

                    @Override
                    public void onRenderSuccess(View view) {

                    }

                    @Override
                    public void onAdClick() {
                        if (mListener != null) {
                            mListener.onAdClicked();
                        }
                    }

                    @Override
                    public void onAdShow() {
                        if (mListener != null) {
                            mListener.onAdImpression();
                        }
                    }
                });
            } else {
                mAlxAd.setAlxNativeAdListener(new AlxNativeAdListener() {
                    @Override
                    public void onAdClick() {
                        if (mListener != null) {
                            mListener.onAdClicked();
                        }
                    }

                    @Override
                    public void onAdShow() {
                        if (mListener != null) {
                            mListener.onAdImpression();
                        }
                    }

                    @Override
                    public void onAdClose() {
                        if (mListener != null) {
                            mListener.onAdClosed();
                        }
                    }
                });
            }

        }

    }

    private static class SampleNativeMappedImage extends NativeAd.Image {

        private Drawable drawable;
        private Uri imageUri;
        private double scale;

        public SampleNativeMappedImage(Drawable drawable, String url, double scale) {
            this.drawable = drawable;
            if (!TextUtils.isEmpty(url)) {
                this.imageUri = Uri.parse(url);
            }
            this.scale = scale;
        }

        @Override
        public Drawable getDrawable() {
            return drawable;
        }

        @Override
        public Uri getUri() {
            return imageUri;
        }

        @Override
        public double getScale() {
            return scale;
        }
    }


    private VersionInfo getAdapterVersionInfo(String version) {
        if (TextUtils.isEmpty(version)) {
            return null;
        }
        try {
            String[] arr = version.split("\\.");
            if (arr == null || arr.length < 3) {
                return null;
            }
            int major = Integer.parseInt(arr[0]);
            int minor = Integer.parseInt(arr[1]);
            int micro = Integer.parseInt(arr[2]);
            return new VersionInfo(major, minor, micro);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return null;
    }
}
