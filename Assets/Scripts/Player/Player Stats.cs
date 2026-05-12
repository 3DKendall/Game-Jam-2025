using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public bool isDead;

    [Header("Stats")]
    public float health;
    public float maxHealth;
    [Space]
    public float hunger;
    public float maxHunger;
    [Space]
    public float thirst;
    public float maxThirst;

    [Header("Stats Depletion")]
    public float hungerDepletion = 0.111f; // Depletes hunger in ~15 minutes
    public float thirstDepletion = 0.167f; // Depletes thirst in ~10 minutes

    [Header("Stat Damages")]
    public float hungerDamage = 1.5f;
    public float thirstDamage = 2.25f;

    [Header("UI References")]
    public Slider healthSlider;
    public Slider hungerSlider;
    public Slider thirstSlider;
    
    private void Start()
    {
        health = maxHealth;
        hunger = maxHunger;
        thirst = maxThirst;
    }

    private void Update()
    {
        if (isDead)
            return;

        UpdateStats();
        UpdateUI();
    }

    public void UpdateUI()
    {
        healthSlider.value = health;
       hungerSlider.value = hunger;
       thirstSlider.value = thirst;
    }

    public void UpdateStats()
    {
        if (health <= 0)
            health = 0;
        if (health >= maxHealth)
            health = maxHealth;

        if (hunger <= 0)
            hunger = 0;
        if (hunger >= maxHunger)
            hunger = maxHunger;

        if (thirst <= 0)
            thirst = 0;
        if (thirst >= maxThirst)
            thirst = maxThirst;

        // DAMAGES
        if (hunger <= 0)
            health -= hungerDamage * Time.deltaTime;
        if (thirst <= 0)
            health -= thirstDamage * Time.deltaTime;

        // DEPLETIONS
        if (hunger > 0)
            hunger -= hungerDepletion * Time.deltaTime;
        if (thirst > 0)
            thirst -= thirstDepletion * Time.deltaTime;
    }
}
