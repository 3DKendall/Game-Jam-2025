using UnityEngine;

public class BuildGhost : MonoBehaviour
{
    public GameObject buildPrefab;
    public bool canBuild;

    private void Start()
    {
        canBuild = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if(!other.isTrigger)
            canBuild = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.isTrigger)
            canBuild = true;
    }
}
