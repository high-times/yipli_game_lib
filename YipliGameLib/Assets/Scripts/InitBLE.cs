using System;
using UnityEngine;
public class InitBLE
{
    static AndroidJavaClass _pluginClass;
    static AndroidJavaObject _pluginInstance;
    const string driverPathName = "com.fitmat.fitmatdriver.Producer.Connection.DeviceControlActivity";
    string FMResponseCount = "";
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
    public static string getBLEStatus()
    {
        return BLEStatus;
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
#endif
    }


    public static bool setGameMode(int gameMode)
    {
        try
        {
            return PluginInstance.Call<bool>("_setGameMode", gameMode);
        }
        catch (Exception e)
        {
            Debug.Log("Exception in _setGameMode() : " + e.Message);
        }
        return false;
    }


    public static void setGameClusterID(int gameID)
    {
        try
        {
            PluginInstance.Call("_setGameID", gameID);
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
            return PluginInstance.CallStatic<int>("_getGameID");
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
            return PluginInstance.CallStatic<string>("_getDriverVersion");
        }
        catch (Exception exp)
        {
            Debug.Log("Exception in Driver Version" + exp.Message);
            return null;
        }
    }
}