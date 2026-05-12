using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private InventoryManager inventory;
    public RecipieTemplate recipieTemplate;
    public CraftingRecipieSO[] recipies;
    public Transform contentHolder;

    // PRIVATE VARIABLES
    public RecipieTemplate recipieInCraft;
    private bool isCrafting;
    public float currentTimer;
    private float timer;


    public bool opened;
    private void Start()
    {
        inventory = GetComponentInParent<InventoryManager>();
        GenerateRecipies();
    }

    private void Update()
    {
        if (isCrafting)
        {
            if (currentTimer < timer)
            {
                recipieInCraft.timerText.text = currentTimer.ToString("f2");
            }
            else
            {
                recipieInCraft.timerText.text = currentTimer.ToString("f2");
                inventory.AddItem(recipieInCraft.recipie.outcome, recipieInCraft.recipie.outcomeAmount);
                isCrafting = false;
            }
            currentTimer += Time.deltaTime;
        }

        if (opened)
            transform.localPosition = new Vector3(0, 0, 0);
        else
            transform.localPosition = new Vector3(-10000, 0, 0);
    }

    public void GenerateRecipies()
    {
        for (int i = 0; i < recipies.Length; i++)
        {
            RecipieTemplate recipie = Instantiate(recipieTemplate.gameObject, contentHolder).GetComponent<RecipieTemplate>();
            recipie.recipie = recipies[i];
            recipie.icon.sprite = recipies[i].icon;
            recipie.nameText.text = recipies[i].recipieName;
            recipie.timerText.text = "";

            for (int b = 0; b < recipies[i].requrirements.Length; b++)
            {
                if (b == 0)
                    recipie.requirementText.text = $"{recipies[i].requrirements[b].data.itemName}  {recipies[i].requrirements[b].amountNeeded}";
                else
                    recipie.requirementText.text = $"{recipie.requirementText.text}, {recipies[i].requrirements[b].data.itemName}  {recipies[i].requrirements[b].amountNeeded}";
            }
        }
    }

    public void Try_Craft(RecipieTemplate template)
    {
        Debug.Log("Trying to craft");
        if (!HasResources(template.recipie))
        {
            Debug.Log("Not enough resources to craft");
            return;
        }
        if (isCrafting)
        {
            Debug.Log("Already crafting");
            return;
        }

        TakeResources(template.recipie);
        recipieInCraft = template;
        isCrafting = true;
        currentTimer = 0;
        timer = template.recipie.craftingTime;
    }

    public void Cancel(RecipieTemplate template)
    {
        if (!isCrafting)
            return;

        for (int i = 0; i < template.recipie.requrirements.Length; i++)
        {
            inventory.AddItem(template.recipie.requrirements[i].data, template.recipie.requrirements[i].amountNeeded);
            Debug.Log("Canceled crafting");
        }
    }

    public bool HasResources(CraftingRecipieSO recipie)
    {
        bool canCraft = true;
        List<int> stackNeededList = new List<int>();

        for (int i = 0; i < recipie.requrirements.Length; i++)
        {
            Debug.Log("Got Stacks needed");
            stackNeededList.Add(recipie.requrirements[i].amountNeeded);
        }

        int[] stackNeeded = stackNeededList.ToArray();
        int[] stacksAvailable = new int[stackNeeded.Length];

        for (int b = 0; b < recipie.requrirements.Length; b++)
        {
            for (int i = 0; i < inventory.inventorySlots.Length; i++)
            {
                if (inventory.inventorySlots[i].data == recipie.requrirements[b].data)
                {
                    stacksAvailable[b] += inventory.inventorySlots[i].stackSize;
                }
            }
        }

        for (int i = 0; i < stackNeeded.Length; i++)
        {
            if (stacksAvailable[i] < stackNeeded[i])
            {
                canCraft = false;
                break;
            }
        }

        Debug.Log(canCraft ? "Can craft" : "Cannot craft");
        return canCraft;
    }

    public void TakeResources(CraftingRecipieSO recipie)
    {
        List<int> stacksNeededList = new List<int>();

        for (int i = 0; i < recipie.requrirements.Length; i++)
        {
            stacksNeededList.Add(recipie.requrirements[i].amountNeeded);
            Debug.Log($"Requirement {i}: {recipie.requrirements[i].data.itemName}, Amount Needed: {recipie.requrirements[i].amountNeeded}");
        }

        int[] stacksNeeded = stacksNeededList.ToArray();

        for (int i = 0; i < recipie.requrirements.Length; i++)
        {
            Debug.Log("TAKE ITEMS");
            for (int b = 0; b < inventory.inventorySlots.Length; b++)
            {
                if (inventory.inventorySlots[b].IsEmpty)
                    continue;

                if (inventory.inventorySlots[b].data == recipie.requrirements[i].data)
                {
                    if (stacksNeeded[i] > 0)
                    {
                        Debug.Log($"Taking from slot {b}: {inventory.inventorySlots[b].data.itemName}, Stack Size: {inventory.inventorySlots[b].stackSize}");
                        if (stacksNeeded[i] <= inventory.inventorySlots[b].stackSize)
                        {
                            inventory.inventorySlots[b].stackSize -= stacksNeeded[i];
                            stacksNeeded[i] = 0;
                        }
                        else
                        {
                            stacksNeeded[i] -= inventory.inventorySlots[b].stackSize;
                            inventory.inventorySlots[b].Clean();
                        }

                        inventory.inventorySlots[b].UpdateSlot();
                    }
                }
            }
        }
    }
}