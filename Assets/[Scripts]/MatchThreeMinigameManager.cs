using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchThreeMinigameManager : MonoBehaviour
{
    public void StartMinigame(DifficultyLevel difficulty)
    {
        MatchThreeEvents.InvokeOnMiniGameStart(difficulty);
    }

    public void StartMinigame(int difficulty)
    {
        MatchThreeEvents.InvokeOnMiniGameStart((DifficultyLevel)difficulty);
    }
}
