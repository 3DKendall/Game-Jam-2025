using System.Collections.Generic;
using UnityEngine;

public class StorageUI : MonoBehaviour
{
    [HideInInspector] public Storage storageOpened;
    public Slot slotPrefab;
    public Transform content;
    [Space]
    public bool opened;
    public Vector3 openPosition;

    [Header("Furnace UI")]
    public GameObject FurnaceUI;
    public GameObject turnOnButton;
    public GameObject turnOffButton;

    private void Update()
    {
        if (opened)
            transform.localPosition = openPosition;
        else
            transform.position = new Vector3(-10000, 0, 0);

        if (storageOpened != null)
        {
            if (storageOpened.isFurnace)
            {
                if (storageOpened.GetComponent<Furnace>().isOn)
                {
                    for (int i = 0; i < storageOpened.slots.Length; i++)
                    {
                        Slot[] slots = GetComponentsInChildren<Slot>();

                        if (storageOpened.slots[i].data == null)
                            slots[i].data = null;
                        else
                            slots[i].data = storageOpened.slots[i].data;

                        slots[i].stackSize = storageOpened.slots[i].stackSize;

                        slots[i].UpdateSlot();
                    }
                }
                else
                {
                    for (int i = 0; i < storageOpened.slots.Length; i++)
                    {
                        Slot[] slots = GetComponentsInChildren<Slot>();

                        if (slots[i].data == null)
                            storageOpened.slots[i].data = null;
                        else
                            storageOpened.slots[i].data = slots[i].data;

                        storageOpened.slots[i].stackSize = slots[i].stackSize;
                    }
                }

                FurnaceUI.SetActive(true);

                if (storageOpened.GetComponent<Furnace>().isOn)
                {
                    turnOffButton.SetActive(true);
                    turnOnButton.SetActive(false);
                }
                else
                {
                    turnOffButton.SetActive(false);
                    turnOnButton.SetActive(true);
                }
            }
            else
            {
                FurnaceUI.SetActive(false);
            }
        }
    }

    public void Open(Storage storage)
    {
        storageOpened = storage;

        for (int i = 0; i < storage.slots.Length; i++)
        {
            Slot slot = Instantiate(slotPrefab, content).GetComponent<Slot>();

            if (storage.slots[i].data == null)
                slot.data = null;
            else
                slot.data = storage.slots[i].data;

            slot.stackSize = storage.slots[i].stackSize;

            slot.UpdateSlot();
        }

        opened = true;
    }

    public void Close()
    {
        if (storageOpened == null)
            return;

        storageOpened.Close(GetComponentsInChildren<Slot>());

        Slot[] slotsToDestroy = GetComponentsInChildren<Slot>();

        for (int i = 0; i < slotsToDestroy.Length; i++)
        {
            Destroy(slotsToDestroy[i].gameObject);
        }

        storageOpened = null;

        opened = false;
    }

    public void TurnOffFurnace()
    {
        if (storageOpened == null)
            return;

        storageOpened.GetComponent<Furnace>().TurnOff();
    }

    public void TurnOnFurnace()
    {
        if (storageOpened == null)
            return;

        storageOpened.GetComponent<Furnace>().TurnOn();
    }
}
