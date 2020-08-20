using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YipliHelper
{
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

    public static bool checkInternetConnection()
    {
        bool bIsNetworkAvailable = true;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            bIsNetworkAvailable = false;
        }
        return bIsNetworkAvailable;
    }

    public static string GetBleConnectionStatus()
    {
        Debug.Log("GetBleConnectionStatus returning : " + InitBLE.getBLEStatus());
        if (PlayerSession.Instance.currentYipliConfig.matPlayMode == false)
            return "connected";
        return InitBLE.getBLEStatus();
    }

    public static void GoToYipli()
    {
        string bundleId = "org.hightimeshq.yipli"; //todo: Change this later
#if UNITY_ANDROID
        try
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

            AndroidJavaObject launchIntent = null;
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
            ca.Call("startActivity", launchIntent);
        }
        catch (AndroidJavaException e)
        {
            Debug.Log(e);
            Application.OpenURL("market://details?id=" + bundleId);
            //zeroPlayersText.text = "Yipli App is not installed. Install Yipli from market place to continue playing.";
        }
#endif
    }
}

