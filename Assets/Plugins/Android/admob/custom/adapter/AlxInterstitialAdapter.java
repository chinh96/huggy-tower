package com.admob.custom.adapter;

import android.app.Activity;
import android.content.Context;
import android.os.Bundle;
import android.os.Handler;
import android.text.TextUtils;
import android.util.Log;

import com.alxad.api.AlxAdSDK;
import com.alxad.api.AlxInterstitialAD;
import com.alxad.api.AlxInterstitialADListener;
import com.alxad.api.AlxSdkInitCallback;
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.mediation.Adapter;
import com.google.android.gms.ads.mediation.InitializationCompleteCallback;
import com.google.android.gms.ads.mediation.MediationAdRequest;
import com.google.android.gms.ads.mediation.MediationConfiguration;
import com.google.android.gms.ads.mediation.VersionInfo;
import com.google.android.gms.ads.mediation.customevent.CustomEventInterstitial;
import com.google.android.gms.ads.mediation.customevent.CustomEventInterstitialListener;

import org.json.JSONObject;

import java.util.List;

/**
 * Google AdMob 平台 插屏广告自定义事件适配器
 */
public class AlxInterstitialAdapter extends Adapter implements CustomEventInterstitial {

    private static final String TAG = "AlxInterstitialAdapter";
    private String unitid = "";
    private String appid = "";
    private String sid = "";
    private String token = "";
    private Boolean isdebug = false;
    private CustomEventInterstitialListener mListener;

    AlxInterstitialAD alxInterstitialAD;
    private Context mContext;
    private Handler mHandler;

    @Override
    public void requestInterstitialAd(Context context, CustomEventInterstitialListener customEventInterstitialListener, String s, MediationAdRequest mediationAdRequest, Bundle bundle) {
        Log.i(TAG, "loadCustomNetworkAd");
        mContext = context;
        mHandler = new Handler(mContext.getMainLooper());
        mListener = customEventInterstitialListener;
        parseServer(s);
        if (TextUtils.isEmpty(sid)) {
            if (mListener != null) {
                Log.i(TAG, "alx sid is empty");
                mHandler.post(new Runnable() {
                    @Override
                    public void run() {
                        AdError error = new AdError(1, "alx sid is empty.", AlxAdSDK.getNetWorkName());
                        mListener.onAdFailedToLoad(error);
                    }
                });
            }
            return;
        }

        if (TextUtils.isEmpty(appid)) {
            if (mListener != null) {
                Log.i(TAG, "alx appid is empty");
                mHandler.post(new Runnable() {
                    @Override
                    public void run() {
                        AdError error = new AdError(1, "alx appid is empty.", AlxAdSDK.getNetWorkName());
                        mListener.onAdFailedToLoad(error);
                    }
                });
            }
            return;
        }

        if (TextUtils.isEmpty(unitid)) {
            if (mListener != null) {
                Log.i(TAG, "alx unitid is empty");
                mHandler.post(new Runnable() {
                    @Override
                    public void run() {
                        AdError error = new AdError(1, "alx unitid is empty.", AlxAdSDK.getNetWorkName());
                        mListener.onAdFailedToLoad(error);
                    }
                });
            }
            return;
        }

        if (TextUtils.isEmpty(token)) {
            if (mListener != null) {
                Log.i(TAG, "alx token is empty");
                mHandler.post(new Runnable() {
                    @Override
                    public void run() {
                        AdError error = new AdError(1, "alx token is empty.", AlxAdSDK.getNetWorkName());
                        mListener.onAdFailedToLoad(error);
                    }
                });
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
            // init
            AlxAdSDK.init(context, token, sid, appid, new AlxSdkInitCallback() {
                @Override
                public void onInit(boolean isOk, String msg) {
                    preloadAd(context);
                }
            });
            AlxAdSDK.setDebug(isdebug);
        } catch (Exception e) {
            Log.e(TAG, e.getMessage());
            e.printStackTrace();
            if (mListener != null) {
                Log.i(TAG, "alx sdk init error");
                mHandler.post(new Runnable() {
                    @Override
                    public void run() {
                        if (mListener != null) {
                            AdError error = new AdError(1, "alx sdk init error", AlxAdSDK.getNetWorkName());
                            mListener.onAdFailedToLoad(error);
                        }
                    }
                });
            }
            return;
        }

    }

    @Override
    public void showInterstitial() {
        if (mContext != null) {
            if (mContext instanceof Activity) {
                if (alxInterstitialAD != null) {
                    alxInterstitialAD.show((Activity) mContext);
                }
            } else {
                Log.i(TAG, "context is not an Activity");
                alxInterstitialAD.show(null);
            }
        }
    }

    @Override
    public void onDestroy() {
        if (alxInterstitialAD != null) {
            alxInterstitialAD.destroy();
            alxInterstitialAD = null;
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
            appid = json.getString("appid");
            sid = json.getString("sid");
            token = json.getString("token");
            unitid = json.getString("unitid");
            isdebug = json.optBoolean("isdebug");
        } catch (Exception e) {
            Log.e(TAG, e.getMessage() + "");
        }
        if (TextUtils.isEmpty(appid) || TextUtils.isEmpty(sid) || TextUtils.isEmpty(token)) {
            try {
                JSONObject json = new JSONObject(s);
                appid = json.getString("appid");
                sid = json.getString("appkey");
                token = json.getString("license");
                unitid = json.getString("unitid");
                isdebug = json.optBoolean("isdebug");
            } catch (Exception e) {
                Log.e(TAG, e.getMessage() + "");
            }
        }
    }

    private void preloadAd(final Context context) {
        alxInterstitialAD = new AlxInterstitialAD();
        alxInterstitialAD.load(context, unitid, new AlxInterstitialADListener() {

            @Override
            public void onInterstitialAdLoaded() {
                if (mListener != null) {
                    mListener.onAdLoaded();
                }
            }

            @Override
            public void onInterstitialAdLoadFail(int errorCode, String errorMsg) {
                if (mListener != null) {
                    AdError error = new AdError(1, errorMsg, AlxAdSDK.getNetWorkName());
                    mListener.onAdFailedToLoad(error);
                }
            }

            @Override
            public void onInterstitialAdClicked() {
                if (mListener != null) {
                    mListener.onAdClicked();
                }
            }

            @Override
            public void onInterstitialAdShow() {
                if (mListener != null) {
                    mListener.onAdOpened();
                }
            }

            @Override
            public void onInterstitialAdClose() {
                if (mListener != null) {
                    mHandler.post(new Runnable() {
                        @Override
                        public void run() {
                            mListener.onAdClosed();
                        }
                    });
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