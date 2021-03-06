using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager InstanceDataManager { get; private set; }

    public string modeJ1;
    public string modeJ2;
    public int tailleTableau;
    public int coupParTour;
    public bool delayAI = true;

    public string gameString = "";
    public string[] gameRecord;
    public int gameNumber;

    /// <summary>
    /// Initialize the DataManager instance to pass informations through the scenes
    /// </summary>
    void Start()
    {
        if (InstanceDataManager != null)
        {
            Destroy(gameObject);
            return;
        }
        InstanceDataManager = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Reset the chosen game record file (given the mode)
    /// !!! Be careful with this function, irreversible !!!
    /// </summary>
    /// <param name="tT">Size of the board</param>
    /// <param name="cPT">Number of move er turn</param>
    private void ResetGameRecord(int tT, int cPT)
    {
        tailleTableau = tT;
        coupParTour = cPT;
        gameNumber = 0;
        gameRecord = new string[0];
        SaveGameRecord();
    }

    /// <summary>
    /// Add the new game string to the game record file
    /// </summary>
    public void AddGameStringToRecord()
    {
        LoadGameRecord();
        gameNumber++;
        if (gameRecord.Length > 0) { gameRecord[gameNumber - 1] = gameString; }
        else
        {
            gameRecord = new string[1];
            gameRecord[0] = gameString;
        }
    }

    /// <summary>
    /// Update the current game string with the last move
    /// </summary>
    /// <param name="c">Move to add</param>
    public void UpdateGameString(string c)
    {
        gameString += c;
        //Debug.Log(gameString);
        if (c == "//1" || c == "//2" || c == "//0")
        {
            AddGameStringToRecord();
            SaveGameRecord();
            ClearGameString();
        }
    }

    /// <summary>
    /// Clear the current game string
    /// Used for an unfinished game
    /// </summary>
    public void ClearGameString()
    {
        gameString = "";
    }

    /// <summary>
    /// Class used to save the game record and the number of games
    /// </summary>
    [System.Serializable]
    class SaveData
    {
        public string[] gameRecord;
        public int gameNumber;
    }

    // C:/Users/tomnd/AppData/LocalLow/DH5/Ascension/

    /// <summary>
    /// Save the game record in the corresponding file
    /// </summary>
    public void SaveGameRecord()
    {
        SaveData data = new SaveData();
        data.gameNumber = gameNumber;
        data.gameRecord = gameRecord;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile" + tailleTableau + coupParTour + ".json", json);
    }

    /// <summary>
    /// Load the game record from the corresponding file
    /// </summary>
    public void LoadGameRecord()
    {
        string path = Application.persistentDataPath + "/savefile" + tailleTableau + coupParTour + ".json";
        Debug.Log(path);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            gameNumber = data.gameNumber;
            gameRecord = new string[gameNumber + 1];
            data.gameRecord.CopyTo(gameRecord, 0);           
        }
    }
}
