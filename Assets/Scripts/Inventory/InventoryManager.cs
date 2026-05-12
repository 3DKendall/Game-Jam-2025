using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Weapon[] weapons;
    [HideInInspector] public BuildingHandler building;
    public bool opened;
    public KeyCode inventoryKey = KeyCode.Tab;

    [Header("Settings")]
    public int inventorySize = 24;
    public int hotbarSize = 6;

    [Header("Refs")]
    public GameObject dropModel;
    public Transform dropPos;
    public GameObject slotTemplate;
    public Transform contentHolder;
    public Transform hotbarContentHolder;

    [HideInInspector] public Slot[] inventorySlots;
    private Slot[] hotbarSlots;

    private void Start()
    {
        building = GetComponentInParent<WindowHandler>().building;

        GenerateSlots();
        GenerateHotBarSlots();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            hotbarSlots[0].Try_Use();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            hotbarSlots[1].Try_Use();
        if (Input.GetKeyDown(KeyCode.Alpha3))
            hotbarSlots[2].Try_Use();
        if (Input.GetKeyDown(KeyCode.Alpha4))
            hotbarSlots[3].Try_Use();
        if (Input.GetKeyDown(KeyCode.Alpha5))
            hotbarSlots[4].Try_Use();
        if (Input.GetKeyDown(KeyCode.Alpha6))
            hotbarSlots[5].Try_Use();



        //Open the inventory on a key
        if (Input.GetKeyDown(inventoryKey))
            opened = !opened;

        if (GetComponentInParent<WindowHandler>().storage.opened)
        {
            GetComponentInParent<WindowHandler>().crafting.opened = false;
        }
        else
            GetComponentInParent<WindowHandler>().crafting.opened = true;


        //Move inventory UI in center of screen
        if (opened)
        {
            transform.localPosition = new Vector3(0, 0, 0);
        }
        //Move it out of the way
        else
        {
            transform.localPosition = new Vector3(-10000, 0, 0);

            if (GetComponentInParent<WindowHandler>().storage.opened)
            {
                GetComponentInParent<WindowHandler>().storage.Close();
            }
        }
    }

    private void GenerateSlots()
    {
        List<Slot> inventorySlots_ = new List<Slot>();

        //GENERATE SLOTS
        for (int i = 0; i < inventorySize; i++)
        {
            Slot slot = Instantiate(slotTemplate.gameObject, contentHolder).GetComponent<Slot>();

            inventorySlots_.Add(slot);
        }

        inventorySlots = inventorySlots_.ToArray();
    }

    private void GenerateHotBarSlots()
    {
        List<Slot> inventorySlots_ = new List<Slot>();
        List<Slot> hotbarList_ = new List<Slot>();

        //UPDATE HOTAR SLOTS ON THE LIST
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots_.Add(inventorySlots[i]);
        }

        //GENERATE SLOTS
        for (int i = 0; i < hotbarSize; i++)
        {
            Slot slot = Instantiate(slotTemplate.gameObject, hotbarContentHolder).GetComponent<Slot>();

            inventorySlots_.Add(slot);
            hotbarList_.Add(slot);
        }

        inventorySlots = inventorySlots_.ToArray();
        hotbarSlots = hotbarList_.ToArray();
    }


    public void DragDrop(Slot from, Slot to)
    {
        //UNQEUIP WEAPONS FROM SLOTS
        if (from.weaponEquipped != null)
        {
            from.weaponEquipped.UnEquip();
        }
        if (to.weaponEquipped != null)
            to.weaponEquipped.UnEquip();


        //STOP BUILDING
        if (from == building.slotInUse)
            building.slotInUse = null;

        if (to == building.slotInUse)
            building.slotInUse = null;


        //SWAPPING
        if (from.data != to.data)
        {
            ItemSO data = to.data;
            int stackSize = to.stackSize;

            to.data = from.data;
            to.stackSize = from.stackSize;

            from.data = data;
            from.stackSize = stackSize;
        }

        //STACKING
        else
        {
            if (from.data.isStackable)
            {
                if (from.stackSize + to.stackSize > from.data.maxStack)
                {
                    int amountLeft = (from.stackSize + to.stackSize) - from.data.maxStack;

                    from.stackSize = amountLeft;

                    to.stackSize = to.data.maxStack;
                }
                else
                {
                    to.stackSize += from.stackSize;

                    from.Clean();

                    from.UpdateSlot();
                    to.UpdateSlot();
                }
            }
            else
            {
                ItemSO data = to.data;
                int stackSize = to.stackSize;

                to.data = from.data;
                to.stackSize = from.stackSize;

                from.data = data;
                from.stackSize = stackSize;
            }

        }

        from.UpdateSlot();
        to.UpdateSlot();
    }

    public void AddItem(Pickup pickUp)
    {
        if (pickUp.data.isStackable)
        {
            Slot stackableSlot = null;

            // TRY FINDING STACKABLE SLOT
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (!inventorySlots[i].IsEmpty)
                {
                    if (inventorySlots[i].data == pickUp.data && inventorySlots[i].stackSize < pickUp.data.maxStack)
                    {
                        stackableSlot = inventorySlots[i];
                        break;
                    }

                }
            }

            if (stackableSlot != null)
            {

                // IF IT CANNOT FIT THE PICKED UP AMOUNT
                if (stackableSlot.stackSize + pickUp.stackSize > pickUp.data.maxStack)
                {
                    int amountLeft = (stackableSlot.stackSize + pickUp.stackSize) - pickUp.data.maxStack;



                    // ADD IT TO THE STACKABLE SLOT
                    stackableSlot.AddItemToSlot(pickUp.data, pickUp.data.maxStack);

                    // TRY FIND A NEW EMPTY STACK
                    for (int i = 0; i < inventorySlots.Length; i++)
                    {
                        if (inventorySlots[i].IsEmpty)
                        {
                            inventorySlots[i].AddItemToSlot(pickUp.data, amountLeft);
                            inventorySlots[i].UpdateSlot();

                            break;
                        }
                    }



                    Destroy(pickUp.gameObject);
                }
                // IF IT CAN FIT THE PICKED UP AMOUNT
                else
                {
                    stackableSlot.AddStackAmount(pickUp.stackSize);

                    Destroy(pickUp.gameObject);
                }

                stackableSlot.UpdateSlot();
            }
            else
            {
                Slot emptySlot = null;


                // FIND EMPTY SLOT
                for (int i = 0; i < inventorySlots.Length; i++)
                {
                    if (inventorySlots[i].IsEmpty)
                    {
                        emptySlot = inventorySlots[i];
                        break;
                    }
                }

                // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
                if (emptySlot != null)
                {
                    emptySlot.AddItemToSlot(pickUp.data, pickUp.stackSize);
                    emptySlot.UpdateSlot();

                    Destroy(pickUp.gameObject);
                }
                else
                {
                    pickUp.transform.position = dropPos.position;
                }
            }

        }
        else
        {
            Slot emptySlot = null;


            // FIND EMPTY SLOT
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].IsEmpty)
                {
                    emptySlot = inventorySlots[i];
                    break;
                }
            }

            // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
            if (emptySlot != null)
            {
                emptySlot.AddItemToSlot(pickUp.data, pickUp.stackSize);
                emptySlot.UpdateSlot();

                Destroy(pickUp.gameObject);
            }
            else
            {
                pickUp.transform.position = dropPos.position;
            }

        }
    }

    public void AddItem(ItemSO data, int stackSize)
    {
        if (data.isStackable)
        {
            Slot stackableSlot = null;

            // TRY FINDING STACKABLE SLOT
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (!inventorySlots[i].IsEmpty)
                {
                    if (inventorySlots[i].data == data && inventorySlots[i].stackSize < data.maxStack)
                    {
                        stackableSlot = inventorySlots[i];
                        break;
                    }

                }
            }

            if (stackableSlot != null)
            {

                // IF IT CANNOT FIT THE PICKED UP AMOUNT
                if (stackableSlot.stackSize + stackSize > data.maxStack)
                {
                    int amountLeft = (stackableSlot.stackSize + stackSize) - data.maxStack;



                    // ADD IT TO THE STACKABLE SLOT
                    stackableSlot.AddItemToSlot(data, data.maxStack);

                    // TRY FIND A NEW EMPTY STACK
                    for (int i = 0; i < inventorySlots.Length; i++)
                    {
                        if (inventorySlots[i].IsEmpty)
                        {
                            inventorySlots[i].AddItemToSlot(data, amountLeft);
                            inventorySlots[i].UpdateSlot();

                            break;
                        }
                    }
                }
                // IF IT CAN FIT THE PICKED UP AMOUNT
                else
                {
                    stackableSlot.AddStackAmount(stackSize);
                }

                stackableSlot.UpdateSlot();
            }
            else
            {
                Slot emptySlot = null;


                // FIND EMPTY SLOT
                for (int i = 0; i < inventorySlots.Length; i++)
                {
                    if (inventorySlots[i].IsEmpty)
                    {
                        emptySlot = inventorySlots[i];
                        break;
                    }
                }

                // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
                if (emptySlot != null)
                {
                    emptySlot.AddItemToSlot(data, stackSize);
                    emptySlot.UpdateSlot();
                }
                else
                {
                    DropItem(data, stackSize);
                }
            }

        }
        else
        {
            Slot emptySlot = null;


            // FIND EMPTY SLOT
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].IsEmpty)
                {
                    emptySlot = inventorySlots[i];
                    break;
                }
            }

            // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
            if (emptySlot != null)
            {
                emptySlot.AddItemToSlot(data, stackSize);
                emptySlot.UpdateSlot();
            }
            else
            {
                DropItem(data, stackSize);
            }
        }
    }

    public void DropItem(Slot slot)
    {
        Pickup pickup = Instantiate(dropModel, dropPos).AddComponent<Pickup>();
        pickup.transform.position = dropPos.position;
        pickup.transform.SetParent(null);

        pickup.data = slot.data;
        pickup.stackSize = slot.stackSize;

        slot.Clean();
    }

    public void DropItem(ItemSO data, int stackSize)
    {
        Pickup pickup = Instantiate(dropModel, dropPos).GetComponent<Pickup>();
        pickup.transform.position = dropPos.position;
        pickup.transform.SetParent(null);

        pickup.data = data;
        pickup.stackSize = stackSize;
    }
}