using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Player player;
    private AudioSource audioSource;    


    [HideInInspector] public Animator anim;
    public Slot slotEquippedOn;
    public GameObject bulletHolePrefab;
    public ItemSO weaponData;
    public bool isAutomatic;
    public ParticleSystem muzzleFlash;
    [Space]
    public Transform shootPoint;
    public LayerMask shootableLayers;

    [Header("Aiming")]
    public float aimSpeed = 10;
    public Vector3 hipPos;
    public Vector3 aimPos;
    public bool isAiming;


    [HideInInspector] public bool isReloading;
    [HideInInspector] public bool hasTakenOut;

    private float currentFireRate;
    private float fireRate;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        transform.localPosition = hipPos;

        fireRate = weaponData.fireRate;

        currentFireRate = weaponData.fireRate;
    }

    private void Update()
    {
        if(slotEquippedOn.data == null)
            UnEquip(); 

        UpdateAnimations();

        if (weaponData.itemType == ItemSO.ItemType.Weapon)
        {
            if (currentFireRate < fireRate)
                currentFireRate += Time.deltaTime;

            //Call the reload function
            if (Input.GetKeyDown(KeyCode.R) && !player.windowHandler.inventory.opened)
                Start_Reload();

            UpdateAiming();

            if (player.windowHandler.inventory.opened)
                return;

            if (isAutomatic)
            {
                if (Input.GetButton("Fire1"))
                {
                    if (!weaponData.shotgunFire)
                        Shoot();
                    else
                        ShotgunShoot();
                }
            }

            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if(!weaponData.shotgunFire)
                    Shoot();
                    else
                        ShotgunShoot();
                }
            }
        }
        else if (weaponData.itemType == ItemSO.ItemType.MeleeWeapon)
        {
            if (currentFireRate < fireRate)
                currentFireRate += Time.deltaTime;

            if (Input.GetButton("Fire1"))
                Swing();
        }
    }

    #region Fire Weapon Functions

    public void Shoot()
    {
        if (currentFireRate < fireRate || isReloading || !hasTakenOut || player.running || slotEquippedOn.stackSize <= 0 || player.windowHandler.inventory.opened)
            return;

        GetComponentInParent<Animator>().SetTrigger("Shake");

        RaycastHit hit;

        Vector3 shootDir = shootPoint.forward;

        if (isAiming)
        {
            shootDir.x += Random.Range(-weaponData.aimSpread, weaponData.aimSpread);
            shootDir.y += Random.Range(-weaponData.aimSpread, weaponData.aimSpread);
        }

        else
        {
            shootDir.x += Random.Range(-weaponData.hipSpread, weaponData.hipSpread);
            shootDir.y += Random.Range(-weaponData.hipSpread, weaponData.hipSpread);
        }

        if (Physics.Raycast(shootPoint.position, shootDir, out hit, weaponData.range, shootableLayers))
        {
            GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

            //Utalize this code when i want to add a rabbit into my game!!!!!!!
            RabbitAI AI_Rabbit = hit.transform.GetComponent<RabbitAI>();

            if(AI_Rabbit != null)
            {
                  AI_Rabbit.health -= weaponData.damage;
            }

            EnemyAI zombie = hit.transform.GetComponent<EnemyAI>();

            if(zombie != null)
            {
                zombie.enemyHealth -= weaponData.damage;
            }
        }

        if(muzzleFlash != null)
        {
            muzzleFlash.Stop();
            muzzleFlash.startRotation = Random.Range(0, 360);
            muzzleFlash.Play();
        }

        else
        {
            Debug.LogWarning("Muzzle is not assigned");
        }


        anim.CrossFadeInFixedTime("Shooting", 0.015f);

        GetComponentInParent<CameraLook>().RecoilCamera(Random.Range(-weaponData.horizontalRecoil, weaponData.horizontalRecoil), Random.Range(weaponData.minVerticalRecoil, weaponData.maxVerticalRecoil));

        audioSource.PlayOneShot(weaponData.shootSound);

        currentFireRate = 0;

        //Remove ammo
        slotEquippedOn.stackSize--;
        slotEquippedOn.UpdateSlot();
    }

    public void ShotgunShoot()
    {
        if (currentFireRate < fireRate || isReloading || !hasTakenOut || player.running || slotEquippedOn.stackSize <= 0 || player.windowHandler.inventory.opened)
            return;

        GetComponentInParent<Animator>().SetTrigger("Shake");

        for (int i = 0; i < weaponData.pelletsPerShot; i++)
        {

            RaycastHit hit;

            Vector3 shootDir = shootPoint.forward;

            if (isAiming)
            {
                shootDir.x += Random.Range(-weaponData.aimSpread, weaponData.aimSpread);
                shootDir.y += Random.Range(-weaponData.aimSpread, weaponData.aimSpread);
            }

            else
            {
                shootDir.x += Random.Range(-weaponData.hipSpread, weaponData.hipSpread);
                shootDir.y += Random.Range(-weaponData.hipSpread, weaponData.hipSpread);
            }

            if (Physics.Raycast(shootPoint.position, shootDir, out hit, weaponData.range, shootableLayers))
            {
                GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

                //Utalize this code when i want to add a rabbit into my game!!!!!!!
                RabbitAI AI_Rabbit = hit.transform.GetComponent<RabbitAI>();

                if(AI_Rabbit != null)
                {
                      AI_Rabbit.health -= weaponData.damage;
                }
            }

            if (muzzleFlash != null)
            {
                muzzleFlash.Stop();
                muzzleFlash.startRotation = Random.Range(0, 360);
                muzzleFlash.Play();
            }

            else
            {
                Debug.LogWarning("Muzzle is not assigned");
            }
        }

        anim.CrossFadeInFixedTime("Shooting", 0.015f);

        GetComponentInParent<CameraLook>().RecoilCamera(Random.Range(-weaponData.horizontalRecoil, weaponData.horizontalRecoil), Random.Range(weaponData.minVerticalRecoil, weaponData.maxVerticalRecoil));

        audioSource.PlayOneShot(weaponData.shootSound);

        currentFireRate = 0;

        //Remove ammo
        slotEquippedOn.stackSize--;
        slotEquippedOn.UpdateSlot();
    }

    public void Start_Reload()
    {
        if (isReloading || slotEquippedOn.stackSize >= weaponData.magSize || player.running || !hasTakenOut || !CheckForBullets(weaponData.bulletData, weaponData.magSize))
            return;

        audioSource.PlayOneShot(weaponData.reloadSound);

        anim.CrossFadeInFixedTime("Reloading", 0.015f);

        isReloading = true;
    }

    public void Finish_Reload()
    {
        isReloading = false;

        TakeBullets(weaponData.bulletData, weaponData.magSize);
    }


    private bool CheckForBullets(ItemSO bulletData, int magSize)
    {
        InventoryManager inventory = GetComponentInParent<Player>().GetComponentInChildren<InventoryManager>();

        int amountfound = 0;

        for (int b = 0; b < inventory.inventorySlots.Length; b++)
        {
            if (!inventory.inventorySlots[b].IsEmpty)
            {
                if (inventory.inventorySlots[b].data == bulletData)
                {
                    amountfound += inventory.inventorySlots[b].stackSize;
                }
            }
        }


        if (amountfound < 1)
            return false;

        return true;
    }

    public void TakeBullets(ItemSO bulletData, int magSize)
    {
        InventoryManager inventory = GetComponentInParent<Player>().GetComponentInChildren<InventoryManager>();

        int ammoToReload = weaponData.magSize - slotEquippedOn.stackSize;
        int ammoInTheInventory = 0;

        // CHECK FOR THE BULLETS
        for (int i = 0; i < inventory.inventorySlots.Length; i++)
        {
            if (!inventory.inventorySlots[i].IsEmpty)
            {
                if (inventory.inventorySlots[i].data == bulletData)
                {
                    ammoInTheInventory += inventory.inventorySlots[i].stackSize;
                }
            }
        }


        int ammoToTakeFromInventory = (ammoInTheInventory >= ammoToReload) ? ammoToReload : ammoInTheInventory;


        // TAKE THE BULLETS FROM THE INVENTORY
        for (int i = 0; i < inventory.inventorySlots.Length; i++)
        {
            if (!inventory.inventorySlots[i].IsEmpty && ammoToTakeFromInventory > 0)
            {
                if (inventory.inventorySlots[i].data == bulletData)
                {
                    if (inventory.inventorySlots[i].stackSize <= ammoToTakeFromInventory)
                    {
                        slotEquippedOn.stackSize += inventory.inventorySlots[i].stackSize;
                        ammoToTakeFromInventory -= inventory.inventorySlots[i].stackSize;
                        inventory.inventorySlots[i].Clean();
                    }
                    else if (inventory.inventorySlots[i].stackSize > ammoToTakeFromInventory)
                    {
                        slotEquippedOn.stackSize = weaponData.magSize;
                        inventory.inventorySlots[i].stackSize -= ammoToTakeFromInventory;
                        ammoToTakeFromInventory = 0;
                        inventory.inventorySlots[i].UpdateSlot();
                    }
                }
            }
        }


        slotEquippedOn.UpdateSlot();

    }


    public void UpdateAiming()
    {
        if (Input.GetButton("Fire2") && !player.running && !isReloading && !player.windowHandler.inventory.opened)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, aimPos, aimSpeed * Time.deltaTime);
            isAiming = true;
        }
        else
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, hipPos, aimSpeed * Time.deltaTime);
            isAiming = false;
        }
    }

    #endregion

    #region Melee Functions
    public void Swing()
    {
        Debug.Log("swing");

        if (currentFireRate < fireRate || isReloading || !hasTakenOut || player.running || slotEquippedOn.stackSize <= 0 || player.windowHandler.inventory.opened)
            return;

        anim.SetTrigger("Swing");

        currentFireRate =  0;
    }

    public void CheckForHit()
    {
        RaycastHit hit;

        if (Physics.SphereCast(shootPoint.position, 0.2f, shootPoint.forward, out hit, weaponData.range, shootableLayers))
        {
            Hit();
        }
        else
            Miss();

    }

    public void Miss()
    {
        anim.SetTrigger("Miss");
    }

    public void Hit()
    {
        anim.SetTrigger("Hit");
    }

    public void ExecuteHit()
    {
        GetComponentInParent<Animator>().SetTrigger("Shake");


        RaycastHit hit;

        if (Physics.SphereCast(shootPoint.position, 0.2f, shootPoint.forward, out hit, weaponData.range, shootableLayers))
        {
            GatherableObject gatherObj = hit.transform.GetComponent<GatherableObject>();
            GatherExtension gatherExten = hit.transform.GetComponentInParent<GatherExtension>();



            if (gatherObj != null)
                gatherObj.Gather(weaponData, GetComponentInParent<WindowHandler>().inventory);

            if (gatherExten != null)
                gatherExten.Gather(weaponData, GetComponentInParent<WindowHandler>().inventory);

            //Utalize this code when i want to add a rabbit into my game!!!!!!!
            RabbitAI AI_Rabbit = hit.transform.GetComponent<RabbitAI>();

            if(AI_Rabbit != null)
            {
                  AI_Rabbit.health -= weaponData.damage;

                Debug.Log("Shot");
            }
        }
    }

    #endregion

    public void UpdateAnimations()
    {
        anim.SetBool("Running", player.running);
    }

    public void Equip(Slot slot)
    {
        gameObject.SetActive(true);

        GetComponentInParent<Player>().GetComponentInChildren<Camera_FOV_Handler>().weapon = this;

        slotEquippedOn = slot;
        slotEquippedOn.weaponEquipped = this;


        transform.localPosition = hipPos;
    }

    public void UnEquip()
    {
        GetComponentInParent<Player>().GetComponentInChildren<Camera_FOV_Handler>().weapon = null;

        slotEquippedOn.weaponEquipped = null;
        slotEquippedOn = null;

        isReloading = false;
        hasTakenOut = false;

        gameObject.SetActive(false);
    }
}