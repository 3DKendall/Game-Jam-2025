using UnityEngine;

public class GatherableObject : MonoBehaviour
{
    public enum DeathType { Destroy, EnablePhysics }
    public DeathType deathType;

    public GatherDataSO[] gatherDatas;
    public int hits;
    public ItemSO[] prefferedTools;
    public int toolMultiplier = 2;

    bool hasDied;

    private void Update()
    {
        if (hits <= 0 && !hasDied)
        {
            if (deathType == DeathType.Destroy)
                Destroy(gameObject);
            else if (deathType == DeathType.EnablePhysics)
            {
                if (GetComponent<Rigidbody>() != null)
                {
                    GetComponent<Rigidbody>().isKinematic = false;
                    GetComponent<Rigidbody>().useGravity = true;

                    //This adds rotatation
                    //GetComponent<Rigidbody>().AddTorque(Vector3.right * 20);

                    //Destroy object in 15 seconds
                    Destroy(gameObject, 10f);
                }
                else
                    Destroy(gameObject);
            }

            hasDied = true;
        }
    }

    public void Gather(ItemSO toolUsed, InventoryManager inventory)
    {
        if (hits <= 0)
            return;


        bool usingRightTool = false;

        //Check for tool
        if (prefferedTools.Length > 0)
        {
            for (int i = 0; i < prefferedTools.Length; i++)
            {
                if (prefferedTools[i] == toolUsed)
                {
                    usingRightTool = true;
                    break;
                }
            }
        }

        //Gather

        int selectedGatherData = Random.Range(0, gatherDatas.Length);

        inventory.AddItem(gatherDatas[selectedGatherData].item, gatherDatas[selectedGatherData].amount);

        hits--;
    }
}
