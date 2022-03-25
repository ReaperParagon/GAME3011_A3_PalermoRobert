using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum DifficultyLevel
{
    Easy    = 0,
    Medium  = 1,
    Hard    = 2,
    Chaos   = 8
}

public class TimerScript : MonoBehaviour
{
    [SerializeField]
    public DifficultyLevel currentDifficulty;

    [SerializeField]
    public TextMeshProUGUI timerText;

    [SerializeField]
    public AnimationCurve timerCurve;

    // Timer variables
    public float timeRemaining { get; private set; }
    public bool timerEnabled { get; private set; }

    private void OnEnable()
    {
        MatchThreeEvents.MiniGameStart += SetupTimer;
        MatchThreeEvents.BoardSetup += StartTimer;
        MatchThreeEvents.MiniGameComplete += StopTimer;
    }

    private void OnDisable()
    {
        MatchThreeEvents.MiniGameStart -= SetupTimer;
        MatchThreeEvents.BoardSetup -= StartTimer;
        MatchThreeEvents.MiniGameComplete -= StopTimer;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!timerEnabled)
            return;

        timeRemaining -= Time.fixedDeltaTime;
        CheckTimerEnd();
        UpdateTimerText();
    }

    /// Functions ///

    public void SetupTimer(DifficultyLevel difficultyLevel)
    {
        currentDifficulty = difficultyLevel;

        timeRemaining = 20.0f * Mathf.FloorToInt(timerCurve.Evaluate((int)currentDifficulty));
        UpdateTimerText();
    }
    
    public void StartTimer()
    {
        timerEnabled = true;
    }

    public void StopTimer()
    {
        timerEnabled = false;
    }

    public void CheckTimerEnd()
    {
        if (timeRemaining <= 0.0f)
        {
            timeRemaining = 0.0f;
            MatchThreeEvents.InvokeOnTimerDone();
            timerEnabled = false;
        }
    }

    private void UpdateTimerText()
    {
        timerText.text = GetTimerFormatted();
    }

    public string GetTimerFormatted()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60.0f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60.0f);

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

}

