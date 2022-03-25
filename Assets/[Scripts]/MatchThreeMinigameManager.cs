using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchThreeMinigameManager : MonoBehaviour
{
    public GameObject resultsScreen;
    public TextMeshProUGUI resultsText;
    public GameObject hintScreen;
    public Animator hintAnimator;
    public float hintScreenTime = 3.0f;

    private void OnEnable()
    {
        hintScreen.SetActive(false);

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

    public void Hint(string msg)
    {
        StartCoroutine(DisplayHintMessage(msg));
    }

    public IEnumerator DisplayHintMessage(string msg)
    {
        hintScreen.SetActive(true);

        yield return new WaitForFixedUpdate();

        var text = hintScreen.GetComponent<TextMeshProUGUI>();
        text.text = msg;

        hintAnimator.SetTrigger("AnimTrigger");

        yield return new WaitForSeconds(hintScreenTime);

        hintScreen.SetActive(false);
    }
}
