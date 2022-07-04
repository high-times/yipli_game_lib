using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yipli.HttpMpdule
{
    public class HTTPPlayerSession : MonoBehaviour
    {
        [Header("Required Scriptable Objects")]
        [SerializeField] private HTTPYipliConfig currentYipliConfig = null;

        // static variables
        private static HTTPPlayerSession _instance;
        public static HTTPPlayerSession Instance { get { return _instance; } }

        // Getters and Setters
        public HTTPYipliConfig CurrentYipliConfig { get => currentYipliConfig; set => currentYipliConfig = value; }

        public void AddMultiPlayerAction(YipliUtils.PlayerActions action, PlayerDetails playerDetails, int count)
        {
            throw new NotImplementedException();
        }

        public void AddPlayerAction(YipliUtils.PlayerActions action, int count)
        {
            throw new NotImplementedException();
        }

        public void ChangePlayer()
        {
            throw new NotImplementedException();
        }

        public void CloseSPSession()
        {
            throw new NotImplementedException();
        }

        public float GetCaloriesBurned()
        {
            throw new NotImplementedException();
        }

        public string GetCurrentPlayer()
        {
            throw new NotImplementedException();
        }

        public string GetCurrentPlayerId()
        {
            throw new NotImplementedException();
        }

        public string GetDriverAndGameVersion()
        {
            throw new NotImplementedException();
        }

        public float GetFitnessPoints()
        {
            throw new NotImplementedException();
        }

        public IDictionary<YipliUtils.PlayerActions, int> getMultiPlayerActionCounts(PlayerDetails playerDetails)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, dynamic> GetMultiPlayerSessionDataJsonDic(PlayerDetails playerDetails, string mpSessionUUID)
        {
            throw new NotImplementedException();
        }

        public int GetOldFMResponseCount()
        {
            throw new NotImplementedException();
        }

        public IDictionary<YipliUtils.PlayerActions, int> getPlayerActionCounts()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, dynamic> GetPlayerSessionDataJsonDic()
        {
            throw new NotImplementedException();
        }

        public void GotoYipli()
        {
            throw new NotImplementedException();
        }

        public void HTTPAwakeOperations()
        {
            throw new NotImplementedException();
        }

        public void LoadingScreenSetActive(bool bOn)
        {
            throw new NotImplementedException();
        }

        public void PauseMPSession()
        {
            throw new NotImplementedException();
        }

        public void PauseSPSession()
        {
            throw new NotImplementedException();
        }

        public void ReInitializeSPSession()
        {
            throw new NotImplementedException();
        }

        public void ResumeMPSession()
        {
            throw new NotImplementedException();
        }

        public void ResumeSPSession()
        {
            throw new NotImplementedException();
        }

        public void RetakeMatControlsTutorial()
        {
            throw new NotImplementedException();
        }

        public void SetGameId(string gameName)
        {
            throw new NotImplementedException();
        }

        public void SetMatPlayMode()
        {
            throw new NotImplementedException();
        }

        public void SetMultiplayerGameInfo(string strGameName, string intensityLevel, int clusterId)
        {
            throw new NotImplementedException();
        }

        public void SetOldFMResponseCount(int count)
        {
            throw new NotImplementedException();
        }

        public void SetSinglePlayerGameInfo(string intensityLevel, int clusterId)
        {
            throw new NotImplementedException();
        }

        public void StartCoroutineForBleReConnection()
        {
            throw new NotImplementedException();
        }

        public void StartMPSession()
        {
            throw new NotImplementedException();
        }

        public void StartOperations()
        {
            throw new NotImplementedException();
        }

        public void StartSPSession()
        {
            throw new NotImplementedException();
        }

        public void StoreMPSession(float playerOneGamePoints, float playerTwoGamePoints)
        {
            throw new NotImplementedException();
        }

        public void StoreSPSession(float gamePoints)
        {
            throw new NotImplementedException();
        }

        public void TroubleShootSystem()
        {
            throw new NotImplementedException();
        }

        public void UpdateCurrentTicketData(Dictionary<string, object> currentTicketData)
        {
            throw new NotImplementedException();
        }

        public void UpdateDuration()
        {
            throw new NotImplementedException();
        }

        public void UpdateGameData(Dictionary<string, string> update)
        {
            throw new NotImplementedException();
        }

        public void UpdateOperations()
        {
            throw new NotImplementedException();
        }

        public void UpdateStoreData(Dictionary<string, object> dStoreData)
        {
            throw new NotImplementedException();
        }

        public void UpdateCurrentPlayersMatTutStatus()
        {
            throw new NotImplementedException();
        }
    }
}