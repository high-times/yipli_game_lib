using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_GameStateManager
{
    public MultiPlayerData playerData;

    public List<string> playerNames = new List<string>();
    public string playerOne, playerTwo;
    public bool isSinglePlayer;

    private YipliPlayerInfo tempPlayer;

    public PlayerDetails playerOneDetails = new PlayerDetails();
    public PlayerDetails playerTwoDetails = new PlayerDetails();

    [SerializeField] private Sprite computerSprite, playerSprite;

    private void Start()
    {
        // Force set screen resolution
        Screen.SetResolution(1280, 720, true);
    }


    public void SetPlayerData(MultiPlayerData multiPlayerData)
    {
        playerData=multiPlayerData;
    }



    #region PlayerImageRetrieval

    //public Sprite playerImage;

    //public Sprite GetCurrentPlayerImage()
    //{
    //    return playerImage;
    //}


    //public void SetCurrentPlayerImage(Sprite image)
    //{
    //    playerImage = image;
    //}

    #endregion

    private YipliPlayerInfo GetPlayerInfoFromPlayerName(string playerName)
    {
        if (MultiPlayerSelection.instance.players.Count > 0)
        {
            foreach (YipliPlayerInfo player in MultiPlayerSelection.instance.players)
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




    public void SetPlayerOne(string name)
    {
        playerOne = name;
        playerData.PlayerOneName = playerOne;
        playerData.PlayerOneImage = playerSprite;

        tempPlayer= GetPlayerInfoFromPlayerName(playerOne);

        playerOneDetails.userId = PlayerSession.Instance.currentYipliConfig.userId;
        playerOneDetails.matId = PlayerSession.Instance.currentYipliConfig.matInfo.matId;
        playerOneDetails.matMacAddress = PlayerSession.Instance.currentYipliConfig.matInfo.macAddress;
        playerOneDetails.playerId = tempPlayer.playerId;
        playerOneDetails.playerAge = tempPlayer.playerAge;
        playerOneDetails.playerHeight = tempPlayer.playerHeight;
        playerOneDetails.playerWeight = tempPlayer.playerWeight;
        playerOneDetails.playerActionCounts = new Dictionary<YipliUtils.PlayerActions, int>();
        playerOneDetails.playerGameData = new Dictionary<string, string>();

        playerData.PlayerOneDetails = playerOneDetails;

        //YipliPlayerInfo yipliPlayer = PlayerSession.Instance.
    }
    public void SetPlayerTwo(string name)
    {
        playerTwo = name;
        playerData.PlayerTwoName = playerTwo;
        playerData.PlayerTwoImage = playerSprite;
        playerData.IsSinglePlayer = false;

        tempPlayer = GetPlayerInfoFromPlayerName(playerTwo);

        playerTwoDetails.userId = PlayerSession.Instance.currentYipliConfig.userId;
        playerTwoDetails.matId = PlayerSession.Instance.currentYipliConfig.matInfo.matId;
        playerTwoDetails.matMacAddress = PlayerSession.Instance.currentYipliConfig.matInfo.macAddress;
        playerTwoDetails.playerId = tempPlayer.playerId;
        playerTwoDetails.playerAge = tempPlayer.playerAge;
        playerTwoDetails.playerHeight = tempPlayer.playerHeight;
        playerTwoDetails.playerWeight = tempPlayer.playerWeight;
        playerTwoDetails.playerActionCounts = new Dictionary<YipliUtils.PlayerActions, int>();
        playerTwoDetails.playerGameData = new Dictionary<string, string>();

        playerData.PlayerTwoDetails = playerTwoDetails;

        isSinglePlayer = false;
    }
    public void SetSinglePlayer()
    {
        playerTwo = "Yipli AI";
        playerData.PlayerTwoName = playerTwo;
        playerData.PlayerTwoImage = computerSprite;
        playerData.IsSinglePlayer = true;
        isSinglePlayer = true;
    }

}
