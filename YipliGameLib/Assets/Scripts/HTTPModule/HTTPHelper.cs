using System;
using UnityEngine;
using yipli.Windows;

namespace Yipli.HttpMpdule
{
    public class HTTPHelper
    {
        public static int GetGameClusterId()
        {
            return InitBLE.getGameClusterID();
        }
     
        public static bool CheckInternetConnection()
        {
            var ping = new System.Net.NetworkInformation.Ping();

            var result = ping.Send("www.google.com");

            return (result.Status == System.Net.NetworkInformation.IPStatus.Success);
        }

        public static string GetMatConnectionStatus()
        {
            return "connected";
        }

        public static void GoToPlaystoreUpdate(string gameAppId)
        {
            #if UNITY_ANDROID
                Application.OpenURL("market://details?id=" + gamePackageId);
            #else
                UnityEngine.Debug.Log("Unsupported os");
            #endif
        }

        public static void GoToYipli(string direction = "NoDir", string gameID = "NoID")
        {
            #if UNITY_ANDROID || UNITY_IOS

                    switch(direction)
                    {
                        case ProductMessages.noMatCase:
                            Debug.LogError("case : " + ProductMessages.noMatCase);
                            Application.OpenURL(ProductMessages.AddMatAppPageUrl);
                            break;

                        case ProductMessages.noUserFound:
                            Debug.LogError("case : " + ProductMessages.noUserFound);
                            Application.OpenURL(ProductMessages.UserFoundAppPageUrl);
                            break;

                        case ProductMessages.noPlayerAdded:
                            Debug.LogError("case : " + ProductMessages.noPlayerAdded);
                            Application.OpenURL(ProductMessages.AddPlayerAppPageUrl);
                            break;

                        case ProductMessages.relaunchGame:
                            Debug.LogError("case : " + ProductMessages.relaunchGame);
                            // Application.OpenURL(ProductMessages.RelaunchGameUrl + Application.identifier); // provide full package id
                            Application.OpenURL(ProductMessages.RelaunchGameUrl + gameID);
                            break;

                        case ProductMessages.openYipliApp:
                            Debug.LogError("case : " + ProductMessages.openYipliApp);
                            Application.OpenURL(ProductMessages.OpenYipliAppUrl);
                            break;

                        default:
                            Debug.LogError("case : default");
                            Application.OpenURL(ProductMessages.OpenYipliAppUrl);
                            break;
                    }
            #elif UNITY_STANDALONE_WIN
                    FileReadWrite.OpenYipliApp();
            #else
                    Debug.Log("Unsupported os");
            #endif
        }

        public static string GetFMDriverVersion()
        {
            return InitBLE.getFMDriverVersion();
        }

        // age from Dob
        public static string CalculateAge(string strDob /* 'mm-dd-yyyy' format */)
        {
            DateTime now = DateTime.Now;
            string[] tokens = strDob.Split('-');
            int month = int.Parse(tokens[0]);
            int year = int.Parse(tokens[2]);
            int day = int.Parse(tokens[1]);
            var years = now.Year - year;
            if (now.Month < month)
            {
                years--;
            }
            if (now.Month == month)
            {
                if (now.Day < day)
                {
                    years--;
                }
            }
            return years.ToString();
        }

        public static void SetGameClusterId(int clusterId)
        {
            InitBLE.setGameClusterID(clusterId);
        }
    }
}