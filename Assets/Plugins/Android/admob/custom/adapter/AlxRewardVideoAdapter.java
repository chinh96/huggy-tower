package com.admob.custom.adapter;

import android.app.Activity;
import android.content.Context;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;

import com.alxad.api.AlxAdSDK;
import com.alxad.api.AlxRewardVideoAD;
import com.alxad.api.AlxRewardVideoADListener;
import com.alxad.api.AlxSdkInitCallback;
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.mediation.Adapter;
import com.google.android.gms.ads.mediation.InitializationCompleteCallback;
import com.google.android.gms.ads.mediation.MediationAdLoadCallback;
import com.google.android.gms.ads.mediation.MediationConfiguration;
import com.google.android.gms.ads.mediation.MediationRewardedAd;
import com.google.android.gms.ads.mediation.MediationRewardedAdCallback;
import com.google.android.gms.ads.mediation.MediationRewardedAdConfiguration;
import com.google.android.gms.ads.mediation.VersionInfo;
import com.google.android.gms.ads.rewarded.RewardItem;

import org.json.JSONObject;

import java.util.List;

/**
 * Google Mobile ads 激励广告适配器
 */
public class AlxRewardVideoAdapter extends Adapter implements MediationRewardedAd
        , AlxRewardVideoADListener {
    private final String TAG = "AlxRewardVideoAdapter";
    public final String AD_NETWORK_NAME = "Algorix";
    private static final String ALX_AD_UNIT_KEY = "parameter";

    private AlxRewardVideoAD alxRewardVideoAD;
    private String unitid = "";
    private String appid = "";
    private String sid = "";
    private String token = "";
    private Boolean isdebug = false;
    private Context mContext;
    private MediationAdLoadCallback<MediationRewardedAd, MediationRewardedAdCallback> mediationAdLoadCallBack;
    private MediationRewardedAdCallback mMediationRewardedAdCallback;

    @Override
    public void initialize(Context context, InitializationCompleteCallback initializationCompleteCallback
            , List<MediationConfiguration> list) {
        for (MediationConfiguration configuration : list) {
            Bundle serverParameters = configuration.getServerParameters();
            String serviceString = serverParameters.getString(ALX_AD_UNIT_KEY);
            if (!TextUtils.isEmpty(serviceString)) {
                parseServer(serviceString);
            }
        }
        if (initSDk(context)) {
            initializationCompleteCallback.onInitializationSucceeded();
        } else {
            initializationCompleteCallback.onInitializationFailed("alx sdk init error");
        }
    }

    @Override
    public VersionInfo getVersionInfo() {
        String versionString = AlxAdSDK.getNetWorkVersion();
        String[] splits = versionString.split("\\.");

        if (splits.length >= 3) {
            int major = Integer.parseInt(splits[0]);
            int minor = Integer.parseInt(splits[1]);
            int micro = Integer.parseInt(splits[2]) * 100 + Integer.parseInt(splits[3]);
            return new VersionInfo(major, minor, micro);
        }

        return new VersionInfo(0, 0, 0);
    }

    @Override
    public VersionInfo getSDKVersionInfo() {
        String versionString = AlxAdSDK.getNetWorkVersion();
        String[] splits = versionString.split("\\.");
        if (splits.length >= 3) {
            int major = Integer.parseInt(splits[0]);
            int minor = Integer.parseInt(splits[1]);
            int micro = Integer.parseInt(splits[2]);
            return new VersionInfo(major, minor, micro);
        }
        return new VersionInfo(0, 0, 0);
    }

    @Override
    public void showAd(Context context) {
        if (!(context instanceof Activity)) {
            Log.e(TAG, "context is not Activity");
            mMediationRewardedAdCallback.onAdFailedToShow(new AdError(1,
                    "An activity context is required to show Sample rewarded ad."
                    , AD_NETWORK_NAME)
            );
            return;
        }
        mContext = context;
        if (!alxRewardVideoAD.isReady()) {
            mMediationRewardedAdCallback.onAdFailedToShow(new AdError(1, "No ads to show."
                    , AD_NETWORK_NAME));
            return;
        }
        alxRewardVideoAD.showVideo((Activity) context);
    }

    @Override
    public void loadRewardedAd(MediationRewardedAdConfiguration configuration
            , MediationAdLoadCallback<MediationRewardedAd, MediationRewardedAdCallback> mediationAdLoadCallback) {
        Log.d(TAG, "loadRewardedAd: ");
        Context context = configuration.getContext();
        mediationAdLoadCallBack = mediationAdLoadCallback;
        Bundle serverParameters = configuration.getServerParameters();
        String serviceString = serverParameters.getString(ALX_AD_UNIT_KEY);
        if (!TextUtils.isEmpty(serviceString)) {
            parseServer(serviceString);
        }
        initSDk(context);
    }

    private boolean initSDk(final Context context) {
        if (TextUtils.isEmpty(unitid)) {
            Log.d(TAG, "alx unitid is empty");
            mediationAdLoadCallBack.onFailure(new AdError(1, "alx unitid is empty."
                    , AD_NETWORK_NAME));
            return false;
        }
        if (TextUtils.isEmpty(sid)) {
            Log.d(TAG, "alx sid is empty");
            mediationAdLoadCallBack.onFailure(new AdError(1, "alx sid is empty."
                    , AD_NETWORK_NAME));
            return false;
        }
        if (TextUtils.isEmpty(appid)) {
            Log.d(TAG, "alx appid is empty");
            mediationAdLoadCallBack.onFailure(new AdError(1, "alx appid is empty."
                    , AD_NETWORK_NAME));
            return false;
        }
        if (TextUtils.isEmpty(token)) {
            Log.d(TAG, "alx token is empty");
            mediationAdLoadCallBack.onFailure(new AdError(1, "alx token is empty"
                    , AD_NETWORK_NAME));
            return false;
        }
        try {
            Log.d(TAG, "alx token: " + token + " alx appid: " + appid + "alx sid: " + sid);
            // set GDPR
            //AlxAdSDK.setSubjectToGDPR(true);
            // set COPPA
            //AlxAdSDK.setBelowConsentAge(true);
            // set CCPA
            //AlxAdSDK.subjectToUSPrivacy("1YYY");
            // init
            AlxAdSDK.init(context, token, sid, appid, new AlxSdkInitCallback() {
                @Override
                public void onInit(boolean isOk, String msg) {
                    //sdk init success, begin load ad
                    alxRewardVideoAD = new AlxRewardVideoAD();
                    alxRewardVideoAD.load(context, unitid, AlxRewardVideoAdapter.this);
                }
            });

            AlxAdSDK.setDebug(isdebug);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return true;
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


    @Override
    public void onRewardedVideoAdLoaded(AlxRewardVideoAD var1) {
        Log.d(TAG, "onRewardedVideoAdLoaded");
        if (mediationAdLoadCallBack != null)
            mMediationRewardedAdCallback = (MediationRewardedAdCallback) mediationAdLoadCallBack
                    .onSuccess(this);
    }


    @Override
    public void onRewardedVideoAdFailed(AlxRewardVideoAD var1, int errCode, String errMsg) {
        Log.d(TAG, "onRewardedVideoAdFailed: " + errMsg);
        if (mediationAdLoadCallBack != null) mediationAdLoadCallBack
                .onFailure(new AdError(errCode, errMsg, AD_NETWORK_NAME));
    }

    @Override
    public void onRewardedVideoAdPlayStart(AlxRewardVideoAD var1) {
        if (mMediationRewardedAdCallback != null && mContext instanceof Activity) {
            ((Activity) mContext).runOnUiThread(() -> {
                // runOnUiThread
                mMediationRewardedAdCallback.reportAdImpression();
                mMediationRewardedAdCallback.onAdOpened();
                mMediationRewardedAdCallback.onVideoStart();
            });
        }
    }

    @Override
    public void onRewardedVideoAdPlayEnd(AlxRewardVideoAD var1) {
        Log.d(TAG, "onRewardedVideoAdPlayEnd: ");
        if (mMediationRewardedAdCallback != null) mMediationRewardedAdCallback.onVideoComplete();
    }

    @Override
    public void onRewardedVideoAdPlayFailed(AlxRewardVideoAD var2, int errCode, String errMsg) {
        Log.d(TAG, "onShowFail: " + errMsg);
        if (mMediationRewardedAdCallback != null) mMediationRewardedAdCallback.onAdFailedToShow(
                new AdError(errCode, errMsg, AD_NETWORK_NAME));
    }

    @Override
    public void onRewardedVideoAdClosed(AlxRewardVideoAD var1) {
        Log.d(TAG, "onRewardedVideoAdClosed: ");
        if (mMediationRewardedAdCallback != null) {
            mMediationRewardedAdCallback.onAdClosed();
        }
    }

    @Override
    public void onRewardedVideoAdPlayClicked(AlxRewardVideoAD var1) {
        Log.d(TAG, "onRewardedVideoAdPlayClicked: ");
        if (mMediationRewardedAdCallback != null) mMediationRewardedAdCallback.reportAdClicked();
    }

    @Override
    public void onReward(AlxRewardVideoAD var1) {
        Log.d(TAG, "onReward: ");
        if (mMediationRewardedAdCallback != null) {
            mMediationRewardedAdCallback.onUserEarnedReward(new RewardItem() {
                @Override
                public String getType() {
                    return "";
                }

                @Override
                public int getAmount() {
                    return 1;
                }
            });
        }
    }
}
