using UnityEngine;

public class StorageSlot : MonoBehaviour
{
    [HideInInspector] public bool IsEmpty;

    public ItemSO data;
    public int stackSize;

    private void Update()
    {
        IsEmpty = data == null;
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
}
