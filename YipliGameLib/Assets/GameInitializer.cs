using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public YipliConfig currentYipliConfig;
    private void Awake()
    {
        currentYipliConfig.gameType = GameType.MULTIPLAYER_GAMING;
        currentYipliConfig.gameId = "multiplayermayhem";
    }
}
