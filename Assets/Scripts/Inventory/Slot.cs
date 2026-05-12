using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Processors;

public class Slot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private DragDropHandler dragDropHandler;
    private InventoryManager inventory;
    public Weapon weaponEquipped;


    public ItemSO data;
    public int stackSize;
    [Space]
    public Image icon;
    public Text stackText;

    private bool isEmpty;
    public bool IsEmpty => isEmpty;

    private void Start()
    {
        dragDropHandler = GetComponentInParent<Player>().GetComponentInChildren<DragDropHandler>();
        inventory = GetComponentInParent<Player>().GetComponentInChildren<InventoryManager>();

        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (data != null)
        {
            if (data.itemType != ItemSO.ItemType.Weapon)
            {
                if (stackSize <= 0)
                {
                    data = null;
                }
            }
        }

        //Show slot prefab
        if (data == null)
        {
            isEmpty = true;

            icon.gameObject.SetActive(false);
            stackText.gameObject.SetActive(false);
        }
        //Do not show slot prefab
        else
        {
            isEmpty = false;

            icon.sprite = data.icon;
            stackText.text = $"x{stackSize}";

            icon.gameObject.SetActive(true);
            stackText.gameObject.SetActive(true);
        }
    }

    public void AddItemToSlot(ItemSO data_, int stackSize_)
    {
        data = data_;
        stackSize = stackSize_;
    }

    public void AddStackAmount(int stackSize_)
    {
        stackSize += stackSize_;
    }

    public void Drop()
    {
        GetComponentInParent<Player>().GetComponentInChildren<InventoryManager>().DropItem(this);
    }

    public void Clean()
    {
        data = null;
        stackSize = 0;

        UpdateSlot();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!dragDropHandler.isDragging)
        {
            if (eventData.button == PointerEventData.InputButton.Left && !isEmpty)
            {
                dragDropHandler.slotDraggedFrom = this;
                dragDropHandler.isDragging = true;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (dragDropHandler.isDragging)
        {
            //DROP
            if (dragDropHandler.slotDraggedTo == null)
            {
                dragDropHandler.slotDraggedFrom.Drop();
                dragDropHandler.isDragging = false;
            }
            //DRAG AND DROP
            else if (dragDropHandler.slotDraggedTo != null)
            {
                inventory.DragDrop(dragDropHandler.slotDraggedFrom, dragDropHandler.slotDraggedTo);
                dragDropHandler.isDragging = false;
            }
        }
    }

    public void Try_Use()
    {
        if (data == null)
            return;

        if (data.itemType == ItemSO.ItemType.Weapon || data.itemType == ItemSO.ItemType.MeleeWeapon)
        {
            bool shouldJustUnequip = false;

            //UNQUEIP ALL ACTIVE WEAPONS
            for (int i = 0; i < inventory.weapons.Length; i++)
            {
                if (inventory.weapons[i].gameObject.activeSelf)
                {
                    if (inventory.weapons[i].slotEquippedOn == this)
                        shouldJustUnequip = true;

                    inventory.weapons[i].UnEquip();
                }
            }

            if (shouldJustUnequip)
                return;

            //EQUIP WEAPONS
            for (int i = 0; i < inventory.weapons.Length; i++)
            {
                if (inventory.weapons[i].weaponData == data)
                {
                    inventory.weapons[i].Equip(this);
                }
            }

        }

        if (data.itemType == ItemSO.ItemType.Consumable)
            Consume();

        if (data.itemType == ItemSO.ItemType.Buildable)
            Try_Build();
    }

    public void Try_Build()
    {
        //UNQUEIP ALL ACTIVE WEAPONS WHEN PLAYER IS BUILDING
        for (int i = 0; i < inventory.weapons.Length; i++)
        {
            if (inventory.weapons[i].gameObject.activeSelf)
            {
                inventory.weapons[i].UnEquip();
            }
        }

        if (inventory.building.slotInUse == null)
        {
            inventory.building.slotInUse = this;
        }
        else
        {
            if (inventory.building.slotInUse == this)
            {
                inventory.building.slotInUse = null;
            }
            else
            {
                inventory.building.slotInUse = this;

                Destroy(inventory.building.ghost.gameObject);
            }
        }

    }

    public void Consume()
    {
        PlayerStats stats = GetComponentInParent<PlayerStats>();

        stats.health += data.healthChange;
        stats.hunger += data.hungerChange;
        stats.thirst += data.thirstChange;

        stackSize--;

        UpdateSlot();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (dragDropHandler.isDragging)
        {
            dragDropHandler.slotDraggedTo = this;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        dragDropHandler.slotDraggedTo = null;
    }
}

