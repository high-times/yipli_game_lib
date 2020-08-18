using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSession : MonoBehaviour
{
    private string userId = ""; // to be recieved from Yipli
    private string gameId = ""; // to be assigned to every game.
    private string playerId = ""; // to be recieved from Yipli for each game
    private float points; // Game points / coins
    private string playerAge = ""; //Current age of the player
    private string playerHeight = ""; //Current height of the player
    private string playerWeight = ""; //Current height of the player
    private string matId = "";
    private DateTime startTime;
    private DateTime endTime;
    private float duration;
    private string intensityLevel = "low"; // to be decided by the game.
    private IDictionary<string, int> playerActionCounts; // to be updated by the player movements

    public class PlayerActions
    {
        public static string LEFTMOVE = "left-move";
        public static string RIGHTMOVE = "right-move";
        public static string JUMP = "jump";
        public static string STOP = "stop";
    }

    [JsonIgnore]
    public YipliConfig currentYipliConfig;

    [JsonIgnore]
    private bool bIsPaused; // to be set, when game is paused.

    [JsonIgnore]
    private static PlayerSession _instance;

    [JsonIgnore]
    public static PlayerSession Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        if (currentYipliConfig.playerInfo == null || currentYipliConfig.playerInfo.playerId.Equals(""))
        {
            // Call Yipli_GameLib_Scene
            Instance.currentYipliConfig.callbackLevel = SceneManager.GetActiveScene().name;
            Debug.Log("Updating the callBackLevel Value to :" + Instance.currentYipliConfig.callbackLevel);
            Debug.Log("Loading Yipli scene for player Selection...");
            SceneManager.LoadScene("yipli_lib_scene");
        }
        else
        {
            Debug.Log("Current player is null.");
        }
    }

    public string GetCurrentPlayer()
    {
        return currentYipliConfig.playerInfo.playerName;
    }

    public void ChangePlayer()
    {
        Instance.currentYipliConfig.callbackLevel = SceneManager.GetActiveScene().name;
        Debug.Log("Updating the callBackLevel Value to :" + Instance.currentYipliConfig.callbackLevel);
        Debug.Log("Loading Yipli scene for player Selection...");
        SceneManager.LoadScene("yipli_lib_scene");
    }

    public Dictionary<string, dynamic> GetJsonDic()
    {
        Dictionary<string, dynamic> x;
        x = new Dictionary<string, dynamic>();
        x.Add("game-id", gameId );
        x.Add("user-id", userId);
        x.Add("player-id", playerId);
        x.Add("age", playerAge);
        x.Add("points", points.ToString());
        x.Add("player-height", playerHeight);
        x.Add("start-time", startTime);
        x.Add("end-time", endTime);
        x.Add("duration", Convert.ToInt32(duration).ToString());
        x.Add("intensity-level", intensityLevel.ToString());
        x.Add("player-action-counts", playerActionCounts);
        x.Add("mat-id", matId);
        return x;
    }

    //Pass here name of the game
    public void SetYipliGameId(string strGameId)
    {
        if (strGameId.Equals("joyfuljumps"))
        {
            gameId = strGameId;
            SetGameClusterId(1);
            intensityLevel = "low";
        }
        else if (strGameId.Equals("eggcatcher"))
        {
            gameId = strGameId;
            SetGameClusterId(2);
            intensityLevel = "low";
        }
        else if (strGameId.Equals("yiplirunner"))
        {
            gameId = strGameId;
            SetGameClusterId(2);
            intensityLevel = "medium";
        }
        else
        {
            gameId = "";
            SetGameClusterId(0);
            intensityLevel = "";
        }
    }

    //First function to be called only once when the game starts()
    public void StartSPSession(string GameId)
    {
        Debug.Log("Starting current player session.");
        userId = currentYipliConfig.userId ?? "";
        playerId = currentYipliConfig.playerInfo.playerId;
        playerAge = currentYipliConfig.playerInfo.playerDOB ?? "";
        playerHeight = currentYipliConfig.playerInfo.playerHeight ?? "";
        playerWeight = currentYipliConfig.playerInfo.playerWeight ?? "";
        playerActionCounts = new Dictionary<string, int>();
        points = 0;
        duration = 0;
        bIsPaused = false;
        startTime = DateTime.Now;
        SetYipliGameId(GameId);
        matId = currentYipliConfig.matInfo.matId;
    }

    //To be used for error handling
    //Call in case of exception while playing game.
    public void CloseSPSession()
    {
        endTime = DateTime.Now;
        points = 0;
        duration = 0;
        Debug.Log("Aborting current player session.");
        //Destroy current player session object
    }

    public void StoreSPSession(float gamePoints)
    {
        Debug.Log("Storing current player session to backend database.");
        points = gamePoints;

        endTime = DateTime.Now;

        if(0 == ValidateSessionBeforePosting()) {
            //Store the session data to backend.
            FirebaseDBHandler.PostPlayerSession(Instance, () => { Debug.Log("Session stored in db"); });
            Debug.Log("Single player session stored successfully.");
        }
        else
        {
            Debug.Log("Session not posted : Validation failed for sessoin data.");
        }
        
    }


    //Function to validate all the session parameters before writing to DB
    private int ValidateSessionBeforePosting()
    {
        if (gameId == null || gameId == "")
        {
            Debug.Log("gameId is not set");
            return -1;
        }
        if (playerId == null || playerId == "")
        {
            Debug.Log("playerId is not set");
            return -1;
        }
        if (playerActionCounts.Count == 0)
        {
            Debug.Log("playerActionCounts is not set");
            return -1;
        }
        if (duration == 0)
        {
            Debug.Log("duration is 0");
            return -1;
        }
        if (intensityLevel == "")
        {
            Debug.Log("intensityLevel is not set");
            return -1;
        }
        return 0;
    }

    //To be called from GamePause function
    public void PauseSPSession()
    {
        Debug.Log("Pausing current player session.");
        bIsPaused = true; // only set the paused flat to true. Fixed update will take care of halting the time counter
    }

    //To be called from GameResume function
    public void ResumeSPSession()
    {
        Debug.Log("Resuming current player session.");
        bIsPaused = false;
    }

    //to be called from all the player movment actions handled script
    public void AddPlayerAction(string action)
    {
        Debug.Log("Adding action current player session.");
        if (playerActionCounts.ContainsKey(action))
            playerActionCounts[action] = playerActionCounts[action] + 1;
        else
            playerActionCounts.Add(action, 1);
    }

    //To be called from GameObject FixedUpdate
    public void UpdateDuration()
    {
        Debug.Log("Updating duration for current player session.");
        if (bIsPaused == false)
        {
            duration += Time.deltaTime;
        }
    }

    public void GoToYipli()
    {
        Debug.Log("Go to Yipli Called");
        string bundleId = "org.hightimeshq.yipli"; //to be changed later
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

        AndroidJavaObject launchIntent = null;
        try
        {
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
            ca.Call("startActivity", launchIntent);
        }
        catch (AndroidJavaException e)
        {
            //Todo: Redirect to playstore for Yipli App.
            Debug.Log(e);
        }
    }

    public void SetGameClusterId(int gameClusterId)
    {
        InitBLE.setGameClusterID(gameClusterId);
    }
}
