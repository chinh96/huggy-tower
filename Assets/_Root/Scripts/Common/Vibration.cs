using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public static class Vibration
{
#if UNITY_ANDROID
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
    public static AndroidJavaObject context;
    public static AndroidJavaClass vibrationEffect;
    public static int AndroidVersion
    {
        get
        {
            int iVersionNumber = 0;
            if (Application.platform == RuntimePlatform.Android)
            {
                string androidVersion = SystemInfo.operatingSystem;
                int sdkPos = androidVersion.IndexOf("API-");
                iVersionNumber = int.Parse(androidVersion.Substring(sdkPos + 4, 2).ToString());
            }
            return iVersionNumber;
        }
    }
#endif

    public static void Init()
    {
#if UNITY_ANDROID
        if (Application.isMobilePlatform)
        {

            unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

            if (AndroidVersion >= 26)
            {
                vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
            }
        }
#endif
    }

    public static void Vibrate()
    {
#if UNITY_ANDROID

        if (AndroidVersion >= 26)
        {
            AndroidJavaObject createOneShot = vibrationEffect.CallStatic<AndroidJavaObject>("createOneShot", 100, -1);
            vibrator.Call("vibrate", createOneShot);
        }
        else
        {
            vibrator.Call("vibrate", 100);
        }
#else
        Handheld.Vibrate ();
#endif
    }
}