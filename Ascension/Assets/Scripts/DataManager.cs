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

    public string gameString = "";
    public string[] gameRecord;
    public int gameNumber;

    // Start is called before the first frame update
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

    private void ResetGameRecord(int tT, int cPT)
    {
        tailleTableau = tT;
        coupParTour = cPT;
        gameNumber = 0;
        gameRecord = new string[0];
        SaveGameRecord();
    }

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
    public void ClearGameString()
    {
        gameString = "";
    }

    [System.Serializable]
    class SaveData
    {
        public string[] gameRecord;
        public int gameNumber;
    }

    // C:/Users/tomnd/AppData/LocalLow/DH5/Ascension/

    public void SaveGameRecord()
    {
        SaveData data = new SaveData();
        data.gameNumber = gameNumber;
        data.gameRecord = gameRecord;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile" + tailleTableau + coupParTour + ".json", json);
    }

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
