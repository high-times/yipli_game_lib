using System;
using UnityEngine;
using YipliFMDriverCommunication;

namespace Yipli.ControlSystem
{
    public class CentralControlSystem : MonoBehaviour
    {
        [Header("Required Scriptables")]
        [SerializeField] private YipliConfig currentYipliConfig;

        // private variables
        YipliUtils.PlayerActions detectedAction;

        // Events and actions
        // Action <action_name, action_id, speed, step_count>
        public static Action<YipliUtils.PlayerActions, string, float, float> CurrentAction;

        // Getters And Setters
        public YipliUtils.PlayerActions DetectedAction { get => detectedAction; set => detectedAction = value; }

        // Update is called once per frame
        void Update()
        {
            //ManageMatResponse();        
        }

        private void ManageMatResponse()
        {
            //if (!currentYipliConfig.onlyMatPlayMode) return;
            
            string fmActionData = InitBLE.GetFMResponse();
            Debug.Log("Json Data from Fmdriver in matinput : " + fmActionData);

            FmDriverResponseInfo singlePlayerResponse = null;

            try {
                singlePlayerResponse = JsonUtility.FromJson<FmDriverResponseInfo>(fmActionData);
            } catch (System.Exception e) {
                Debug.Log("singlePlayerResponse is having problem : " + e.Message);
            }

            if (singlePlayerResponse == null) return;

            if (currentYipliConfig.oldFMResponseCount != singlePlayerResponse.count)
            {
                PlayerSessionFB.Instance.currentYipliConfig.oldFMResponseCount = singlePlayerResponse.count;

                DetectedAction = ActionAndGameInfoManager.GetActionEnumFromActionID(singlePlayerResponse.playerdata[0].fmresponse.action_id);

                string[] tokens = singlePlayerResponse.playerdata[0].fmresponse.properties.Split(',');
                float footSteps = 0f;
                float currentSteps = 0f;
                float speed = 0f;

                if (tokens.Length > 0)
                {
                    //Split the property value pairs:
                    string[] totalStepsCountKeyValue = tokens[1].Split(':');
                    if (totalStepsCountKeyValue[0].Equals("totalStepsCount"))
                    {
                        footSteps += int.Parse(totalStepsCountKeyValue[1]);
                        Debug.Log("Total footSteps : " + footSteps);

                        Debug.Log("Adding steps : " + totalStepsCountKeyValue[1]);

                        currentSteps = int.Parse(totalStepsCountKeyValue[1]);
                    }

                    string[] speedKeyValue = tokens[0].Split(':');
                    if (speedKeyValue[0].Equals("speed"))
                    {
                        //TODO : Do some handling if speed parameter needs to be used to adjust the running speed in the game.
                        speed = float.Parse(speedKeyValue[1]);
                    }
                }
            }
        }

        /*

        packageFMResponse: 
        { "count":53, 
            "timestamp":1619501976556, 
            "playerdata":
            [
                {
                    "id":1, 
                    "fmresponse":
                        {
                            "action_id": "SWLO", 
                            "action_name" : "Running", 
                            "properties": "speed:4.499460, totalStepsCount:7"
                        }
                }
            ]
        }

        */
    }
}