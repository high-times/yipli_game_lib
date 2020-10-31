using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YipliUtils
{
     /* ******Gamification*******
     * Function to be called after the gameplay for Report card screen for every game
     * Calculations are aligned to actual cloud functions formulas which gets stored to the player backend
     */
    public static float GetFitnessPoints(IDictionary<PlayerActions, int> playerActionCounts)
    {
        float fp = 0.0f;
        foreach (KeyValuePair<PlayerActions, int> action in playerActionCounts)
        {
            fp += GetFitnessPointsPerAction(action.Key) * action.Value;
        }
        return fp;
    }

    /* ******Gamification*******
    * Function to be called after the gameplay for Experience Points for every game
    * Calculations are aligned to actual cloud functions formulas which gets stored to the player backend
    */
    public static int GetXP(double secs)
    {
        return (int)secs/10;
    }

    /* ******Gamification*******
     * Function to be called after the gameplay for Report card screen for every game
     * Calculations are aligned to actual cloud functions formulas which gets stored to the player backend
     */
    public static float GetCaloriesBurned(IDictionary<PlayerActions, int> playerActionCounts)
    {
        float calories = 0.0f;
        foreach (KeyValuePair<PlayerActions, int> action in playerActionCounts)
        {
            calories += GetCaloriesPerAction(action.Key) * action.Value;
        }
        return calories;
    }

 /* 
  * This function returns Yipli Fitness points predeclared for every player Action.
  * Add a new case here with its identified FPs, whenever a new player action.
  * The values are mapped with the cloud functions algorithm to calculate the fitness points.
  * Change this function, if the values in the cloud-function changes.
  */
    private static float GetFitnessPointsPerAction(PlayerActions playerAction)
    {
        Debug.Log("GetFitnessPointsPerAction() called for " + playerAction);
        float fp = 0.0f;
        switch (playerAction)
        {
            case PlayerActions.LEFTMOVE:
                fp = 10.0f;
                break;
            case PlayerActions.RIGHTMOVE:
                fp = 10.0f;
                break;
            case PlayerActions.JUMP:
                fp = 10.0f;
                break;
            case PlayerActions.RUNNING:
                fp = 4.0f;
                break;
            case PlayerActions.JUMPIN:
                fp = 10.0f;
                break;
            case PlayerActions.JUMPOUT:
                fp = 10.0f;
                break;
            case PlayerActions.STOP:
                fp = 0.0f;
                break;
            default:
                Debug.Log("Invalid action found while calculating the FP. FP returned would be 0.");
                break;
        }
        return fp;
    }


    /* 
    * This class defines all the player actions to be recieved from the FmDriver.
    * Add a new const string here, whenever a new action is added for the player in FmDriver.
    */
    public enum PlayerActions
    {
        LEFT,
        RIGHT,
        ENTER,
        LEFTMOVE,
        RIGHTMOVE,
        JUMP,
        STOP,
        RUNNINGSTOPPED,
        RUNNING,
        PAUSE,
        JUMPIN,
        JUMPOUT,
        TILES,
        INVALID_ACTION
    }

    /* 
     * This function returns calories predeclared for every player Action.
     * Add a new case here with its identified calories burnt, whenever a new player action is added.
     * The values are mapped with the cloud functions algorithm to calculate the calories.
     * Change this function, if the values in the cloud-function changes.
     */
    private static float GetCaloriesPerAction(PlayerActions playerAction)
    {
        Debug.Log("GetCaloriesPerAction() called for " + playerAction);
        float calories = 0.0f;
        switch (playerAction)
        {
            case PlayerActions.LEFTMOVE:
                calories = 0.1f;
                break;
            case PlayerActions.RIGHTMOVE:
                calories = 0.1f;
                break;
            case PlayerActions.JUMP:
                calories = 0.1f;
                break;
            case PlayerActions.RUNNING:
                calories = 0.04f;
                break;
            case PlayerActions.JUMPIN:
                calories = 0.1f;
                break;
            case PlayerActions.JUMPOUT:
                calories = 0.1f;
                break;
            case PlayerActions.STOP:
                calories = 0.1f;
                break;
            default:
                Debug.Log("Invalid action found while calculating the calories. Calories returned would be 0.");
                break;
        }
        return calories;
    }

}
