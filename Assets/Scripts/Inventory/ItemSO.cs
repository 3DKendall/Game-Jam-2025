using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Survivors Edge/Inventory/New Item")]
public class ItemSO : ScriptableObject
{
    public enum ItemType { Generic, Consumable, Weapon, MeleeWeapon, Buildable }

    [Header("General")]
    public int ID;
    public ItemType itemType;
    public Sprite icon;
    public string itemName = "Item Name";
    public string description = "New Item Description";
    [Space]
    public bool isStackable;
    public int maxStack = 1;

    [Header("Weapon")]
    public float damage = 20f;
    public float range = 200f;
    [Space]
    public int magSize = 30;
    public ItemSO bulletData;
    public float fireRate = 0.1f;
    [Space]
    public float zoomFOV = 60f;
    [Space]
    public float horizontalRecoil;
    public float minVerticalRecoil;
    public float maxVerticalRecoil;
    [Space]
    [Space]
    public float hipSpread = 0.04f;
    public float aimSpread = 0;
    [Space]
    public bool shotgunFire;
    public int pelletsPerShot = 8;
    [Space]
    [Space]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip takoutsound;
    public AudioClip emptySound;

    [Header("Consumable")]
    public float healthChange = 10f;
    public float hungerChange = 10f;
    public float thirstChange = 10f;

    [Header("Buildable")]
    public BuildGhost ghost;

    [Header("FUEL / SMELTING")]
    public bool isFuel;
    public ItemSO outcome;
    [Range(1, 100)]
    public int outcomeAmount = 1;
    public float processTime = 3f;
}
