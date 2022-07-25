using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using Yipli.HttpMpdule.Classes;
using System.Collections.Generic;

// Access a website and use UnityWebRequest.Get to download a page.
// Also try to download a non-existing page. Display the error.
namespace Yipli.HttpMpdule 
{
    public class HTTPRequestManager : MonoBehaviour
    {
        // Required variables
        [Header("Scriptable Objects")]
        [SerializeField] private HTTPYipliConfig currentYipliConfig = null;
        [SerializeField] private YipliConfig fbYipliConfig = null;
        [SerializeField] private HTTPPlayerSelection playerSelection = null;

        // Unity Oprations


        // Data Operations
        public bool GetHttpServerStatus()
        {
            // update this Function for proper status check

            return true;
        }


        public void GatherAllData()
        {
            currentYipliConfig.ResetData();

            //SetGameData();
            //currentYipliConfig.CurrentUserInfo = JsonConvert.DeserializeObject<UserData>(HTTPDataManager.userJsonData);
            ṢetPlayerData();
            //currentYipliConfig.CurrentActiveMatData = JsonConvert.DeserializeObject<MatData>(HTTPDataManager.currentMatJson);
            //currentYipliConfig.AllUrls = JsonConvert.DeserializeObject<UrlData>(HTTPDataManager.urlDataJson);

            //currentYipliConfig.BAllDataIsReceived = true;
            //currentYipliConfig.BIsInternetConnected = true;

            //playerSelection.StartDataManagement = true;
        }

        private void ṢetPlayerData()
        {
            TestData td = JsonConvert.DeserializeObject<TestData>(HTTPDataManager.playerJsonData);

            Debug.LogError(td.players.ToString());

            currentYipliConfig.AllPlayersOfThisUser = JsonConvert.DeserializeObject<List<PlayerInfo>>(td.players.ToString());
        }

        // Specific Data Operations
        private void SetGameData() {
            currentYipliConfig.CurrentGameInfo = JsonConvert.DeserializeObject<GameData>(HTTPDataManager.gameDataJson);
            currentYipliConfig.CurrentGameInfo.ThisGameType = fbYipliConfig.gameType;
        }

        // Request Operations
        IEnumerator GetRequest(string uri)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        break;

                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;

                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;

                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        break;

                    default:
                        Debug.Log("Default case");
                        break;
                }
            }
        }        
    }
}
