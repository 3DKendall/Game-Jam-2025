using UnityEngine;

[CreateAssetMenu (fileName = "New Gather Data", menuName = "Survivors Edge/ Gather Data")]
public class GatherDataSO : ScriptableObject
{
    public ItemSO item;
    public int amount;
}
