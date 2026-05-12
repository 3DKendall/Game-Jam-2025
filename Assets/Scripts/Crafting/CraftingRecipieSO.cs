using UnityEngine;

[CreateAssetMenu (fileName = "New Requirement", menuName= "Survivors Edge/Crafting/New Recipie")]
public class CraftingRecipieSO : ScriptableObject
{
    public Sprite icon;
    public string recipieName;
    [Space]
    public CraftingRequirement[] requrirements;
    [Space]
    public float craftingTime;
    [Space]
    public ItemSO outcome;
    public int outcomeAmount = 1;
}