using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenuBehavior : MonoBehaviour
{
    public TitleBehavior titleController;

    public void newButton()
    {
        //titleController.newClicked();

        FindObjectOfType<AudioManager>().Play("Menu_Clicked_Play");

        if(SaveSystem.loadPlayer() == null)
        {
            RunStatistics.Instance.isNew = true;
        }
        else
        {
            RunStatistics.Instance.isNew = false;
        }

        FindObjectOfType<AudioManager>().Stop("Title_Theme");
        SceneManager.LoadScene("Main");
    }

    public void loadButton()
    {
        titleController.loadClicked();
    }

    public void creditButton()
    {
        FindObjectOfType<AudioManager>().Stop("Title_Theme");
        FindObjectOfType<AudioManager>().Play("Credit_Theme");
        titleController.creditClicked();
    }

    public void statisticButton()
    {
        //SceneManager.LoadScene("Statistic");
    }

    public void optionButton()
    { 
        titleController.optionClicked();
    }

    public void exitButton()
    {
        Application.Quit();
    }

    public void backButton()
    {
        titleController.menuClicked();
    }

}
