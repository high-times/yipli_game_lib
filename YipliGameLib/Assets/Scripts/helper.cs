using System;
using UnityEngine;

public static class YipliHelper
{
    private static string yipliAppBundleId = "com.yipli.app"; //todo: Change this later

    public static int GetGameClusterId()
    {
        return InitBLE.getGameClusterID();
    }

    public static string GetFMDriverVersion()
    {
        return InitBLE.getFMDriverVersion();
    }

    public static void SetGameClusterId(int gameClusterId)
    {
        InitBLE.setGameClusterID(gameClusterId);
    }

    public static void SetGameMode(int gameMode)
    {
        Debug.Log("GameMode: " + gameMode);
        InitBLE.setGameMode(gameMode);
    }

    public static bool checkInternetConnection()
    {
        bool bIsNetworkAvailable = true;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            bIsNetworkAvailable = false;
        }
        return bIsNetworkAvailable;
    }

    public static string GetMatConnectionStatus()
    {
        if(!PlayerSession.Instance.currentYipliConfig.onlyMatPlayMode)
            return "connected";
        Debug.Log("GetBleConnectionStatus returning : " + InitBLE.getMatConnectionStatus());
        return InitBLE.getMatConnectionStatus();
    }


    public static void GoToPlaystoreUpdate(string gamePackageId)
    {
#if UNITY_ANDROID
            Application.OpenURL("market://details?id=" + gamePackageId);
#else
        Debug.Log("Unsupported os");
#endif
    }

    public static void GoToYipli()
    {
#if UNITY_ANDROID
        try
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

            AndroidJavaObject launchIntent = null;
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", yipliAppBundleId);
            ca.Call("startActivity", launchIntent);
        }
        catch (AndroidJavaException e)
        {
            Debug.Log(e);
            Application.OpenURL("market://details?id=" + yipliAppBundleId);
        }
#else
        Debug.Log("Unsupported os");
#endif
    }


    //Returns true if YipliApp is installed, else returns false
    public static bool IsYipliAppInstalled()
    {
#if UNITY_ANDROID
        AndroidJavaObject launchIntent = null;
        try
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
            Debug.Log(" Quering if Yipli App is installed.");
            
            //if the app is installed, no errors. Else, doesn't get past next line

            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", yipliAppBundleId);
        }
        catch (Exception ex)
        {
            Debug.Log("exception" + ex.Message);
        }
        if (launchIntent == null)
        {
            Debug.Log("Yipli app is not installed.");
            return false;
        }

        Debug.Log("Yipli App is Installed. Returning true.");
        return true;

#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
        Debug.Log("Yipli App validation for windows isnt required. Returning true");
        return true;
#else
        Debug.Log("OS not supported. Returnin false.");
        return false;
#endif
    }
}

