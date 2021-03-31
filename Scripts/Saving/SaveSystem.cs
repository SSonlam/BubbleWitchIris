using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveSystem
{
    
    /*
     * savePlayer Method
     * */
    public static void savePlayer()
    {
        

        if(loadPlayer() != null)
        {
            PlayerData playerData = loadPlayer();
            toSave(playerData);
        }
        else
        {
            PlayerData playerData = new PlayerData();
            toSave(playerData);
        }

        
    }

    private static void toSave(PlayerData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string playerName = RunStatistics.Instance.playerName;

        string savePath = Application.persistentDataPath + "/SaveData.score";
        FileStream stream = new FileStream(savePath, FileMode.Create);

        data.playerName = playerName;
        data.setSessionTime(RunStatistics.Instance.time);
        data.setStageCleared(RunStatistics.Instance.stagesCleared);
        data.setScore(RunStatistics.Instance.totalScore);
        data.setBubbleCleared(RunStatistics.Instance.bubblesCleared);

        int chainCleard = 0;
        for (int i = 0; i < RunStatistics.Instance.bubblesChainCleared.Length; i++)
        {
            chainCleard += RunStatistics.Instance.bubblesChainCleared[i];
        }

        data.setbubbleMatched(chainCleard);
        data.setBossCleared(RunStatistics.Instance.bossCleared);
        data.calculateStageAverage();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData loadPlayer()
    {
        string path = Application.persistentDataPath + "/SaveData.score";

        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Save file not found in " + path);
            return null;
        }
    }

    public static void quickSave(QuickSaveData newSave)
    {

        string jsonSave = JsonUtility.ToJson(newSave);
        File.WriteAllText(Application.persistentDataPath + "/QuickData.quick", jsonSave);
        Debug.Log(jsonSave);


        /*
         * 
         *  BinaryFormatter formatter = new BinaryFormatter();

        string playerName = RunStatistics.Instance.playerName;

        string savePath = Application.persistentDataPath + "/QuickData.quick";
        FileStream stream = new FileStream(savePath, FileMode.Create);
        QuickSaveData data = new QuickSaveData();


        data.playerName = playerName;
        data.time = RunStatistics.Instance.time;
        data.stagesCleared = (RunStatistics.Instance.stagesCleared);
        data.totalScore = (RunStatistics.Instance.totalScore);
        data.bubblesCleared = (RunStatistics.Instance.bubblesCleared);
        data.bubblesChainCleared = (RunStatistics.Instance.bubblesChainCleared);
        data.bossCleared = (RunStatistics.Instance.bossCleared);
        data.trapCount = (RunStatistics.Instance.trapCount);

        data.totalBubbles = GameManager.theManager.bubbleCounter;

        data.currentBubbleUnit = newSave.currentBubbleUnit;
        data.currentBubbleSpirit = newSave.currentBubbleSpirit;
        data.currentBubbleProjectile = newSave.currentBubbleSpirit;
        data.thePlayer = newSave.thePlayer;
        data.currentPlayerProjectile = newSave.currentPlayerProjectile;

        formatter.Serialize(stream, data);
        stream.Close();
         * */

    }

    public static QuickSaveData quickLoad()
    {
        string path = Application.persistentDataPath + "/QuickData.quick";

        if (File.Exists(path))
        {
            string savePath = File.ReadAllText(path);
            QuickSaveData loadQuicksave = JsonUtility.FromJson<QuickSaveData>(savePath);
            return loadQuicksave;
        }
        else
        {
            Debug.Log("Save file not found in " + path);
            return null;
        }
    }

    public static void deleteSaveData()
    {
        string path = Application.persistentDataPath + "/SaveData.score";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            Debug.Log("No Files To Delete");
        }
    }

    public static void deleteQuick()
    {
        string path = Application.persistentDataPath + "/QuickData.quick";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            Debug.Log("No Files To Delete");
        }
    }

}
