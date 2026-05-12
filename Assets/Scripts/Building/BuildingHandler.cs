using UnityEngine;

public class BuildingHandler : MonoBehaviour
{
    public Slot slotInUse;
    public Transform offGroundPoint;
    [Space]
    public float range = 5f;
    public Color allowed;
    public Color blocked;
    [Space]
    public BuildGhost ghost;
    public bool canBuild;

    private void Update()
    {
        UpdateBuilding();
    }

    public void UpdateColors()
    {
        MeshRenderer renderer = null;

        if (ghost.GetComponent<MeshRenderer>() != null)
            renderer = ghost.GetComponent<MeshRenderer>();

        else if (ghost.GetComponentInChildren<MeshRenderer>() != null)
            renderer = ghost.GetComponentInChildren<MeshRenderer>();

        if (renderer.materials.Length > 1)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (canBuild && ghost.canBuild)
                    renderer.materials[i].color = allowed;
                else
                    renderer.materials[i].color = blocked;
            }
        }

        else if (renderer.materials.Length == 1)
        {
            if(canBuild && ghost.canBuild)
                renderer.material.color = allowed;
            else
                renderer.material.color = blocked;
        }

    }

    public void UpdateBuilding()
    {
        if(slotInUse == null)
        {
            if(ghost != null)
            {
                Destroy(ghost.gameObject);
            }

            return;
        }

        if (slotInUse.stackSize <= 0 || slotInUse.data == null)
        {
            if (ghost != null)
            {
                Destroy(ghost.gameObject);
            }

            return;
        }

        if (ghost == null)
        {
            ghost = Instantiate(slotInUse.data.ghost, offGroundPoint.transform.position, GetComponentInParent<Player>().transform.rotation);
        }

        UpdateColors();

        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            if (hit.transform.GetComponent<BuildBlocked>() == null)
            {
                ghost.transform.position = hit.point;
                ghost.transform.rotation = GetComponentInParent<Player>().transform.rotation;
                canBuild = true;
            }
            else
            {
                ghost.transform.position = offGroundPoint.position;
                ghost.transform.rotation = GetComponentInParent<Player>().transform.rotation;
                canBuild = false;
            }
        }

        else
        {
            ghost.transform.position = offGroundPoint.position;
            ghost.transform.rotation = GetComponentInParent<Player>().transform.rotation;
            canBuild = false;
        }

        if(Input.GetButtonDown("Fire1") && canBuild && ghost.canBuild)
        {
            slotInUse.stackSize--;
            slotInUse.UpdateSlot();

            Instantiate(ghost.buildPrefab, ghost.transform.position, ghost.transform.rotation);
        }
    }
}
