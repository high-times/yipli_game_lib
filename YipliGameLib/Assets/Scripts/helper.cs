using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class YipliHelper
{
    private static string yipliAppBundleId = "org.hightimeshq.yipli"; //todo: Change this later

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
            //zeroPlayersText.text = "Yipli App is not installed. Install Yipli from market place to continue playing.";
        }
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
          
        return true;
#else
        Debug.Log("OS not supported. Returnin false.");
         return false;
#endif
    }
}

