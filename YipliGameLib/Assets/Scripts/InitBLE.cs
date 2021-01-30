#if UNITY_STANDALONE_WIN
using com.fitmat.fitmatdriver.Producer.Connection;
#endif

using System;
using UnityEngine;
public class InitBLE
{
    static AndroidJavaClass _pluginClass;
    static AndroidJavaObject _pluginInstance;
    const string driverPathName = "com.fitmat.fitmatdriver.Producer.Connection.DeviceControlActivity";
    static string BLEStatus = "";
    //STEP 3 - Create Unity Callback class
    class UnityCallback : AndroidJavaProxy
    {
        private Action<string> initializeHandler;
        public UnityCallback(Action<string> initializeHandlerIn) : base(driverPathName + "$UnityCallback")
        {
            initializeHandler = initializeHandlerIn;
        }
        public void sendMessage(string message)
        {
            Debug.Log("sendMessage: " + message);
            if (message == "connected")
            {
                BLEStatus = "CONNECTED";
            }
            if (message == "disconnected")
            {
                BLEStatus = "DISCONNECTED";
            }
            if (message == "lost")
            {
                BLEStatus = "CONNECTION LOST";
            }
            if (message.Contains("error"))
            {
                BLEStatus = "ERROR";
            }
            initializeHandler?.Invoke(message);
        }
    }

    public static string GetFMResponse()
    {
        try
        {
#if UNITY_ANDROID
                return PluginClass.CallStatic<string>("_getFMResponse");
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            return DeviceControlActivity._getFMResponse();
#endif
        }
        catch(Exception e)
        {
            Debug.Log("Exception in getMatConnectionStatus() : " + e.Message);
            return "error";
        }
    }

    //STEP 4 - Init Android Class & Objects
    public static AndroidJavaClass PluginClass
    {
        get
        {
            if (_pluginClass == null)
            {
                _pluginClass = new AndroidJavaClass(driverPathName);
            }
            return _pluginClass;
        }
    }
    public static AndroidJavaObject PluginInstance
    {
        get
        {
            if (_pluginInstance == null)
            {
                AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
                _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance", activity);
            }
            return _pluginInstance;
        }
    }

    public static string getMatConnectionStatus()
    {
        try
        {
#if UNITY_ANDROID && UNITY_EDITOR
            return "CONNECTED";
#elif UNITY_ANDROID
            return BLEStatus;
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            return DeviceControlActivity._IsDeviceConnected() == 1 ? "CONNECTED" : "DISCONNECTED";
#endif
        }
        catch (Exception e)
        {
            Debug.Log("Exception in getMatConnectionStatus() : " + e.Message);
            return "disconnected";
        }
    }

    public static void reconnectMat()
    {
        try
        {
#if UNITY_ANDROID
            System.Action<string> callback = ((string message) =>
            {
                BLEFramework.Unity.BLEControllerEventHandler.OnBleDidInitialize(message);
            });
            PluginInstance.Call("_InitBLEFramework", new object[] { new UnityCallback(callback) });
#elif UNITY_STANDALONE_WIN
        DeviceControlActivity._reconnectDevice();
#endif
        }
        catch (Exception e)
        {
            Debug.Log("Exception in reconnectMat() : " + e.Message);
        }
    }

    //STEP 5 - Init Android Class & Objects
    public static void InitBLEFramework(string macaddress, int gameID)
    {
        Debug.Log("init_ble: setting macaddress & gameID - " + macaddress + " " + gameID);
#if UNITY_IPHONE
            // Now we check that it's actually an iOS device/simulator, not the Unity Player. You only get plugins on the actual device or iOS Simulator.
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _InitBLEFramework();
            }
#elif UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                System.Action<string> callback = ((string message) =>
                {
                    BLEFramework.Unity.BLEControllerEventHandler.OnBleDidInitialize(message);
                });

                PluginInstance.Call("_setMACAddress", macaddress);
                setGameClusterID(gameID);
                PluginInstance.Call("_InitBLEFramework", new object[] { new UnityCallback(callback) });
                /*
                if(!setGameMode(0)){
                    Debug.Log("Failed to set Game Mode. Probable reason is your game doesnt support MultiPlayer functionality yet. ");
                }
                */
            }
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            Debug.Log("Calling DeviceControlActivity.InitPCFramework()");
            DeviceControlActivity.InitPCFramework(gameID);
#endif
    }


    public static void setGameMode(int gameMode)
    {
        try
        {
#if UNITY_ANDROID
                PluginInstance.Call("_setGameMode", gameMode);
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            DeviceControlActivity._setGameMode(gameMode);
#endif
        }
        catch (Exception e)
        {
            Debug.Log("Exception in _setGameMode() : " + e.Message);
        }
    }

    public static int getGameMode()
    {
        try
        {
#if UNITY_ANDROID
                return PluginInstance.CallStatic<int>("_getGameMode");
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            return DeviceControlActivity._getGameMode();
#endif
        }
        catch (Exception e)
        {
            Debug.Log("Exception in _getGameMode() : " + e.Message);
            return 1000;//1000 will be flagged as an invalid GameId on game side.
        }
    }


    public static void setGameClusterID(int gameID)
    {
        try
        {
#if UNITY_ANDROID
                PluginInstance.Call("_setGameID", gameID);
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            DeviceControlActivity._setGameID(gameID);
#endif
        }
        catch (Exception e)
        {
            Debug.Log("Exception in setGameClusterID() : " + e.Message);
        }
    }

    public static int getGameClusterID()
    {
        try
        {
#if UNITY_ANDROID
                return PluginInstance.Call<int>("_getGameID");
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            return DeviceControlActivity._getGameID();
#endif
        }
        catch (Exception e)
        {
            Debug.Log("Exception in getGameClusterID() : " + e.Message);
            return 1000;//1000 will be flagged as an invalid GameId on game side.
        }
    }

    public static string getFMDriverVersion()
    {
        try
        {
#if UNITY_ANDROID
                return PluginInstance.CallStatic<string>("_getDriverVersion");
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            return DeviceControlActivity._getDriverVersion();
#endif
        }
        catch (Exception exp)
        {
            Debug.Log("Exception in Driver Version" + exp.Message);
            return null;
        }
    }

    public static void setGameClusterID(int P1_gameID, int P2_gameID)
    {
        try
        {
#if UNITY_ANDROID
            PluginInstance.Call("_setGameID", P1_gameID, P2_gameID);
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
                DeviceControlActivity._setGameID(P1_gameID, P2_gameID);
#endif
        }
        catch (Exception e)
        {
            Debug.Log("Exception in setGameClusterID() : " + e.Message);
        }
    }

    public static int getGameClusterID(int playerID)
    {
        try
        {
#if UNITY_ANDROID
            return PluginInstance.Call<int>("_getGameID", playerID);
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
                return DeviceControlActivity._getGameID(playerID);
#endif
        }
        catch (Exception e)
        {
            Debug.Log("Exception in getGameClusterID() : " + e.Message);
            return 1000;//1000 will be flagged as an invalid GameId on game side.
        }
    }
}