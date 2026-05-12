using UnityEngine;

public class WindowHandler : MonoBehaviour
{
    private CameraLook cam;
    public bool windowOpened;

    [HideInInspector] public InventoryManager inventory;
    [HideInInspector] public CraftingManager crafting;
    [HideInInspector] public StorageUI storage;
    [HideInInspector] public BuildingHandler building;
    [HideInInspector] public GameMenu gameMenu;
    public DeathScreen deathScreen;

    private void Start()
    {
        cam = GetComponentInChildren<CameraLook>();

        inventory = GetComponentInChildren<InventoryManager>();
        crafting = GetComponentInChildren<CraftingManager>();
        storage = GetComponentInChildren<StorageUI>();
        building = GetComponentInChildren<BuildingHandler>();
        gameMenu = FindObjectOfType<GameMenu>();
    }

    private void Update()
    {
        // Allow player to use cursor to navigate on UI
        if (windowOpened)
        {
            cam.lockCursor = false;
            cam.canMove = false;
        }
        else
        {
            cam.lockCursor = true;
            cam.canMove = true;
        }

        if (gameMenu != null)
        {
            if (inventory.opened || GetComponent<PlayerStats>().isDead || gameMenu.opened)
                windowOpened = true;
            else
                windowOpened = false;
        }
        else
        {
            if (inventory.opened || GetComponent<PlayerStats>().isDead)
                windowOpened = true;
            else
                windowOpened = false;
        }
    }
}
