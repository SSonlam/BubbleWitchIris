using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName { get; set; } = "Iris";
    public float totalPlayTime { get; private set; }
    public float averageStageTime { get; private set; } = 0;
    public int stageCleared { get; private set; } = 0;
    public int maxLevel { get; private set; } = 0;
    public int maxScore { get; private set; } = 0;
    public int maxBubbleCleared { get; private set; } = 0;
    public int maxBubbleMatched { get; private set; } = 0;
    public int maxBossCleared { get; private set; } = 0;
    public float lastSessionTime;
    public int lastStageCleared;
    public int lastLevel;
    public int lastScore;
    public int lastBubbleCleared;
    public int lastBubbleMatched;
    public int lastBossCleared;
    public int saveNum { get; set; } = 0;

    public PlayerData()
    {
    }

    public float getSessionTime()
    {
        return lastSessionTime;
    }

    public void setSessionTime(float input)
    {
        lastSessionTime = input;
        totalPlayTime = totalPlayTime + lastSessionTime;
    }

    public int getStageCleared()
    {
        return lastStageCleared;
    }

    public void setStageCleared(int input)
    {
        lastStageCleared = input;
        stageCleared += input;
    }

    public int getLevel()
    {
        return lastLevel;
    }

    public void setLevel(int input)
    {
        lastLevel = input;

        if(maxLevel < input)
        {
            maxLevel = input;
        }

    }

    public int getScore()
    {
        return lastScore;
    }

    public void setScore(int input)
    {
        lastScore = input;

        if(maxScore < input)
        {
            maxScore = input;
        }
    }

    public int getBubbleCleared()
    {
        return lastBubbleCleared;
    }

    public void setBubbleCleared(int input)
    {
        lastBubbleCleared = input;

        if(maxBubbleCleared < input)
        {
            maxBubbleCleared = input;
        }
    }

    public int getBubbleMatched()
    {
        return lastBubbleMatched;
    }

    public void setbubbleMatched(int input)
    {
        lastBubbleMatched = input;

        if(maxBubbleMatched < input)
        {
            maxBubbleMatched = input;
        }
    }

    public int getBossCleared()
    {
        return lastBossCleared;
    }

    public void setBossCleared(int input)
    {
        lastBossCleared = input;

        if(maxBossCleared < input)
        {
            maxBossCleared = input;
        }
    }

    public void calculateStageAverage()
    {
        if(stageCleared != 0)
        {
            averageStageTime = (totalPlayTime / stageCleared);
        }
        else
        {
            averageStageTime = 0;
        }
        
    }
}
