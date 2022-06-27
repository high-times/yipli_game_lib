using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public YipliConfig currentYipliConfig;

    private void Awake()
    {
        // comment if part for release
        ///*
        if (!currentYipliConfig.dataReseted)
        { 
            currentYipliConfig.ResetThisObjectsData();
            currentYipliConfig.dataReseted = true;
        }
        //*/

        currentYipliConfig.gameType = GameType.FITNESS_GAMING;
        currentYipliConfig.gameId = "skateroverdrive";
    }
}
