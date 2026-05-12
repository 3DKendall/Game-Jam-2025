using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public float respawnTime = 1200f;
    public int maxEntities = 20;
    public GameObject[] spawnableEntities;
    public List<GameObject> spawnedEntities;
    [Space]
    public float width = 10f;
    public float length = 10f;
    public float height = 10f;
    public Color color = Color.green;

    private float currentTimer;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position, new Vector3(width, height, length));
    }

    private void Start()
    {
        currentTimer = respawnTime;
    }

    private void Update()
    {
        if (currentTimer < respawnTime)
            currentTimer += Time.deltaTime;
        else
        {
            if (spawnableEntities.Length <= 0)
            {
                Debug.LogError("Spawnable Entities is not set");
                return;
            }

            int amountToRespawn = 0;
            amountToRespawn = GetAvailableEntities();

            if (amountToRespawn <= 0) return;

            for (int i = 0; i < amountToRespawn; i++)
            {
                int chosenEntity = Random.Range(0, spawnableEntities.Length);
                StartCoroutine(SpawnObject(spawnableEntities[chosenEntity]));
            }

            currentTimer = 0;
        }
    }

    public int GetAvailableEntities()
    {
        if (spawnedEntities.Count > 0)
        {
            for (int i = 0; i < spawnedEntities.Count; i++)
            {
                if (spawnedEntities[i] == null)
                    spawnedEntities.RemoveAt(i);
            }
        }
        return maxEntities - spawnedEntities.Count;
    }

    public IEnumerator SpawnObject(GameObject obj)
    {
        bool foundSpot = false;
        int attempts = 0;
        int maxAttempts = 100;

        while (!foundSpot && attempts < maxAttempts)
        {
            attempts++;
            Vector3 spawnPos = transform.position;
            spawnPos.x += Random.Range(-width, width);
            spawnPos.y = height;
            spawnPos.z += Random.Range(-length, length);

            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.transform.GetComponent<Terrain>() != null)
                {
                    // Generate a random Y-axis rotation
                    Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                    // Instantiate the object with random Y-axis rotation
                    GameObject spawnedObj = Instantiate(obj, hit.point, randomRotation);

                    // Generate random scale values
                    float randomScale = Random.Range(0.95f, 1.05f); // Adjust the range as needed
                    spawnedObj.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

                    spawnedEntities.Add(spawnedObj);

                    foundSpot = true;
                }
            }

            yield return null; // Yield for one frame to prevent freezing
        }

        if (!foundSpot)
        {
            Debug.LogWarning("Failed to find a suitable spot for spawning after max attempts.");
        }

        yield return null;
    }
}
