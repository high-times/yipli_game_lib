using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YipliHelper
{
    public static bool checkInternetConnection()
    {
        bool bIsNetworkAvailable = true;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            bIsNetworkAvailable = false;
        }
        return bIsNetworkAvailable;
    }
}

