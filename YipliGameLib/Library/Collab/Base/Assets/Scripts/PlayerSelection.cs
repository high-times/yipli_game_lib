using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    public GameObject PlayerSelectionPanel;
    public TextMeshProUGUI continueOrSwitchPlayerText;
    public TextMeshProUGUI onlyOnePlayerText;
    public GameObject PlayersContainer;
    public GameObject switchPlayerPanel;
    public GameObject zeroPlayersPanel;
    public TextMeshProUGUI zeroPlayersText;
    public YipliConfig currentYipliConfig;
    public GameObject PlayerButtonPrefab;
    public GameObject OnlyOnePlayerPanel;
    public GameObject LoadingPanel;

    private List<YipliPLayerInfo> players = new List<YipliPLayerInfo>();
    private string PlayerName;
    private YipliPLayerInfo defaultPlayer;

    public MatSelection matSelectionScript;

    private List<GameObject> generatedObjects = new List<GameObject>();

    // When the game starts
    public async void Start()
    {
        try
        {
            Debug.Log("In player Selection Start()");
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            currentYipliConfig.userId = intent.Call<string>("getDataString");

            if (!currentYipliConfig.userId.Equals(""))
            {
                UserDataPersistence.SavePropertyValue("user-id", currentYipliConfig.userId);
            }
            else
            {
                currentYipliConfig.userId = UserDataPersistence.GetPropertyValue("user-id");
            }
        }
        catch (System.Exception exp)
        {
            Debug.Log("Exception occured in GetIntent!!!");
            Debug.Log(exp.ToString());
            //currentYipliConfig.userId = UserDataPersistence.GetPropertyValue("user-id"); // handling of game directing opening, without yipli app
            currentYipliConfig.userId = "F9zyHSRJUCb0Ctc15F9xkLFSH5f1";
        }

        if (currentYipliConfig.userId.Equals("") || currentYipliConfig.userId.Equals(null))
        {
            //Go to yipli Panel
            switchPlayerPanel.SetActive(false);
            PlayerSelectionPanel.SetActive(false);
            OnlyOnePlayerPanel.SetActive(false);
            zeroPlayersText.text = "User not found. PLease launch the game from Yipli app once.";
            zeroPlayersPanel.SetActive(true);
        }
        else
        {
            LoadingPanel.SetActive(true);
            //setBluetoothEnabled(); // Enable the bluetooth first
            string str = await matSelectionScript.validateAndConnectMat(currentYipliConfig.userId); // initiate the mat Connection in advance as it takes time to connect
            LoadingPanel.SetActive(false);

            CheckCurrentPlayer();
        }
    }

    public async void Retry()
    {
        switchPlayerPanel.SetActive(false);
        PlayerSelectionPanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);
        zeroPlayersPanel.SetActive(false);
        try
        {
            Debug.Log("In player Selection Start()");
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            currentYipliConfig.userId = intent.Call<string>("getDataString");

            if (!currentYipliConfig.userId.Equals(""))
                UserDataPersistence.SavePropertyValue("user-id", currentYipliConfig.userId);
            else
                currentYipliConfig.userId = UserDataPersistence.GetPropertyValue("user-id");
        }
        catch (System.Exception exp)
        {
            Debug.Log("Exception occured in GetIntent!!!");
            Debug.Log(exp.ToString());
            currentYipliConfig.userId = UserDataPersistence.GetPropertyValue("user-id"); // handling of game directing opening, without yipli app
            //userId = "4B5EfR3JjafcMPFT50YVKE78WF92";
            //userId = "HTdI5n3wJqgHUtszCKqNC70E2OF3";
        }

        if (currentYipliConfig.userId.Equals("") || currentYipliConfig.userId.Equals(null))
        {
            //Go to yipli Panel
            switchPlayerPanel.SetActive(false);
            PlayerSelectionPanel.SetActive(false);
            OnlyOnePlayerPanel.SetActive(false);
            zeroPlayersText.text = "User not found. PLease launch the game from Yipli app once.";
            zeroPlayersPanel.SetActive(true);
        }
        else
        {
            LoadingPanel.SetActive(true);
            //setBluetoothEnabled(); // Enable the bluetooth first
            string str = await matSelectionScript.validateAndConnectMat(currentYipliConfig.userId); // initiate the mat Connection in advance as it takes time to connect
            LoadingPanel.SetActive(false);
            CheckCurrentPlayer();
        }
    }

    public async void CheckCurrentPlayer()//Call this for every StartGame()/Game Session
    {
        Debug.Log("Checking current player.");

        PlayerSelectionPanel.SetActive(false);
        switchPlayerPanel.SetActive(false);
        switchPlayerPanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);

        LoadingPanel.SetActive(true);
        //Get Current player details from userId
        defaultPlayer = await FirebaseDBHandler.GetCurrentPlayerdetails(currentYipliConfig.userId, () => { Debug.Log("Got the current player details from db."); });

        LoadingPanel.SetActive(false);
        if (defaultPlayer != null)
        {
            Debug.Log("Found current player : " + defaultPlayer.playerName);
            //This means we have the default Player info from backend.
            //In this case we need to call the player change screen and not the player selection screen
            if (!defaultPlayer.playerName.Equals(""))
            {
                currentYipliConfig.playerInfo = defaultPlayer;
                PlayerChangeFlow();
            }
        }
        else //Current player not found in Db.
        {
            if (currentYipliConfig.playerInfo == null || currentYipliConfig.playerInfo.playerId.Equals("")) // If current Player isn't set in memory
            {
                Debug.Log("No player found in cache. Calling Player selection flow.");
                switchPlayerPanel.SetActive(false);
                PlayerSelectionPanel.SetActive(false);
                OnlyOnePlayerPanel.SetActive(false);
                zeroPlayersText.text = "No players found. Please create a new player from the YIPLI app to continue playing.";
                zeroPlayersPanel.SetActive(true);
            }
            else //Current player is set, call PlayerChangeFlow
            {
                //This means we already have the Current Player info.
                //In this case we need to call the player change screen and not the player selection screen
                PlayerChangeFlow();
            }
        }
    }

    public void PlayerSelectionFlow()
    {
        Debug.Log("In Player selection flow.");
        if (players.Count != 0) //Atleast 1 player found for the corresponding userId
        {
            Debug.Log("Player/s found from firebase : " + players.Count);

            try
            {
                Quaternion spawnrotation = Quaternion.identity;
                Vector3 NextPosition = PlayersContainer.transform.localPosition + new Vector3(0, 65, 0);

                for (int i = 0; i < players.Count; i++)
                {
                    GameObject PlayerButton = Instantiate(PlayerButtonPrefab, NextPosition, spawnrotation) as GameObject;
                    generatedObjects.Add(PlayerButton);
                    PlayerButton.name = players[i].playerName;
                    PlayerButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = players[i].playerName;
                    PlayerButton.transform.SetParent(PlayersContainer.transform, false);
                    PlayerButton.transform.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(SelectPlayer);
                    NextPosition = new Vector3(PlayerButton.transform.localPosition.x, PlayerButton.transform.localPosition.y - 40, PlayerButton.transform.localPosition.z);
                }
            }
            catch (Exception exp)
            {
                Debug.Log("Exception in Adding player : " + exp.Message);
                Application.Quit();
            }
            switchPlayerPanel.SetActive(false);
            OnlyOnePlayerPanel.SetActive(false);
            zeroPlayersPanel.SetActive(false);
            PlayerSelectionPanel.SetActive(true);
        }
        else
        {
            Debug.Log("No player found from firebase.");
            switchPlayerPanel.SetActive(false);
            PlayerSelectionPanel.SetActive(false);
            OnlyOnePlayerPanel.SetActive(false);
            zeroPlayersText.text = "No players found. Please create a new player from the YIPLI app to continue playing.";
            zeroPlayersPanel.SetActive(true);
        }
    }

    public void PlayerChangeFlow()
    {
        PlayerSelectionPanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);
        zeroPlayersPanel.SetActive(false);
        continueOrSwitchPlayerText.text = "Press continue to play as " + currentYipliConfig.playerInfo.playerName + "If not " + currentYipliConfig.playerInfo.playerName + ", you can switch player."; 
        switchPlayerPanel.SetActive(true);
    }

    public void SelectPlayer()
    {
        PlayerName = EventSystem.current.currentSelectedGameObject.name;

        //// first of all destroy all PlayerButton prefabs
        foreach(var obj1 in generatedObjects)
        {
            Destroy(obj1);
        }
        Debug.Log("Player Selected :  " + PlayerName);

        continueOrSwitchPlayerText.text = "Press continue to play as " + PlayerName + ". If not " + PlayerName + ", you can switch player.";

        //Changing the currentSelected player in the Scriptable object
        currentYipliConfig.playerInfo = GetPlayerInfoFromPlayerName(PlayerName);

        switchPlayerPanel.SetActive(true);
        PlayerSelectionPanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);
        zeroPlayersPanel.SetActive(false);
    }

    //public void OnOkPress()
    //{
    //    Debug.Log("Ok Button Pressed.");
    //    Debug.Log(PlayerName + " selected");

    //    switchPlayerPanel.SetActive(false);
    //    PlayerSelectionPanel.SetActive(false);
    //    OnlyOnePlayerPanel.SetActive(false);
    //    zeroPlayersPanel.SetActive(false);

    //    currentYipliConfig.playerInfo = GetPlayerInfoFromPlayerName(PlayerName);

    //    //Make new default player persist to the backend as well, so that it gets reflected in the Yipli App as well.
    //    FirebaseDBHandler.ChangeCurrentPlayer(currentYipliConfig.userId, currentYipliConfig.playerInfo.playerId, () => { Debug.Log("Change the default player in the backend"); });

    //    matSelectionScript.CheckMatConnectionStatus(currentYipliConfig.userId);
    //}

    private YipliPLayerInfo GetPlayerInfoFromPlayerName(string playerName)
    {
        if (players.Count > 0)
        {
            foreach (YipliPLayerInfo player in players)
            {
                Debug.Log("Found player : " + player.playerName);
                if (player.playerName == playerName)
                {
                    Debug.Log("Found player : " + player.playerName);
                    return player;
                }
            }
        }
        else
        {
            Debug.Log("No Players found.");
        }
        return null;
    }

    //public void OnCancelPress()
    //{
    //    Debug.Log("Ok Cancel Pressed.");
    //    Debug.Log(PlayerName + " selected");
    //    switchPlayerPanel.SetActive(false);
    //    zeroPlayersPanel.SetActive(false);
    //    OnlyOnePlayerPanel.SetActive(false);
    //    PlayerSelectionPanel.SetActive(true);
    //}

    //public void OnCancelSelectionPress()
    //{
    //    Debug.Log("Cancel selection Pressed.");
    //    PlayerSelectionPanel.SetActive(false);
    //    zeroPlayersPanel.SetActive(false);
    //    OnlyOnePlayerPanel.SetActive(false);
    //    switchPlayerPanel.SetActive(true);
    //}

    public void OnContinuePress()
    {
        Debug.Log("Continue Pressed.");
        PlayerSelectionPanel.SetActive(false);
        switchPlayerPanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);
        switchPlayerPanel.SetActive(false);

        if (defaultPlayer.playerId.Equals(currentYipliConfig.playerInfo.playerId))
        {
            //No need to change player in the backend, as its already the default player there.
            Debug.Log("Continuing as "+ currentYipliConfig.playerInfo.playerName);
            Debug.Log("Not changing the default player in the backend, as it is the same");
        }
        else
        {
            Debug.Log("Chaning the defualt player in the backend to " + currentYipliConfig.playerInfo.playerName);
            //Make new default player persist to the backend as well, so that it gets reflected in the Yipli App as well.
            FirebaseDBHandler.ChangeCurrentPlayer(currentYipliConfig.userId, currentYipliConfig.playerInfo.playerId, () => { Debug.Log("Changed the default player in the backend"); });
        }
        matSelectionScript.CheckMatConnectionStatus(currentYipliConfig.userId);
    }

    public async void OnChangePlayerPress()
    {
        PlayerSelectionPanel.SetActive(false);
        switchPlayerPanel.SetActive(false);
        switchPlayerPanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);

        LoadingPanel.SetActive(true);
        players = await FirebaseDBHandler.GetAllPlayerdetails(currentYipliConfig.userId, currentYipliConfig.playerInfo.playerId, () => { Debug.Log("Got the player details from db"); });

        LoadingPanel.SetActive(false);
        //First check if the players count under userId is more than 1 ?
        if (players.Count > 1)
        {
            PlayerSelectionFlow();
        }
        else // If No then throw a new panel to tell the Gamer that there is only 1 player currently
        {
            PlayerSelectionPanel.SetActive(false);
            switchPlayerPanel.SetActive(false);
            switchPlayerPanel.SetActive(false);
            onlyOnePlayerText.text = currentYipliConfig.playerInfo.playerName + "  is the only player found.\nAdd more players from the YIPLI app";

            OnlyOnePlayerPanel.SetActive(true);
        }
    }

    public void OnGoToYipliPress()
    {
        string bundleId = "org.hightimeshq.yipli"; //todo: Change this later
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
            Debug.Log(e);
            zeroPlayersText.text = "Yipli App is not installed. Please install Yipli from market place to continue playing.";
        }
    }

    public void OnBackButtonPress()
    {
        Debug.Log("In backbutton press");
        OnlyOnePlayerPanel.SetActive(false);
        PlayerSelectionPanel.SetActive(false);
        switchPlayerPanel.SetActive(true);
        Debug.Log("Back Button Success");
    }

    public void setBluetoothEnabled()
    {
        using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
        {
            try
            {
                using (var BluetoothManager = activity.Call<AndroidJavaObject>("getSystemService", "bluetooth"))
                {
                    using (var BluetoothAdapter = BluetoothManager.Call<AndroidJavaObject>("getAdapter"))
                    {
                        BluetoothAdapter.Call("enable");
                    }

                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("could not enable the bluetooth automatically");
            }
        }
    }
}
