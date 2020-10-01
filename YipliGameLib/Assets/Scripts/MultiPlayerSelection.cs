using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiPlayerSelection : MonoBehaviour
{

    public static MultiPlayerSelection instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    [SerializeField] private GameObject playersPanel, switchPlayerPanel;


    public YipliConfig currentYipliConfig;
    private GameObject playerButton;

    public event Action playerSelectedEvent;
    public event Action openPlayerSelectEvent;


    public GameObject PlayersContainer;
    public GameObject PlayerButtonPrefab;
    private List<GameObject> generatedObjects = new List<GameObject>();
    private List<Button> playerButtons = new List<Button>();

    public List<YipliPlayerInfo> players;

    private GameObject playerOneButton, playerTwoButton, computerPlayerButton;
    private int playerOneIndex, playerTwoIndex;

    public bool isSwitchingPlayerOne, isSwitchingPlayerTwo;

    private async void Start()
    {
        await GetPlayersListAsync();
        isSwitchingPlayerOne = false;
        isSwitchingPlayerTwo = false;
    }



    public async System.Threading.Tasks.Task GetPlayersListAsync()
    {
        players = await FirebaseDBHandler.GetAllPlayerdetails(currentYipliConfig.userId, () => { Debug.Log("Got the player details from db"); });
    }

    public void CreatePlayersList(int switchingPlayer)
    {
        for(int i = 0; i < PlayersContainer.transform.childCount; i++)
        {
            Destroy(PlayersContainer.transform.GetChild(i).gameObject);
        }
        generatedObjects.Clear();
        playerButtons.Clear();
        PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerNames.Clear();

        Quaternion spawnrotation = Quaternion.identity;
        Vector3 playerTilePosition = PlayersContainer.transform.localPosition;
        for (int i = 0; i < players.Count; i++)
        {
            PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerNames.Add(players[i].playerName);
            playerButton = Instantiate(PlayerButtonPrefab, playerTilePosition, spawnrotation) as GameObject;
            playerButton.name = players[i].playerName;
            playerButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = players[i].playerName;
            playerButton.transform.SetParent(PlayersContainer.transform, false);

            if (players[i].playerName == PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerOne)
            {
                playerOneIndex = i;
                playerOneButton = playerButton;
                if (switchingPlayer == 2)
                {
                    Destroy(playerButton);
                    continue;
                }
            }
            else if (players[i].playerName == PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerTwo)
            {
                playerTwoIndex = i;
                playerTwoButton = playerButton;
                if (switchingPlayer == 1)
                {
                    Destroy(playerButton);
                    continue;
                }
            }
                generatedObjects.Add(playerButton);
                playerButtons.Add(playerButton.GetComponent<Button>());
        }

        if (switchingPlayer == 1)
        {
            for (int i = 0; i < playerButtons.Count; i++)
            {
                playerButtons[i].onClick.RemoveAllListeners();
                playerButtons[i].onClick.AddListener(SelectFirstPlayer);
            }
        }
        else if (switchingPlayer == 2)
        {

            for (int i = 0; i < playerButtons.Count; i++)
            {
                playerButtons[i].onClick.RemoveAllListeners();
                playerButtons[i].onClick.AddListener(SelectSecondPlayer);
            }

            playerButton = Instantiate(PlayerButtonPrefab, playerTilePosition, spawnrotation) as GameObject;
            playerButton.name = "Yipli AI";
            playerButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Yipli AI";
            playerButton.transform.SetParent(PlayersContainer.transform, false);
            playerButton.GetComponent<Button>().onClick.AddListener(SelectComputerPlayer);
            computerPlayerButton = playerButton;
            generatedObjects.Add(playerButton);
            playerButtons.Add(playerButton.GetComponent<Button>());
        }
    }

    public void OpenSelectionForPlayerOne()
    {
        CreatePlayersList(1);
        openPlayerSelectEvent?.Invoke();
    }

    public void OpenSelectionForPlayerTwo()
    {
        CreatePlayersList(2);
        openPlayerSelectEvent?.Invoke();
    }

    public void SelectFirstPlayer()
    {
        playerOneButton = EventSystem.current.currentSelectedGameObject;
        PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.SetPlayerOne(EventSystem.current.currentSelectedGameObject.name);
        playerSelectedEvent?.Invoke();
    }

    public void SelectSecondPlayer()
    {
        playerTwoButton = EventSystem.current.currentSelectedGameObject;
        PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.SetPlayerTwo(EventSystem.current.currentSelectedGameObject.name);
        playerSelectedEvent?.Invoke();
    }

    public void SelectComputerPlayer()
    {
        PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.SetSinglePlayer();
        playerSelectedEvent?.Invoke();

        if (playerTwoButton != null)
        {
            generatedObjects.Remove(playerTwoButton);
            playerButtons.Remove(playerTwoButton.GetComponent<Button>());
        }
        generatedObjects.Remove(computerPlayerButton);
        playerButtons.Remove(computerPlayerButton.GetComponent<Button>());
    }

}
