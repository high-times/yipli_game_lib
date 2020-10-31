using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum QueryStatus
{
    NotStarted,
    InProgress,
    Completed
};

public class firebaseDBListenersAndHandlers : MonoBehaviour
{
    public YipliConfig currentYipliConfig;

    //Track if the query exection is completed or not
    private static QueryStatus getAllPlayersQureyStatus = global::QueryStatus.NotStarted;

    //Track if the query exection is completed or not
    private static QueryStatus getDefaultMatQueryStatus = global::QueryStatus.NotStarted;

    public static QueryStatus GetPlayersQueryStatus()
    {
        return getAllPlayersQureyStatus;
    }

    public static QueryStatus GetMatQueryStatus()
    {
        return getDefaultMatQueryStatus;
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        //Add listeners to the Firebase backend for all the DB Calls
        Debug.Log("Add listeners to the Firebase backend for all the DB Calls");
        PlayerSelection.NewUserFound += addGetPlayersListener;
        PlayerSelection.NewUserFound += addDefaultMatIdListener;

        PlayerSession.NewPlayerFound += addGameDataListener;
        PlayerSelection.DefaultPlayerChanged += addGameDataListener;

        PlayerSession.NewMatFound += addDefaultMatIdListener;

        StartCoroutine(TrackNetworkConnectivity());
    }

    private IEnumerator TrackNetworkConnectivity()
    {
        yield return anonAuthenticate();
        FirebaseDatabase.DefaultInstance.GetReference(".info/connected").ValueChanged += HandleConnectedChanged;
        //FirebaseDatabase.DefaultInstance.GetReference(".info/serverTimeOffset").ValueChanged += HandleServerTimeOffsetChanged;
    }

    private void HandleConnectedChanged(object sender, ValueChangedEventArgs e)
    {
        Debug.Log("Network : " + e.Snapshot.Value);
        currentYipliConfig.bIsInternetConnected = e.Snapshot.Value.Equals(true);
    }

    void OnDisable()
    {
        //Remove the events to avoid memory leaks
        PlayerSelection.NewUserFound -= addGetPlayersListener;
        PlayerSession.NewPlayerFound -= addGameDataListener;
        PlayerSelection.NewUserFound -= addDefaultMatIdListener;
        PlayerSelection.DefaultPlayerChanged -= addGameDataListener;
    }

    private async void addDefaultMatIdListener()
    {
        Debug.Log("addDefaultMatIdListener invoked");
        await anonAuthenticate();
        FirebaseDatabase.DefaultInstance
        .GetReference("profiles/users/" + currentYipliConfig.userId + "/current-mat-id")
        .ValueChanged += HandleCurrentMatIdValueChanged;
    }

    private async void addDefaultMatInfoListener(string matId)
    {
        Debug.Log("addDefaultMatInfoListener invoked");
        await anonAuthenticate();
        FirebaseDatabase.DefaultInstance
        .GetReference("profiles/users/" + currentYipliConfig.userId + "/mats/" + matId)
        .ValueChanged += HandleCurrentMatInfoValueChanged;
    }

    private void HandleCurrentMatInfoValueChanged(object sender, ValueChangedEventArgs args)
    {
        Debug.Log("HandleCurrentMatInfoValueChanged invoked");
        if(args.Snapshot.Value != null)
            currentYipliConfig.matInfo = new YipliMatInfo(args.Snapshot, args.Snapshot.Key);
        getDefaultMatQueryStatus = global::QueryStatus.Completed;
    }

    private void HandleCurrentMatIdValueChanged(object sender, ValueChangedEventArgs args)
    {
        Debug.Log("HandleCurrentMatIdValueChanged invoked");
        getDefaultMatQueryStatus = global::QueryStatus.InProgress;
        //args.Snapshot has mat-Id for default mat.
        string matId = args.Snapshot.ToString();
        addDefaultMatInfoListener(matId);
    }

    private async Task anonAuthenticate()
    {
        Debug.Log("Syncing data from the Firebase backend");
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.FirebaseUser newUser = await auth.SignInAnonymouslyAsync();
        Debug.LogFormat("User signed in successfully: {0} ({1})",
        newUser.DisplayName, newUser.UserId);

        Firebase.FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://yipli-project.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private async void addGetPlayersListener()
    {
        Debug.Log("addGetPlayersListener invoked");
        await anonAuthenticate();
        FirebaseDatabase.DefaultInstance
        .GetReference("profiles/users/" + currentYipliConfig.userId + "/players")
        .ValueChanged += HandleAllPlayersDataValueChanged;
    }

    void HandleAllPlayersDataValueChanged(object sender, ValueChangedEventArgs args)
    {
        getAllPlayersQureyStatus = global::QueryStatus.InProgress;
        Debug.Log("HandleAllPlayersDataValueChanged invoked");
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        if (currentYipliConfig.allPlayersInfo == null)
            currentYipliConfig.allPlayersInfo = new List<YipliPlayerInfo>();

        foreach (var childSnapshot in args.Snapshot.Children)
        {
            YipliPlayerInfo playerInstance = new YipliPlayerInfo(childSnapshot, childSnapshot.Key);
            if (playerInstance.playerId != null)
            {
                currentYipliConfig.allPlayersInfo.Add(playerInstance);
            }
            else
            {
                Debug.Log("Skipping this instance of player, backend seems corrupted.");
            }
        }
        Debug.Log("All players data got successfully.");
        getAllPlayersQureyStatus = global::QueryStatus.Completed;
    }

    private async void addGameDataListener()
    {
        Debug.Log("addGameDataListener invoked");
        await anonAuthenticate();

        if (!currentYipliConfig.gameId.Equals("default") || currentYipliConfig.gameId.Length > 1)
            FirebaseDatabase.DefaultInstance
            .GetReference("profiles/users/" + currentYipliConfig.userId + "/players/" + currentYipliConfig.playerInfo.playerId + "/activity-statistics/games-statistics/" + currentYipliConfig.gameId + "/game-data")
            .ValueChanged += HandleGameDataValueChanged;
    }

    void HandleGameDataValueChanged(object sender, ValueChangedEventArgs args)
    {
        Debug.Log("HandleGameDataValueChanged invoked");
        currentYipliConfig.gameDataForCurrentPlayer = args.Snapshot;
    }
}