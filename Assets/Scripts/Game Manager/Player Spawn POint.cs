using UnityEngine;

public class PlayerSpawnPOint : MonoBehaviour
{
    public GameObject player;
    public GameObject spawn;

    private void Awake()
    {
        player.transform.position = spawn.transform.position;
    }
}
