using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    string savePath;
    public Text userInput;
    private bool isShake;
    private float shakeTime;
    private Vector3 originalPos;
    private float shakeMagnitude;
    public Text helpText = null;

    private CanvasGroup helpGroup;
    void Start()
    {
        shakeTime = 0f;
        originalPos = transform.localPosition;
        if(helpText != null)
        {
            helpGroup = helpText.GetComponent<CanvasGroup>();
        }
    }

    void FixedUpdate()
    {
        checkTime();
        if (isShake == true)
            {
                shake();
            }

    }


    public void newGame()
    {
        string pName = userInput.text.Trim();

        if (pName != "" && pName.Length >= 3 && pName.Length <= 6)
        {
            FindObjectOfType<AudioManager>().Play("Menu_Clicked_Play");
            RunStatistics.Instance.playerName = pName; // Set the game's playerName to the correct playerName
            RunStatistics.Instance.isNew = true;
            
            FindObjectOfType<AudioManager>().Stop("Title_Theme"); 
            SceneManager.LoadScene("Main");
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("Menu_Play_NoName");
            isShake = true;
            shakeTime = Time.timeSinceLevelLoad + 1f;
            shakeMagnitude = 1f;
        }
    }

    // Method used to load the player's progress
    public void loadGame()
    {
        if(SaveSystem.loadPlayer() != null)
        {
            FindObjectOfType<AudioManager>().Play("Menu_Clicked_Play");
            PlayerData playerData = SaveSystem.loadPlayer();
            RunStatistics.Instance.playerName = playerData.playerName;
            RunStatistics.Instance.isQuick = false;
            RunStatistics.Instance.isNew = false;
            FindObjectOfType<AudioManager>().Stop("Title_Theme"); 
            SceneManager.LoadScene("Main");
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("Menu_Play_NoName");
            isShake = true;
            shakeTime = Time.timeSinceLevelLoad + 1f;
            shakeMagnitude = 1f;
        }
        
    }

    // Method used to let the player continue from quick saves
    public void continueGame()
    {
        FindObjectOfType<AudioManager>().Play("Menu_Clicked_Play");
        RunStatistics.Instance.isQuick = true;
        RunStatistics.Instance.isNew = false;
        FindObjectOfType<AudioManager>().Stop("Title_Theme");
        SceneManager.LoadScene("Main");
    }

    public void clearSaves()
    {
        SaveSystem.deleteSaveData();
    }

    public void viewStatistic()
    {
        FindObjectOfType<AudioManager>().Stop("Stage_BG");
        FindObjectOfType<AudioManager>().Stop("Title_Theme");
        SceneManager.LoadScene("Statistic");
    }

    public void setPath(string iPath)
    {
        savePath = iPath;
    }

    private void checkTime()
    {
        if (Time.timeSinceLevelLoad >= shakeTime)
        {
            isShake = false;
        }
    }

    private void shake()
    {
        Vector3 Pos = originalPos;
        Pos.x += shakeMagnitude * Mathf.Sin(Time.time * 62.832f); // Sin for more even shake
        Pos.y += shakeMagnitude * Mathf.Sin(Time.time * 62.832f); // 62.832f ensure 10 frequency per sec
        transform.localPosition = Pos;
        shakeMagnitude -= shakeMagnitude * Time.smoothDeltaTime;
    }

    public void onNewText()
    {
        showHelpText();
        helpText.text = "Start a new game";
    }

    public void onContinueText()
    {
        showHelpText();
        helpText.text = "Continue from a quick save";
    }

    public void onStatisticText()
    {
        showHelpText();
        helpText.text = "Check your all time statistic and global leaderboard";
    }

    public void onOptionText()
    {
        showHelpText();
        helpText.text = "Change the game's setting";
    }

    public void onCreditText()
    {
        showHelpText();
        helpText.text = "Show who made Bubble Witch Iris";
    }

    public void onQuitText()
    {
        showHelpText();
        helpText.text = "Exit application";
    }

    public void offButtonText()
    {
        hideHelpText();
    }

    public void showHelpText()
    {
        helpGroup.alpha = 1;
    }

    public void hideHelpText()
    {
        helpGroup.alpha = 0;
    }

}
