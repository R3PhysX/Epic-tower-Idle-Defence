using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ActiveGameData
{
    private static ActiveGameData _instance;

    public static ActiveGameData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ActiveGameData();
            }
            return _instance;
        }
    }

    public SaveData saveData = new SaveData();

    internal bool adFor4x = true;
    internal bool adFor2_4x;
    internal bool adFor1_4x;
    internal bool dailyRewardPopupShown = false;

    internal bool diceRollActive = false;

    internal int currentSelectedWorld { get { return saveData.currentSelectedWorld; } set { saveData.currentSelectedWorld = value; } }

}