package com.admob.custom.adapter;

import android.content.Context;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;

import com.alxad.api.AlxAdSDK;
import com.alxad.api.AlxBannerAD;
import com.alxad.api.AlxBannerADListener;
import com.alxad.api.AlxSdkInitCallback;
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.AdSize;
import com.google.android.gms.ads.mediation.Adapter;
import com.google.android.gms.ads.mediation.InitializationCompleteCallback;
import com.google.android.gms.ads.mediation.MediationAdRequest;
import com.google.android.gms.ads.mediation.MediationConfiguration;
import com.google.android.gms.ads.mediation.VersionInfo;
import com.google.android.gms.ads.mediation.customevent.CustomEventBanner;
import com.google.android.gms.ads.mediation.customevent.CustomEventBannerListener;

import org.json.JSONObject;

import java.util.List;

/**
 * Google AdMob 平台 Banner广告自定义事件适配器
 */
public class AlxBannerAdapter extends Adapter implements CustomEventBanner {

    private static final String TAG = "AlxBannerAdapter";
    private String unitid = "";
    private String appid = "";
    private String sid = "";
    private String token = "";
    private Boolean isdebug = false;
    AlxBannerAD mBannerView;
    AlxBannerAD.AlxAdSize alxAdSize = AlxBannerAD.AlxAdSize.SIZE_320_50;


    @Override
    public void requestBannerAd(final Context context, final CustomEventBannerListener customEventBannerListener, String s, final AdSize adSize, MediationAdRequest mediationAdRequest, Bundle bundle) {
        Log.d(TAG, "requestBannerAd");
        if (context == null) {
            throw new NullPointerException("context is empty object");
        }
        parseServer(s);
        if (TextUtils.isEmpty(appid)) {
            if (customEventBannerListener != null) {
                Log.d(TAG, "alx appid is empty");
                AdError error = new AdError(1, "alx appid is empty.", AlxAdSDK.getNetWorkName());
                customEventBannerListener.onAdFailedToLoad(error);
            }
            return;
        }

        if (TextUtils.isEmpty(sid)) {
            if (customEventBannerListener != null) {
                Log.d(TAG, "alx appkey is empty");
                AdError error = new AdError(1, "alx appkey is empty.", AlxAdSDK.getNetWorkName());
                customEventBannerListener.onAdFailedToLoad(error);
            }
            return;
        }

        if (TextUtils.isEmpty(unitid)) {
            if (customEventBannerListener != null) {
                Log.d(TAG, "alx unitid is empty");
                AdError error = new AdError(1, "alx unitid is empty.", AlxAdSDK.getNetWorkName());
                customEventBannerListener.onAdFailedToLoad(error);
            }
            return;
        }

        if (TextUtils.isEmpty(token)) {
            if (customEventBannerListener != null) {
                Log.d(TAG, "alx license is empty");
                AdError error = new AdError(1, "alx license is empty.", AlxAdSDK.getNetWorkName());
                customEventBannerListener.onAdFailedToLoad(error);
            }
            return;
        }

        try {
            Log.i(TAG, "alx token: " + token + " alx appid: " + appid + "alx sid: " + sid);
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
                    load(context, customEventBannerListener, adSize);
                }
            });

            AlxAdSDK.setDebug(isdebug);
        } catch (Exception e) {
            Log.e(TAG, e.getMessage() + "");
            e.printStackTrace();
            if (customEventBannerListener != null) {
                Log.d(TAG, "alx sdk init error");
                AdError error = new AdError(1, "alx sdk init error", AlxAdSDK.getNetWorkName());
                customEventBannerListener.onAdFailedToLoad(error);
            }
        }
    }

    @Override
    public void onDestroy() {
        Log.d(TAG, "onDestroy");
        if (mBannerView != null) {
            mBannerView.destory();
            mBannerView = null;
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

    private void load(Context context, final CustomEventBannerListener listener, AdSize adSize) {
        int width_dp = 0;
        int height_dp = 0;
        if (adSize != null) {
            width_dp = adSize.getWidth();
            height_dp = adSize.getHeight();
        }
        String size = width_dp + "x" + height_dp;
        Log.i(TAG, "width x height=" + size);
        switch (size) {
            case "300x250":
                alxAdSize = AlxBannerAD.AlxAdSize.SIZE_300_250;
                break;
            case "320x480":
                alxAdSize = AlxBannerAD.AlxAdSize.SIZE_320_480;
                break;
            default:
                alxAdSize = AlxBannerAD.AlxAdSize.SIZE_320_50;
                break;
        }


        mBannerView = new AlxBannerAD(context);
        // auto refresh ad  default = open = 1, 0 = close
        mBannerView.setRefresh(0);
        final AlxBannerADListener alxBannerADListener = new AlxBannerADListener() {
            @Override
            public void onAdLoaded(AlxBannerAD banner) {
                if (listener != null) {
                    listener.onAdLoaded(banner);
                }
            }

            @Override
            public void onAdError(AlxBannerAD banner, int errorCode, String errorMsg) {
                if (listener != null) {
                    AdError adError = new AdError(errorCode, errorMsg, AlxAdSDK.getNetWorkName());
                    listener.onAdFailedToLoad(adError);
                }
            }

            @Override
            public void onAdClicked(AlxBannerAD banner) {
                if (listener != null) {
                    listener.onAdClicked();
                }
            }

            @Override
            public void onAdShow(AlxBannerAD banner) {
                if (listener != null) {
                    listener.onAdOpened();
                }
            }

            @Override
            public void onAdClose() {
                if (listener != null) {
                    listener.onAdClosed();
                }
            }
        };
        mBannerView.load(context, unitid, alxBannerADListener, alxAdSize);
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