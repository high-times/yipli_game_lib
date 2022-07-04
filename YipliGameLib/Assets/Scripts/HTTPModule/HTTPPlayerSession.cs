using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yipli.HttpMpdule
{
    public class HTTPPlayerSession : MonoBehaviour
    {
        // static variables
        private static HTTPPlayerSession _instance;
        public static HTTPPlayerSession Instance { get { return _instance; } }

        internal void AddMultiPlayerAction(YipliUtils.PlayerActions action, PlayerDetails playerDetails, int count)
        {
            throw new NotImplementedException();
        }

        internal void AddPlayerAction(YipliUtils.PlayerActions action, int count)
        {
            throw new NotImplementedException();
        }

        internal void ChangePlayer()
        {
            throw new NotImplementedException();
        }

        internal void CloseSPSession()
        {
            throw new NotImplementedException();
        }

        internal string GetCurrentPlayer()
        {
            throw new NotImplementedException();
        }

        internal string GetCurrentPlayerId()
        {
            throw new NotImplementedException();
        }

        internal IDictionary<YipliUtils.PlayerActions, int> getMultiPlayerActionCounts(PlayerDetails playerDetails)
        {
            throw new NotImplementedException();
        }

        internal Dictionary<string, dynamic> GetMultiPlayerSessionDataJsonDic(PlayerDetails playerDetails, string mpSessionUUID)
        {
            throw new NotImplementedException();
        }

        internal IDictionary<YipliUtils.PlayerActions, int> getPlayerActionCounts()
        {
            throw new NotImplementedException();
        }

        internal Dictionary<string, dynamic> GetPlayerSessionDataJsonDic()
        {
            throw new NotImplementedException();
        }

        internal void GotoYipli()
        {
            throw new NotImplementedException();
        }

        internal void HTTPAwakeOperations()
        {
            throw new NotImplementedException();
        }

        internal void LoadingScreenSetActive(bool bOn)
        {
            throw new NotImplementedException();
        }

        internal void PauseMPSession()
        {
            throw new NotImplementedException();
        }

        internal void PauseSPSession()
        {
            throw new NotImplementedException();
        }

        internal void ReInitializeSPSession()
        {
            throw new NotImplementedException();
        }

        internal void ResumeMPSession()
        {
            throw new NotImplementedException();
        }

        internal void ResumeSPSession()
        {
            throw new NotImplementedException();
        }

        internal void SetGameId(string gameName)
        {
            throw new NotImplementedException();
        }

        internal void StartCoroutineForBleReConnection()
        {
            throw new NotImplementedException();
        }

        internal void StartMPSession()
        {
            throw new NotImplementedException();
        }

        internal void StartOperations()
        {
            throw new NotImplementedException();
        }

        internal void StartSPSession()
        {
            throw new NotImplementedException();
        }

        internal void StoreMPSession(float playerOneGamePoints, float playerTwoGamePoints)
        {
            throw new NotImplementedException();
        }

        internal void StoreSPSession(float gamePoints)
        {
            throw new NotImplementedException();
        }

        internal void UpdateDuration()
        {
            throw new NotImplementedException();
        }

        internal void UpdateGameData(Dictionary<string, string> update)
        {
            throw new NotImplementedException();
        }

        internal void UpdateOperations()
        {
            throw new NotImplementedException();
        }

        internal void UpdateStoreData(Dictionary<string, object> dStoreData)
        {
            throw new NotImplementedException();
        }
    }
}