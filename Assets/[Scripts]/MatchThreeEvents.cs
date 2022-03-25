using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchThreeEvents
{
    /// Timer Done ///

    public delegate void OnTimerDone();

    public static event OnTimerDone TimerFinished;

    public static void InvokeOnTimerDone()
    {
        TimerFinished?.Invoke();
    }

    /// Minigame Start ///

    public delegate void OnMiniGameStart(DifficultyLevel difficultyLevel);

    public static event OnMiniGameStart MiniGameStart;

    public static void InvokeOnMiniGameStart(DifficultyLevel difficultyLevel)
    {
        MiniGameStart?.Invoke(difficultyLevel);
    }

    /// Board Setup ///

    public delegate void OnBoardSetup();

    public static event OnBoardSetup BoardSetup;

    public static void InvokeOnBoardSetup()
    {
        BoardSetup?.Invoke();
    }


    /// Minigame Complete ///

    public delegate void OnMiniGameComplete();

    public static event OnMiniGameComplete MiniGameComplete;

    public static void InvokeOnMiniGameComplete()
    {
        MiniGameComplete?.Invoke();
    }

    /// Add Score ///

    public delegate void OnAddScore(int score);

    public static event OnAddScore AddScore;

    public static void InvokeOnAddScore(int score)
    {
        AddScore?.Invoke(score);
    }

    /// Bomb Triggered ///

    public delegate void OnBomb();

    public static event OnBomb BombTriggered;

    public static void InvokeOnBomb()
    {
        BombTriggered?.Invoke();
    }
}
