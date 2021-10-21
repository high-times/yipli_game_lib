using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public YipliConfig currentYipliConfig;
    private void Awake()
    {
        currentYipliConfig.gameType = GameType.FITNESS_GAMING;
        currentYipliConfig.gameId = "skateroverdrive";
    }
}
