using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public ItemSO[] gameItems;

    public ItemSO GetItemData(int id)
    {
        for (int i = 0; i < gameItems.Length; i++)
        {
            if (gameItems[i].ID == id)
            {
                return gameItems[i];
            }
        }
        
        return null;
    }
}
