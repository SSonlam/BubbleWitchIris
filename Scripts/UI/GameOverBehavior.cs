using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverBehavior : MonoBehaviour
{
    public Image lostScreenUI;
    private CanvasGroup lostScreenGroup;

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void showLost()
    {
        lostScreenGroup.alpha = 1f;
        lostScreenGroup.blocksRaycasts = true;
        lostScreenGroup.interactable = true;
    }

    // Method used to hide the Lose screen
    public void hideLost()
    {
        lostScreenGroup.alpha = 0f;
        lostScreenGroup.blocksRaycasts = false;
        lostScreenGroup.interactable = false;
    }

    // Method used to update the lost screen.
    public void updateLost()
    {
        GameObject statisticBG = lostScreenUI.transform.Find("Statistic Background").gameObject;
        GameObject scoreText = statisticBG.transform.Find("ScoreText").gameObject;
        GameObject stageText = statisticBG.transform.Find("StageText").gameObject;
        GameObject timeText = statisticBG.transform.Find("TimeText").gameObject;
        GameObject clearText = statisticBG.transform.Find("ClearText").gameObject;
        GameObject chainText = statisticBG.transform.Find("ChainText").gameObject;

        scoreText.GetComponent<Text>().text = "Current Score: " + RunStatistics.Instance.totalScore;
        stageText.GetComponent<Text>().text = "Stage Cleared: " + RunStatistics.Instance.stagesCleared;
        timeText.GetComponent<Text>().text = "Session Time: " + System.Math.Round(RunStatistics.Instance.time, 2);
        clearText.GetComponent<Text>().text = "Bubble Cleared: " + RunStatistics.Instance.bubblesCleared;
        int chainCleard = 0;
        for (int i = 0; i < RunStatistics.Instance.bubblesChainCleared.Length; i++)
        {
            chainCleard += RunStatistics.Instance.bubblesChainCleared[i];
        }

        chainText.GetComponent<Text>().text = "Bubble Chained: " + chainCleard;
    }
}
