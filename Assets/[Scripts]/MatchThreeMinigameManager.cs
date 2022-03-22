using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchThreeMinigameManager : MonoBehaviour
{
    public GameObject resultsScreen;
    public TextMeshProUGUI resultsText;

    private void OnEnable()
    {
        MatchThreeEvents.MiniGameStart += Setup;
        MatchThreeEvents.MiniGameComplete += GameComplete;
        MatchThreeEvents.TimerFinished += OutOfTime;
    }

    private void OnDisable()
    {
        MatchThreeEvents.MiniGameStart -= Setup;
        MatchThreeEvents.MiniGameComplete -= GameComplete;
        MatchThreeEvents.TimerFinished -= OutOfTime;
    }

    public void StartMinigame(DifficultyLevel difficulty)
    {
        MatchThreeEvents.InvokeOnMiniGameStart(difficulty);
    }

    public void StartMinigame(int difficulty)
    {
        MatchThreeEvents.InvokeOnMiniGameStart((DifficultyLevel)difficulty);
    }

    private void GameComplete()
    {
        DisplayResults("You Beat the Level!");
    }

    private void OutOfTime()
    {
        DisplayResults("You Ran Out of Time!");
    }

    private void DisplayResults(string message)
    {
        print(message);
        resultsText.text = message;
        resultsScreen.SetActive(true);
    }

    private void Setup(DifficultyLevel _)
    {
        resultsScreen.SetActive(false);
    }
}
