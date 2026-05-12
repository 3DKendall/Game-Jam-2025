using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveHandler : MonoBehaviour
{
    public static bool load;

    [Header("Savable Objects")]
    [Tooltip("You must drag here every savable object that you wish to load")]
    public List<SavableObject> savableObjects;

    private void Start()
    {
        Invoke("Load", 0.3f);
    }

    public void Load()
    {
        // CHECK THAT DIRECTORY & FILE HAVE BEEN CREATED
        if (!Directory.Exists(Application.dataPath + "/saves"))
        {
            return;
        }

        if (!File.Exists(Application.dataPath + "/saves/GameSave.save"))
        {
            return;
        }


        // LOAD THE FILE TO SAVE DATA
        SaveData.Load();

        // PLAYER STATS
        FindObjectOfType<PlayerStats>().health = SaveData.Singleton.currentHealth;
        FindObjectOfType<PlayerStats>().hunger = SaveData.Singleton.currentHunger;
        FindObjectOfType<PlayerStats>().thirst = SaveData.Singleton.currentThirst;

        // DAY AND NIGHT CYCLE
        FindObjectOfType<DayNightCycle>().timeOfDay = SaveData.Singleton.timeOfDay;

        //SAVE INVENTORY
        InventoryManager inventory = FindObjectOfType<Player>().GetComponentInChildren<InventoryManager>();
        ItemDatabase database = FindObjectOfType<ItemDatabase>();

        int[] IDs = new int[0];
        int[] stacks = new int[0];

        IDs = SaveData.Singleton.itemIDs.ToArray();
        stacks = SaveData.Singleton.itemStacks.ToArray();

        for (int i = 0; i < inventory.inventorySlots.Length; i++)
        {
            if (SaveData.Singleton.itemIDs[i] == 0)
            {
                inventory.inventorySlots[i].data = null;
                inventory.inventorySlots[i].stackSize = 0;
                inventory.inventorySlots[i].UpdateSlot();
            }
            else
            {
                inventory.inventorySlots[i].data = database.GetItemData(IDs[i]);
                inventory.inventorySlots[i].stackSize = stacks[i];
                inventory.inventorySlots[i].UpdateSlot();
            }
        }


        // DESTROY INSTANTIATED SAVABLE OBJECT
        SavableObject[] objectsToDestroy = FindObjectsOfType<SavableObject>();

        for (int i = 0; i < objectsToDestroy.Length; i++)
        {
            if (objectsToDestroy[i].instantiate)
                Destroy(objectsToDestroy[i].gameObject);
        }



        // RUN THROUGH SAVABLE OBJECTS LIST AND LOAD THE DATA TO THEM
        foreach (SAVE_SavableObject save in SaveData.Singleton.savableObjects)
        {
            SavableObject obj = null;


            // FIND OBJECT IN SAVABLE OBJECTS
            for (int i = 0; i < savableObjects.Count; i++)
            {
                if (save.Id == savableObjects[i].ID)
                {
                    obj = savableObjects[i];
                    break;
                }
            }


            // IF THE OBJECT IS NOT FOUND THAN JUST RETURN
            if (obj == null)
            {
                Debug.LogError($"SAVE SYSTEM : The object to load was not found, make sure the object is in the Savable Objects list of SaveHandler");
                return;
            }



            // LOAD DATA INTO THE OBJECT
            if (obj.instantiate)
            {
                SavableObject instance = Instantiate(obj.gameObject).GetComponent<SavableObject>();

                instance.ID = save.Id;
                instance.transform.position = save.position;
                instance.transform.rotation = save.rotation;

                //UPDATE DROP BAG
                instance.itemID = save.itemID;
                instance.itemStack = save.itemStack;

                if (instance.itemID != 0)
                {
                    Pickup pickup = instance.GetComponent<Pickup>();

                    if (pickup != null)
                    {
                        pickup.data = FindAnyObjectByType<ItemDatabase>().GetItemData(instance.itemID);
                        pickup.stackSize = instance.itemStack;

                    }
                    else
                    {
                        Debug.LogError("SAVE SYSTEM: Object trying to load pickup info, but the pickup info component does not exist");
                    }
                }

                //STORAGE SYSTEM
                if (instance.GetComponent<Storage>() != null)
                {
                    Storage storage = instance.GetComponent<Storage>();

                    storage.GenerateSlots();

                    for (int i = 0; i < storage.slots.Length; i++)
                    {
                        if (save.itemsData[i] == 0)
                        {
                            storage.slots[i].data = null;
                            storage.slots[i].stackSize = 0;
                        }
                        else
                        {
                            storage.slots[i].data = FindAnyObjectByType<ItemDatabase>().GetItemData(save.itemsData[i]);
                            storage.slots[i].stackSize = save.itemsStack[i];
                        }
                    }
                }


            }
            else
            {
                obj.ID = save.Id;
                obj.transform.position = save.position;
                obj.transform.rotation = save.rotation;
            }

        }

    }

    public void Save()
    {
        // INITIALIZE THE SAVABLE OBJECT FILES LIST IN SAVE DATA
        SaveData.Singleton.savableObjects = new List<SAVE_SavableObject>();

        SavableObject[] objectsToSave = FindObjectsOfType<SavableObject>();


        foreach (SavableObject obj in objectsToSave)
        {
            // SAVE THE SAVABLE OBJECTS DATA TO THE SAVABLE OBJECT SAVE SCRIPT
            SAVE_SavableObject save = new SAVE_SavableObject();

            //TRANSFORM
            save.Id = obj.ID;
            save.position = obj.transform.position;
            save.rotation = obj.transform.rotation;

            //DROP BAG
            save.itemID = obj.itemID;
            save.itemStack = obj.itemStack;


            //STORAGE
            if (obj.GetComponent<Storage>() != null)
            {
                List<int> datas = new List<int>();
                List<int> stacks = new List<int>();

                for (int i = 0; i < obj.storageItemsID.Length; i++)
                {
                    if (obj.storageItemsID[i] == 0)
                    {
                        datas.Add(0);
                        stacks.Add(0);
                    }
                    else
                    {
                        datas.Add(obj.storageItemsID[i]);
                        stacks.Add(obj.storageItemsStack[i]);
                    }
                }

                save.itemsData = datas.ToArray();
                save.itemsStack = stacks.ToArray();
            }

            SaveData.Singleton.savableObjects.Add(save);

        }

        //PLAYER STATS
        SaveData.Singleton.currentHealth = FindObjectOfType<PlayerStats>().health;
        SaveData.Singleton.currentHunger = FindObjectOfType<PlayerStats>().hunger;
        SaveData.Singleton.currentThirst = FindObjectOfType<PlayerStats>().thirst;

        //DAY NIGHT CYCLE
        SaveData.Singleton.timeOfDay = FindObjectOfType<DayNightCycle>().timeOfDay;

        //SAVE INVENTORY
        InventoryManager inventory = FindObjectOfType<Player>().GetComponentInChildren<InventoryManager>();

        SaveData.Singleton.itemIDs = new List<int>();
        SaveData.Singleton.itemStacks = new List<int>();

        SaveData.Singleton.itemIDs.Clear();
        SaveData.Singleton.itemStacks.Clear();

        for (int i = 0; i < inventory.inventorySlots.Length; i++)
        {
            if (inventory.inventorySlots[i].data == null)
            {
                SaveData.Singleton.itemIDs.Add(0);
                SaveData.Singleton.itemStacks.Add(0);
            }
            else
            {
                SaveData.Singleton.itemIDs.Add(inventory.inventorySlots[i].data.ID);
                SaveData.Singleton.itemStacks.Add(inventory.inventorySlots[i].stackSize);
            }
        }

        //STORAGE 

        // SAVE IT TO SAVE DATA
        if (SaveData.Save())
        {
            Debug.Log("SAVE SYSTEM : Game was saved successfully.");
        }
        else
        {
            Debug.LogWarning("SAVE SYSTEM : An error occured while saving the game.");
        }

    }

    public void DeleteSaveFile()
    {
        if (!Directory.Exists(Application.dataPath + "/saves"))
        {
            return;
        }

        if (!File.Exists(Application.dataPath + "/saves/GameSave.save"))
        {
            return;
        }

        File.Delete(Application.dataPath + "/saves/GameSave.save");


    }
}
