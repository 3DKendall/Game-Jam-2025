using UnityEngine;

[CreateAssetMenu (fileName = "New Requirement", menuName = "Survivors Edge/Crafting/New Requirement")]
public class CraftingRequirement : ScriptableObject
{
    public ItemSO data;
    public int amountNeeded;
}
