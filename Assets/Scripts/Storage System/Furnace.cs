using System.Threading;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    public Storage storage;
    [Space]
    [HideInInspector] public StorageSlot fuelSlot;
    [HideInInspector] public StorageSlot smeltingSlot;
    [Space]
    public bool isOn;
    public GameObject VFX;

    private float currentFuelTimer;
    private float fuelTimer;

    private float currentSmeltTimer;
    private float smeltTimer;

    private void Start()
    {
        if (GetComponent<Storage>() != null)
        {
            storage = GetComponent<Storage>();
        }
        else
        {
            Debug.LogError("FURNACE: Furnace does not have a storage script attached to it.");
        }
    }

    private void Update()
    {
        #region FIND SLOTS

        if (isOn)
        {
            if (fuelSlot == null)
            {
                for (int i = 0; i < storage.slots.Length; i++)
                {
                    if (storage.slots[i].data != null && storage.slots[i].data.isFuel)
                    {
                        fuelSlot = storage.slots[i];
                        currentFuelTimer = 0;
                        fuelTimer = fuelSlot.data.processTime;
                        break;
                    }
                }
            }

            if (smeltingSlot == null)
            {
                for (int i = 0; i < storage.slots.Length; i++)
                {
                    if (storage.slots[i].data != null && storage.slots[i].data.outcome != null)
                    {
                        smeltingSlot = storage.slots[i];
                        currentSmeltTimer = 0;
                        smeltTimer = smeltingSlot.data.processTime;
                        break;
                    }
                }
            }

            if (fuelSlot == null || fuelSlot.data == null)
            {
                TurnOff();
            }
        }

        #endregion

        #region SMELT

        if (isOn)
        {
            // FUEL
            if (currentFuelTimer < fuelTimer)
            {
                currentFuelTimer += Time.deltaTime;
            }
            else
            {
                currentFuelTimer = 0;
                if (fuelSlot != null) fuelSlot.stackSize--;
            }

            // SMELTING
            if (currentSmeltTimer < smeltTimer)
            {
                currentSmeltTimer += Time.deltaTime;
            }
            else
            {
                currentSmeltTimer = 0;

                if (smeltingSlot != null && smeltingSlot.data != null)
                {
                    // Add cooked / smelted object
                    storage.AddItem(smeltingSlot.data.outcome, smeltingSlot.data.outcomeAmount);
                }
                
                smeltingSlot.stackSize--;
            }
        }

        #endregion
    }

    public void TurnOn()
    {
        Debug.Log("Turned on");

        isOn = true;
        if (VFX != null) VFX.SetActive(true);
    }

    public void TurnOff()
    {
        Debug.Log("Turned Off");

        isOn = false;
        if (VFX != null) VFX.SetActive(false);

        fuelSlot = null;
        smeltingSlot = null;
    }
}
