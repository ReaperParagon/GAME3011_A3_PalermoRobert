using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreScript : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI scoreUI;
    [SerializeField]
    private GameObject scoreAddUI;
    public float animationTime = 1.0f;

    public int Score { get; private set; }
    private int tickUpScore;
    private int tempAddingScore;
    private GameObject tempAddScoreObject;

    private IEnumerator AnimateAddScoreCoroutine_Ref = null;


    private void OnEnable()
    {
        MatchThreeEvents.AddScore += AddScore;
    }

    private void OnDisable()
    {
        MatchThreeEvents.AddScore -= AddScore;
    }


    private void AddScore(int score)
    {
        tickUpScore = Score;
        Score += score;

        int scoreToAdd = score;

        if (AnimateAddScoreCoroutine_Ref != null)
        {
            StopCoroutine(AnimateAddScoreCoroutine_Ref);
            Destroy(tempAddScoreObject);
            scoreToAdd = score + tempAddingScore;
        }

        AnimateAddScoreCoroutine_Ref = AnimateAddScore(scoreToAdd);
        StartCoroutine(AnimateAddScoreCoroutine_Ref);
    }

    private IEnumerator AnimateAddScore(int score)
    {
        tempAddingScore = score;

        // Display added score
        tempAddScoreObject = Instantiate(scoreAddUI, transform);
        tempAddScoreObject.GetComponent<TextMeshProUGUI>().text = "+" + score;

        yield return new WaitForSeconds(animationTime);
        Destroy(tempAddScoreObject);

        // Tick up score UI to the added score total
        for (; tickUpScore < Score; tickUpScore += 1)
        {
            scoreUI.text = tickUpScore.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        scoreUI.text = Score.ToString();

        AnimateAddScoreCoroutine_Ref = null;
    }

}
