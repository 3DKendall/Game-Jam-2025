using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private static SaveData _singleton;

    public static SaveData Singleton
    {
        get
        {
            if (_singleton == null)
            {
                _singleton = new SaveData();
            }

            return _singleton;
        }
    }

    public List<SAVE_SavableObject> savableObjects;

    [Header("Player")]
    public float currentHealth;
    public float currentHunger;
    public float currentThirst;

    public List<int> itemIDs;
    public List<int> itemStacks;

    public float timeOfDay;


    public static bool Save()
    {
        return SerializationManager.Save("GameSave", SaveData.Singleton);
    }

    public static void Load()
    {
        _singleton = (SaveData)SerializationManager.Load(Application.dataPath + "/saves/GameSave.save");
    }

}