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
using UnityEngine.Audio;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Threading;
using Firebase.Storage;

public class PlayerSelection : MonoBehaviour
{
    public GameObject phoneHolderInfo;
    public GameObject LaunchFromYipliAppPanel;
    public TextMeshProUGUI[] TextsToBeChanged;
    public GameObject playerSelectionPanel;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI onlyOnePlayerText;
    public GameObject PlayersContainer;
    public GameObject switchPlayerPanel;
    public GameObject zeroPlayersPanel;
    public GameObject GuestUserPanel;
    public TextMeshProUGUI zeroPlayersText;
    public YipliConfig currentYipliConfig;
    public GameObject PlayerButtonPrefab;
    public GameObject onlyOnePlayerPanel;
    public GameObject LoadingPanel;
    public GameObject noNetworkPanel;
    public TextMeshProUGUI noNetworkPanelText;
    public MatSelection matSelectionScript;
    public Image profilePicImage;
                

    private List<YipliPlayerInfo> players;
    private string PlayerName;
    private YipliPlayerInfo defaultPlayer;
    private List<GameObject> generatedObjects = new List<GameObject>();
    private bool bIsCheckingForIntents;
    private bool bIsProfilePicLoaded = false;
    private Sprite defaultProfilePicSprite;

    public void OnEnable()
    {
        defaultProfilePicSprite = profilePicImage.sprite;
    }
    // When the game starts
    public void Start()
    {
        //Read intents and Initialize defaults
        CheckIntentsAndInitializePlayerEnvironment();
        /*
         Flag to support mobile gamePlay. Initialize to MatMode by default.
         If the mat is skipped later on with the 'Skip' button, 
         the flag will be set to false, and game could be played with mobile touch.
         */
        currentYipliConfig.matPlayMode = true;

        // Disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    //Whenever the Yipli App launches the game, the user will be found and next flow will be called automatically.
    //No need to keep retrying.
    //Start this coroutine to check for intents till a valid user is not found.
    IEnumerator KeepCheckingForIntents()
    {

        Debug.Log("Started Coroutine : KeepCheckingForIntents");
        while (true)
        {
            bIsCheckingForIntents = true;

            //If the given panel is active, means the game is not launched from Yipli App.
            //Keep checking after every .5f Seconds if the app is launched from Yipli App.
            if (LaunchFromYipliAppPanel.activeSelf)
            {
                CheckIntentsAndInitializePlayerEnvironment();
            }
            yield return new WaitForSeconds(0.5f);

            bIsCheckingForIntents = false;
        }
    }

    async public void playPhoneHolderTutorial()
    {
        TurnOffAllPanels();
        Debug.Log("Starting PhoneHolder Tutorial for " + defaultPlayer.playerName);
        Debug.Log("Is profilePicLoaded = " + bIsProfilePicLoaded);
        if (!bIsProfilePicLoaded)
        {
            LoadingPanel.SetActive(true);
            bIsProfilePicLoaded = await loadProfilePicAsync(profilePicImage, defaultPlayer.profilePicUrl);
            LoadingPanel.SetActive(true);
        }
        playerNameText.text = "Hi, " + defaultPlayer.playerName;
        playerNameText.gameObject.SetActive(true);
        phoneHolderInfo.SetActive(true);
        StartCoroutine(ChangeTextMessage());
    }

    IEnumerator ChangeTextMessage()
    {
        while (true)//Infinite loop
        {
            foreach (var str in TextsToBeChanged)
            {
                str.gameObject.SetActive(true);
                yield return new WaitForSecondsRealtime(5f);
                str.gameObject.SetActive(false);
            }
        }
    }

    private void NoUserFoundInGameFlow()
    {
        //Go to Yipli Panel
        TurnOffAllPanels();

        playerNameText.gameObject.SetActive(false);

        //Depending upon if Main Yipli App is installed or not, take a decision to show No user panel / or Guest User Panel
        if (false == YipliHelper.IsYipliAppInstalled())
            GuestUserPanel.SetActive(true);
        else
        {
            //Currently User isnt found. Start a coroutine which keeps on checking for the user details.
            if (false == bIsCheckingForIntents)
                StartCoroutine(KeepCheckingForIntents());

            LaunchFromYipliAppPanel.SetActive(true);
        }
    }

    //Here default player object is filled with the intents passed.
    //If no intents found, it is filled with the device persisted default player object.
    private void InitializeDefaultPlayer()
    {
        //Not storing Default player in scriptable object as it would be done later.

        if (defaultPlayer != null)
        {
            //If PlayerInfo is found in the Intents as an argument.
            //This code block will be called when the game App is launched from the Yipli app.
            UserDataPersistence.SavePlayerToDevice(defaultPlayer);
        }
        else
        {
            //If there is no PlayerInfo found in the Intents as an argument.
            //This code block will be called when the game App is not launched from the Yipli app.
            defaultPlayer = UserDataPersistence.GetSavedPlayer();
        }
    }

    //Here default player object is filled with the intents passed.
    //If no intents found, it is filled with the device persisted default player object.
    private void InitializeDefaultMat()
    {
        //Not storing Default player in scriptable object as it would be done later.

        if (currentYipliConfig.matInfo != null)
        {
            //If PlayerInfo is found in the Intents as an argument.
            //This code block will be called when the game App is launched from the Yipli app.
            UserDataPersistence.SaveMatToDevice(currentYipliConfig.matInfo);
        }
        else
        {
            //If there is no PlayerInfo found in the Intents as an argument.
            //This code block will be called when the game App is not launched from the Yipli app.
            currentYipliConfig.matInfo = UserDataPersistence.GetSavedMat();
        }
    }

    private void InitializeUserId()
    {
        try
        {

            if (currentYipliConfig.userId != "")
            {
                //If UserId is found in the Intents as an argument.
                //This code block will be called when the game App is launched from the Yipli app.
                UserDataPersistence.SavePropertyValue("user-id", currentYipliConfig.userId);
                //PlayerPrefs.Save();
            }
            else
            {
                //If there is no UserId found in the Intents as an argument.
                //This code block will be called when the game App is not launched from the Yipli app.
                currentYipliConfig.userId = UserDataPersistence.GetPropertyValue("user-id");
            }
        }
        catch (Exception exp)
        {
            currentYipliConfig.userId = null;
            Debug.Log("Exception in InitializeUserId() : " + exp.Message);
            Debug.Log("Setting User ID to null.");
        }
    }

    private void ReadIntents()
    {
        Debug.Log("Reading intents.");
        AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
        AndroidJavaObject extras = intent.Call<AndroidJavaObject>("getExtras");

        currentYipliConfig.userId = extras.Call<string>("getString", "uId");
        if (currentYipliConfig.userId == null)
        {
            Debug.Log("Returning from readIntents as no userId found.");
            return;
        }

        string pId = extras.Call<string>("getString", "pId");
        string pName = extras.Call<string>("getString", "pName");
        string pDOB = extras.Call<string>("getString", "pDOB");
        string pHt = extras.Call<string>("getString", "pHt");
        string pWt = extras.Call<string>("getString", "pWt");
        string mId = extras.Call<string>("getString", "mId");
        string mMac = extras.Call<string>("getString", "mMac");

        Debug.Log("Found intents : " + currentYipliConfig.userId + ", " + pId + ", " + pDOB + ", " + pHt + ", " + pWt + ", " + pName + ", " + mId + ", " + mMac);

        if (pId != null && pName != null)
        {
            defaultPlayer = new YipliPlayerInfo(pId, pName, pDOB, pHt, pWt);
        }

        if (mId != null && mMac != null)
        {
            currentYipliConfig.matInfo = new YipliMatInfo(mId, mMac);
        }
    }

    public void Retry()
    {
        TurnOffAllPanels();
        CheckIntentsAndInitializePlayerEnvironment();
    }

    private void TurnOffAllPanels()
    {
        phoneHolderInfo.SetActive(false);
        switchPlayerPanel.SetActive(false);
        playerSelectionPanel.SetActive(false);
        onlyOnePlayerPanel.SetActive(false);
        zeroPlayersPanel.SetActive(false);
        noNetworkPanel.SetActive(false);
        GuestUserPanel.SetActive(false);
        LaunchFromYipliAppPanel.SetActive(false);
        LoadingPanel.SetActive(false);
    }

    private void CheckIntentsAndInitializePlayerEnvironment()
    {
        try
        {
            Debug.Log("In player Selection Start()");
            ReadIntents();
        }
        catch (System.Exception exp)// handling of game directing opening, without yipli app
        {
            Debug.Log("Exception occured in GetIntent!!!");
            Debug.Log(exp.Message);
            //defaultPlayer = null;

            //currentYipliConfig.userId = null;
            //defaultPlayer = null;

            /*currentYipliConfig.userId = "F9zyHSRJUCb0Ctc15F9xkLFSH5f1";
            defaultPlayer = new YipliPlayerInfo("-M2iG0P2_UNsE2VRcU5P", "rooo", "03-01-1999", "120", "49");
            currentYipliConfig.matInfo = new YipliMatInfo("-M3HgyBMOl9OssN8T6sq", "54:6C:0E:20:A0:3B");*/
        }

        //Setting User Id in the scriptable Object
        InitializeUserId();

        //Setting default Player in the scriptable Object
        InitializeDefaultPlayer();

        //Setting Deafult mat
        InitializeDefaultMat();

        if (currentYipliConfig.userId == null || currentYipliConfig.userId == "")
        {
            //This code block will be called when the game App is not launched from the Yipli app even once.
            NoUserFoundInGameFlow();
        }
        else
        {
            //setBluetoothEnabled(); // Enable the bluetooth first
            // Initiate the mat Connection in advance as it takes time to connect.
            matSelectionScript.ValidateAndConnectMat(); //This async call is not awaited, hence mat connection runs in the background.

            //Stop the Coroutine which keep schecking for the intents.
            StopCoroutine(KeepCheckingForIntents());

            //Uncomment following line to always start the flow from phoneHolder panel
            //currentYipliConfig.bIsMatIntroDone = false;
            if (!currentYipliConfig.bIsMatIntroDone && defaultPlayer != null)
                playPhoneHolderTutorial();
            else
                SwitchPlayerFlow();
        }
    }

    public async void SwitchPlayerFlow()//Call this for every StartGame()/Game Session
    {
        Debug.Log("Checking current player.");

        TurnOffAllPanels();

        if (YipliHelper.checkInternetConnection())
        {
            LoadingPanel.SetActive(true);
            //Get Current player details from userId
            defaultPlayer = await FirebaseDBHandler.GetCurrentPlayerdetails(currentYipliConfig.userId, () => { Debug.Log("Got the current player details from db."); });
            LoadingPanel.SetActive(false);
        }

        if (defaultPlayer != null)
        {
            //This means we have the default Player info from backend.
            //In this case we need to call the player change screen and not the player selection screen
            Debug.Log("Found current player : " + defaultPlayer.playerName);
            currentYipliConfig.playerInfo = defaultPlayer;
            UserDataPersistence.SavePlayerToDevice(currentYipliConfig.playerInfo);

            //Activate the PlayerName and Image display object
            LoadingPanel.SetActive(true);
            if (!bIsProfilePicLoaded)
                bIsProfilePicLoaded = await loadProfilePicAsync(profilePicImage, defaultPlayer.profilePicUrl);
            LoadingPanel.SetActive(false);
            playerNameText.text = "Hi, " + defaultPlayer.playerName;
            playerNameText.gameObject.SetActive(true);

            //Initiate the player change flow
            switchPlayerPanel.SetActive(true);
        }
        else //Current player not found in Db.
        {
            if (currentYipliConfig.playerInfo == null) // If current Player isn't set in memory
            {
                //Force to switch player as no default player found.
                OnSwitchPlayerPress(true);
            }
            else //Current player is set, call PlayerChangeFlow
            {
                //This means we already have the Current Player info.
                //In this case we need to call the player change screen and not the player selection screen
                //continueOrSwitchPlayerText.text = "Press continue to play as " + currentYipliConfig.playerInfo.playerName + ".\nIf not " + currentYipliConfig.playerInfo.playerName + ", you can switch player.";
                LoadingPanel.SetActive(true);
                if (!bIsProfilePicLoaded)
                    bIsProfilePicLoaded = await loadProfilePicAsync(profilePicImage, defaultPlayer.profilePicUrl);
                LoadingPanel.SetActive(false);
                playerNameText.text = "Hi, " + currentYipliConfig.playerInfo.playerName;
                playerNameText.gameObject.SetActive(true);
                switchPlayerPanel.SetActive(true);
            }
        }
    }

    public void PlayerSelectionFlow()
    {
        Debug.Log("In Player selection flow.");

        // first of all destroy all PlayerButton prefabs. This is required to remove stale prefabs.
        if (generatedObjects != null)
        {
            Debug.Log("Destroying all the stale --PlayerName-- prefabs, before spawning new ones.");
            foreach (var obj1 in generatedObjects)
            {
                Destroy(obj1);
            }
        }

        if (players.Count != 0) //Atleast 1 player found for the corresponding userId
        {
            Debug.Log("Player/s found from firebase : " + players.Count);

            try
            {
                Quaternion spawnrotation = Quaternion.identity;
                Vector3 playerTilePosition = PlayersContainer.transform.localPosition;

                for (int i = 0; i < players.Count; i++)
                {
                    GameObject PlayerButton = Instantiate(PlayerButtonPrefab, playerTilePosition, spawnrotation) as GameObject;
                    generatedObjects.Add(PlayerButton);
                    PlayerButton.name = players[i].playerName;
                    PlayerButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = players[i].playerName;
                    PlayerButton.transform.SetParent(PlayersContainer.transform, false);
                    PlayerButton.transform.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(SelectPlayer);
                }
            }
            catch (Exception exp)
            {
                Debug.Log("Exception in Adding player : " + exp.Message);
                Application.Quit();
            }
            TurnOffAllPanels();
            playerSelectionPanel.SetActive(true);
        }
        else
        {
            Debug.Log("No player found from firebase.");
            TurnOffAllPanels();
            //zeroPlayersText.text = "No players found. Create a player from the YIPLI app to continue playing.";
            zeroPlayersPanel.SetActive(true);
        }
    }

    async public void SelectPlayer()
    {
        playerSelectionPanel.SetActive(false);
        FindObjectOfType<YipliAudioManager>().Play("ButtonClick");
        PlayerName = EventSystem.current.currentSelectedGameObject.name;

        // first of all destroy all PlayerButton prefabs. This is required to remove stale prefabs.
        foreach (var obj1 in generatedObjects)
        {
            Destroy(obj1);
        }
        Debug.Log("Player Selected :  " + PlayerName);


        //Changing the currentSelected player in the Scriptable object
        //No Making this player persist in the device. This will be done on continue press.
        defaultPlayer = GetPlayerInfoFromPlayerName(PlayerName);
        LoadingPanel.SetActive(true);
        bIsProfilePicLoaded = await loadProfilePicAsync(profilePicImage, defaultPlayer.profilePicUrl);
        LoadingPanel.SetActive(false);
        playerNameText.text = "Hi, " + PlayerName;
        playerNameText.gameObject.SetActive(true);

        TurnOffAllPanels();
        switchPlayerPanel.SetActive(true);
    }
    private YipliPlayerInfo GetPlayerInfoFromPlayerName(string playerName)
    {
        if (players.Count > 0)
        {
            foreach (YipliPlayerInfo player in players)
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

    public void OnContinuePress()
    {
        Debug.Log("Continue Pressed.");
        TurnOffAllPanels();

        if (defaultPlayer != null)
        {
            if (defaultPlayer.playerId.Equals(currentYipliConfig.playerInfo.playerId))
            {
                //No need to change player in the backend, as its already the default player there.
                Debug.Log("Continuing as " + currentYipliConfig.playerInfo.playerName);
                Debug.Log("Not changing the default player in the backend, as it is the same");
            }
            else
            {
                currentYipliConfig.playerInfo = defaultPlayer;
                UserDataPersistence.SavePlayerToDevice(currentYipliConfig.playerInfo);

                //Make new default player persist to the backend as well, so that it gets reflected in the Yipli App as well.
                if (YipliHelper.checkInternetConnection())
                {
                    Debug.Log("Chaning the defualt player in the backend to " + currentYipliConfig.playerInfo.playerName);
                    FirebaseDBHandler.ChangeCurrentPlayerInBackend(currentYipliConfig.userId, currentYipliConfig.playerInfo.playerId, () => { Debug.Log("Changed the default player in the backend"); });
                }
            }
        }
        else
        {
            Debug.Log("Chaning the defualt player in the backend to " + currentYipliConfig.playerInfo.playerName);
            //Make new default player persist to the backend as well, so that it gets reflected in the Yipli App as well.
            if (YipliHelper.checkInternetConnection())
            {
                FirebaseDBHandler.ChangeCurrentPlayerInBackend(currentYipliConfig.userId, currentYipliConfig.playerInfo.playerId, () => { Debug.Log("Changed the default player in the backend"); });
            }
        }
        matSelectionScript.MatConnectionFlow();
    }

    public void OnJumpOnMat()
    {
        LoadingPanel.SetActive(true);
        currentYipliConfig.bIsMatIntroDone = true;
        phoneHolderInfo.SetActive(false);
        StopCoroutine(ChangeTextMessage());
        SwitchPlayerFlow();
    }

    public async void OnSwitchPlayerPress(bool isInternalCall = false /* If called internally that means no default player found.*/)
    {
        TurnOffAllPanels();

        if (!YipliHelper.checkInternetConnection())
        {
            // Network not available. Only default player allowed to play.
            Debug.Log("Network not available. Only default player allowed to play.");
            players = null;

            //If default player also not available, then show no players panel.
            if (defaultPlayer != null)
            {
                //noNetworkPanelText.text = "No active network connection. Cannot switch player. Press continue to play as " + defaultPlayer.playerName;
                noNetworkPanel.SetActive(true);
            }
            else
            {
                //Default player is not there.
                zeroPlayersPanel.SetActive(true);
            }

        }
        else//Active Network connection is available
        {
            LoadingPanel.SetActive(true);
            players = await FirebaseDBHandler.GetAllPlayerdetails(currentYipliConfig.userId, () => { Debug.Log("Got the player details from db"); });
            LoadingPanel.SetActive(false);

            //In case of internall call
            //This is to handle if the current-player-id is currupted in the backend
            if (isInternalCall)
            {
                //This means no default player is present.
                //Show all the players for selection.
                //Whichever gets selected, will become the default player later.
                //First check if the players count under userId is more than 0?
                if (players != null && players.Count > 0)
                {
                    Debug.Log("Calling the PlayerSelectionFlow");
                    PlayerSelectionFlow();
                }
                else // If No then throw a new panel to tell the Gamer that there is no player found
                {
                    TurnOffAllPanels();
                    zeroPlayersPanel.SetActive(true);
                }
            }
            else
            {
                //First check if the players count under userId is more than 1 ?
                if (players != null && players.Count > 1)
                {
                    PlayerSelectionFlow();
                }
                else // If No then throw a new panel to tell the Gamer that there is only 1 player currently
                {
                    TurnOffAllPanels();
                    onlyOnePlayerPanel.SetActive(true);
                }
            }
        }
    }

    public void OnGoToYipliPress()
    {
        YipliHelper.GoToYipli();
    }

    public void OnBackButtonPress()
    {
        TurnOffAllPanels();
        switchPlayerPanel.SetActive(true);
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

    async private Task<bool> loadProfilePicAsync(Image gameObj, string profilePicUrl)
    {
        if (profilePicUrl == null || profilePicUrl == "" || gameObj == null)
        {
            Debug.Log("Something went wrong. Returning.");
            //Set the profile pic to a default one.
            gameObj.sprite = defaultProfilePicSprite;
        }
        else
        {   
            // Create local filesystem URL
            string onDeviceProfilePicPath = Application.persistentDataPath + "/" + profilePicUrl;
            Sprite downloadedSprite = await FirebaseDBHandler.GetImageAsync(profilePicUrl, onDeviceProfilePicPath);
            if (downloadedSprite != null)
            {
                gameObj.sprite = downloadedSprite;
                return true;
            }
            else
            {
                //Actual profile pic in the backend
                gameObj.sprite = defaultProfilePicSprite;
            }
        }
        return false;
    }
}
