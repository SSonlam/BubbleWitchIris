using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class StatisticUI : MonoBehaviour
{
    public Image localStatistic;
    public Image globalStatistic;

    private CanvasGroup localCanvas;
    private CanvasGroup globalCanvas;
    private GetLeaderboardResult board;
    public GameObject loaderFinder;
    public Image loader;
    private bool stillLoading = true;
    public GameObject textFinder;
    public Text loadingText;
    private bool loggedInFlag = false;

    public void setLoginFlag()
    {
        loggedInFlag = true;
    }

    void Start()
    {
        FindObjectOfType<AudioManager>().Play("Statistic_BG");
        loaderFinder = GameObject.Find("Loader");
        loader = loaderFinder.GetComponent<Image>();
        loader.enabled = false;
        textFinder = GameObject.Find("Loading");
        loadingText = textFinder.GetComponent<Text>();
        loadingText.enabled = false;
        //FindObjectOfType<AudioManager>().Stop("Title_Theme"); 
        localCanvas = localStatistic.GetComponent<CanvasGroup>();
        globalCanvas = globalStatistic.GetComponent<CanvasGroup>();
        showLocal();
        hideGlobal();
        updateLocal();

        StartCoroutine(updateGlobalWithSleeps());
    }

    IEnumerator updateGlobalWithSleeps()
    {
        //PlayFabManager.thePlayFabManager.Login("ccb");
        //PlayFabManager.thePlayFabManager.Login("ccb");
        //yield return new WaitForSecondsRealtime(3);
        int currentPosition = 1;
        GameObject statisticBG = globalStatistic.transform.Find("Global Statistic View").gameObject;

        PlayFabManager.thePlayFabManager.GetLeaderboard();
        yield return new WaitForSecondsRealtime(3);
        board = PlayFabManager.thePlayFabManager.returnLeaderboard();

        foreach(var item in board.Leaderboard){
            GameObject e = Instantiate(Resources.Load("Prefabs/PlayerRanking") as
                                   GameObject);
            GameObject pPlayerPositionText = e.transform.Find("PlayerPosition").gameObject;
            GameObject pPlayerNameText = e.transform.Find("PlayerName").gameObject;
            GameObject pPlayerScoreText = e.transform.Find("PlayerScore").gameObject;

            pPlayerPositionText.GetComponent<Text>().text = currentPosition.ToString();
            string[] splitArray =  item.DisplayName.Split(char.Parse("/"));
            //strArr = item.DisplayName.Split("/");
            pPlayerNameText.GetComponent<Text>().text = splitArray[0];
            pPlayerScoreText.GetComponent<Text>().text =  item.StatValue.ToString();
            currentPosition++;
            e.transform.SetParent(statisticBG.transform);
            e.transform.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
        loader.enabled = false;
        loadingText.enabled = false;
        stillLoading = false;
    }


    public void showLocal()
    {
        localCanvas.alpha = 1f;
        localCanvas.interactable = true;
        localCanvas.blocksRaycasts = true;
    }
    
    public void hideLocal()
    {
        localCanvas.alpha = 0f;
        localCanvas.interactable = false;
        localCanvas.blocksRaycasts = false;
    }

    public void showGlobal()
    {
        globalCanvas.alpha = 1f;
        globalCanvas.interactable = true;
        globalCanvas.blocksRaycasts = true;
        if(stillLoading)
        {
            loader.enabled = true;
            loadingText.enabled = true;
        }
    }

    public void hideGlobal()
    {
        globalCanvas.alpha = 0f;
        globalCanvas.interactable = false;
        globalCanvas.blocksRaycasts = false;
    }


    /*
     * Method used to update the local statistic Screen
     * I'm not going to lie, I hate the fact that I wrote this.
     * */
    private void updateLocal()
    {
        
        if(SaveSystem.loadPlayer() != null)
        {
            GameObject LocalEmptyUI = localStatistic.transform.Find("Local Empty View").gameObject;
            CanvasGroup emptyView = LocalEmptyUI.GetComponent<CanvasGroup>();
            emptyView.alpha = 0;
            emptyView.interactable = false;
            emptyView.blocksRaycasts = false;

            GameObject statisticBG = localStatistic.transform.Find("Local Statistic View").gameObject;
            GameObject playerText = statisticBG.transform.Find("Player Name").gameObject;
            GameObject totalPlayText = statisticBG.transform.Find("Total Play Time").gameObject;
            GameObject averagePlayText = statisticBG.transform.Find("Average Play Time").gameObject;
            GameObject stageClearText = statisticBG.transform.Find("Stage Cleared").gameObject;
            GameObject maxScoreText = statisticBG.transform.Find("Max Score").gameObject;
            GameObject maxBubbleText = statisticBG.transform.Find("Max Bubble Cleared").gameObject;
            GameObject maxMatchText = statisticBG.transform.Find("Max Bubble Matched").gameObject;
            GameObject lastTimeText = statisticBG.transform.Find("Last Play Time").gameObject;
            GameObject pStageText = statisticBG.transform.Find("Previous Stage Cleared").gameObject;
            GameObject pScoreText = statisticBG.transform.Find("Previous Score").gameObject;
            GameObject pBubbleText = statisticBG.transform.Find("Previous Bubble Cleared").gameObject;
            GameObject pMatchText = statisticBG.transform.Find("Previous Bubble Matched").gameObject;

            PlayerData saveData = SaveSystem.loadPlayer();

            if(saveData.playerName == "")
            {
                playerText.GetComponent<Text>().text = "Player Name: Iris";
            }
            else
            {
                playerText.GetComponent<Text>().text = "Player Name: " + saveData.playerName;
            }
            
            totalPlayText.GetComponent<Text>().text = "Total Play Time: " + System.Math.Round(saveData.totalPlayTime,2);
            averagePlayText.GetComponent<Text>().text = "Average Stage Time: " + System.Math.Round(saveData.averageStageTime, 2);
            stageClearText.GetComponent<Text>().text = "Stage Cleared: " + saveData.stageCleared;
            maxScoreText.GetComponent<Text>().text = "Max Score: " + saveData.maxScore;
            maxBubbleText.GetComponent<Text>().text = "Max Bubble Cleared: " + saveData.maxBubbleCleared;
            maxMatchText.GetComponent<Text>().text = "Max Bubble Matched: " + saveData.maxBubbleMatched;
            lastTimeText.GetComponent<Text>().text = "Last Session Time: " + System.Math.Round(saveData.lastSessionTime, 2);
            pStageText.GetComponent<Text>().text = "Previous Stage Cleared: " + saveData.lastStageCleared;
            pScoreText.GetComponent<Text>().text = "Previous Score: " + saveData.lastScore;
            pBubbleText.GetComponent<Text>().text = "Previous Bubble Cleared: " + saveData.lastBubbleCleared;
            pMatchText.GetComponent<Text>().text = "Previous Bubble Matched: " + saveData.lastBubbleMatched;
        }
    }
}
