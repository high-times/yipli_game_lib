using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class YipliConfig : ScriptableObject
{
    [HideInInspector]
    public string callbackLevel;

    [HideInInspector]
    public YipliPlayerInfo playerInfo;

    [HideInInspector]
    public YipliMatInfo matInfo;

    [HideInInspector]
    public string userId;
    
    public string gameId;

    [HideInInspector]
    public bool bIsMatIntroDone = false;

    [HideInInspector]
    public MP_GameStateManager MP_GameStateManager;

    [HideInInspector]
    public List<YipliPlayerInfo> allPlayersInfo;

    [HideInInspector]
    public DataSnapshot gameDataForCurrentPlayer;

    [HideInInspector]
    public bool bIsChangePlayerCalled;

    public bool onlyMatPlayMode = true;

    [HideInInspector]
    public bool bIsInternetConnected;

    [HideInInspector]
    public YipliInventoryGameInfo gameInventoryInfo;

    [HideInInspector]
    public int oldFMResponseCount;
}
