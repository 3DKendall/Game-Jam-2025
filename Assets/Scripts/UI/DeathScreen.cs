using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    public ItemSO respawnItem;

    public void Respawn()
    {
        //Player reference
        Player player = GetComponentInParent<Player>();

        //Allow the player to spawn with a tool. (Anything i want)
        if (respawnItem != null)
            player.windowHandler.inventory.AddItem(respawnItem, 1);

        PlayerRespawnPoint[] respawnPoints = FindObjectsOfType<PlayerRespawnPoint>();

        //get a random position
        int i = Random.Range(0, respawnPoints.Length);

        //Set the new respawn values
        player.GetComponent<PlayerStats>().health = player.GetComponent<PlayerStats>().maxHealth;
        player.GetComponent<PlayerStats>().hunger = player.GetComponent<PlayerStats>().maxHunger;
        player.GetComponent<PlayerStats>().thirst = player.GetComponent<PlayerStats>().maxThirst;

        //Unlock cursor
        player.cam.lockCursor = false;
        player.cam.canMove = false;

        //Set the players spawn position according to a random point
        player.transform.position = respawnPoints[i].transform.position;


        player.GetComponent<PlayerStats>().isDead = false;
        
        gameObject.SetActive(false);

    }
}
