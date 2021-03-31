using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonControl : MonoBehaviour
{

    public PlayerBehavior thePlayer;
    public GameUIControl theUI;
    public StatisticUI theStatistic;
    public Text userInput;
    public GameObject loaderFinder;
    public Image loader;
    public GameObject textFinderForSent;
    public Text sentText;
    public GameObject textFinderForSending;
    public Text sendingText;
    
    // Function used to set the game back to run
    public void resumeGame()
    {
        GameManager.theManager.setRun();
    }

    // your upgrade music sequence is broken.
    // hack to fix an annoying bug that nathan refuses to fix
    public void delayedUpgradeMusicStop()
    {
        FindObjectOfType<AudioManager>().Stop("Upgrading_BG");
    }

    // Function used to restarting the game in the current game scene
    public void gameToNewGame()
    {
        FindObjectOfType<AudioManager>().Stop("Stage_BG");
        FindObjectOfType<AudioManager>().Stop("Upgrading_BG");
        Invoke("delayedUpgradeMusicStop", 1.5f);
        GameManager.theManager.saveGame();
        GameManager.theManager.restartLevel();
    }

    // Function used to continue the current game save
    public void continueGame()
    {
        GameManager.theManager.setUpgrade();
    }

    // Function used to move from upgrade sequence to next sequence
    public void continueFromUpgrade()
    {
        FindObjectOfType<AudioManager>().Stop("Stage_BG");
        FindObjectOfType<AudioManager>().Stop("Upgrading_BG");
        Invoke("delayedUpgradeMusicStop", 1.5f);
        GameManager.theManager.saveGame();
        GameManager.theManager.setNextSequence();
    }

    // Function used to move the player to the title screen from game scene
    public void gameToMenu()
    {
        GameManager.theManager.saveGame();
        FindObjectOfType<AudioManager>().Stop("Stage_BG"); 
        FindObjectOfType<AudioManager>().Stop("Upgrading_BG");
        Invoke("delayedUpgradeMusicStop", 1.5f);
        FindObjectOfType<AudioManager>().Stop("Statistic_BG");
        RunStatistics.Instance.bubblesChainCleared = new int[BubbleColor.count];
        SceneManager.LoadScene("TitleScreen");
    }

    public void menuToTitle()
    {      
        FindObjectOfType<AudioManager>().Stop("Stage_BG"); 
        FindObjectOfType<AudioManager>().Stop("Upgrading_BG");
        Invoke("delayedUpgradeMusicStop", 1.5f);
        FindObjectOfType<AudioManager>().Stop("Statistic_BG");
        SaveSystem.deleteQuick();
        RunStatistics.Instance.bubblesChainCleared = new int[BubbleColor.count];
        SceneManager.LoadScene("TitleScreen");
    }

    // Function used to move the player to the title screen from statistic scene
    public void statisticToMenu()
    {
        FindObjectOfType<AudioManager>().Stop("Statistic_BG");
        SceneManager.LoadScene("TitleScreen");
    }

    public void quickSaveAndQuit()
    {
        GameManager.theManager.quickSave();
        FindObjectOfType<AudioManager>().Stop("Stage_BG");
        FindObjectOfType<AudioManager>().Stop("Upgrading_BG");
        Invoke("delayedUpgradeMusicStop", 1.5f);
        SceneManager.LoadScene("TitleScreen");
    }

    public void pauseToOption()
    {
        theUI.hideMenu();
        theUI.showOption();
    }

    public void optionBackButton()
    {
        theUI.hideOption();
        theUI.showMenu();
    }

    public void lostToSendButton()
    {
        loaderFinder = GameObject.Find("SendScoreUI/SendBackground/Loader");
        loader = loaderFinder.GetComponent<Image>();
        loader.enabled = false;

        textFinderForSent = GameObject.Find("SendScoreUI/SendBackground/SentText");
        sentText = textFinderForSent.GetComponent<Text>();
        sentText.enabled = false;

        textFinderForSending = GameObject.Find("SendScoreUI/SendBackground/Sending");
        sendingText = textFinderForSending.GetComponent<Text>();
        sendingText.enabled = false;

        theUI.showSendScore();
        theUI.hideLost();
    }

    public void sendBackButton()
    {
        theUI.hideSendScore();
        theUI.showLost();
    }


    // Method used to send the player's score to the leaderboard
    public void sendScore()
    {
        string pName = userInput.text.Trim();

        if (pName != "" && pName.Length >= 3 && pName.Length <= 6)
        {
            StartCoroutine(SendHighScore(pName));
            //FindObjectOfType<AudioManager>().Play("Menu_Clicked_Play");
            RunStatistics.Instance.playerName = pName; // Set the game's playerName to the correct playerName

            //FindObjectOfType<AudioManager>().Stop("Title_Theme");
            //SceneManager.LoadScene("TitleScreen");
        }
        else
        {
            
        }
        
    }

    IEnumerator SendHighScore(string firstName)
    {
        loaderFinder = GameObject.Find("SendScoreUI/SendBackground/Loader");
        loader = loaderFinder.GetComponent<Image>();
        loader.enabled = true;
        textFinderForSending = GameObject.Find("SendScoreUI/SendBackground/Sending");
        sendingText = textFinderForSending.GetComponent<Text>();
        sendingText.enabled = true;
        PlayFabManager.thePlayFabManager.Login(firstName);
        yield return new WaitForSecondsRealtime(3);
        PlayFabManager.thePlayFabManager.SendLeaderboard(RunStatistics.Instance.totalScore);
        loader.enabled = false;
        sendingText.enabled = false;
        textFinderForSent = GameObject.Find("SendScoreUI/SendBackground/SentText");
        sentText = textFinderForSent.GetComponent<Text>();
        sentText.enabled = true;
    }

    public void upgradeTrap()
    {
        if (RunStatistics.Instance.totalScore >= 500)
        {
            if (thePlayer.trapCountCap < 10f)
            {

                thePlayer.trapCountCap = thePlayer.trapCountCap + 3;
                RunStatistics.Instance.totalScore -= 500;

                if (thePlayer.trapCountCap >= 10)
                {
                    theUI.trapMax = true;
                }
            }
        }
    }

    public void upgradeCapture()
    {
        if (RunStatistics.Instance.totalScore >= 500)
        {
            if (thePlayer.captureCoolDown > 1f)
            {
                thePlayer.captureCoolDown = thePlayer.captureCoolDown - 0.2f;
                theUI.captureCooldown = thePlayer.captureCoolDown;
                RunStatistics.Instance.totalScore -= 500;

                if (thePlayer.captureCoolDown <= 1f)
                {
                    theUI.captureMax = true;
                }
            }
        }
    }

    public void upgradeRoll()
    {
        if (RunStatistics.Instance.totalScore >= 500)
        {
            if (thePlayer.dashCoolDown > 4f)
            {
                thePlayer.dashCoolDown = thePlayer.dashCoolDown - 0.5f;
                RunStatistics.Instance.totalScore -= 500;

                if (thePlayer.dashCoolDown <= 4f)
                {
                    theUI.rollMax = true;
                }
            }
        }
    }

    public void upgradeLife()
    {
        if (RunStatistics.Instance.totalScore >= 1000)
        {
            if (RunStatistics.Instance.currentLife < 3f)
            {
                RunStatistics.Instance.currentLife += 1;
                RunStatistics.Instance.totalScore -= 1000;
            }
        }
    }

    public void toShowlocalStatistic()
    {
        theStatistic.showLocal();
        theStatistic.hideGlobal();
    }

    public void toShowGlobalStatistic()
    {

        theStatistic.showGlobal();
        theStatistic.hideLocal();
    }

    public void hoverOne()
    {
        theUI.setCost(500);
    }

    public void hoverTwo()
    {
        theUI.setCost(1000);
    }

    public void hoverTrap()
    {
        theUI.setUpgradeHelp("Increase Bubble Trap Cap");
    }

    public void hoverCapture()
    {
        theUI.setUpgradeHelp("Reduce Bubble Capture Cooldown");
    }

    public void hoverRoll()
    {
        theUI.setUpgradeHelp("Reduce Bubble Roll Cooldown");
    }

    public void hoverLife()
    {
        theUI.setUpgradeHelp("Add An Additional Life");
    }

    public void offHoverUpgrade()
    {
        theUI.hideUpgradeHelp();
    }

    public void offHoverCost()
    {
        theUI.hideCost();
    }

    public void nextHelp()
    {
        theUI.helpPageTwo();
    }

    public void secondNextHelp()
    {
        theUI.helpPageThree();
    }

    public void previousHelp()
    {
        theUI.helpPageOne();
    }
}
