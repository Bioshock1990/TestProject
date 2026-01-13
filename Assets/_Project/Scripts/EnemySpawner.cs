using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn settings")]
    public GameObject enemyPrefab;
    public Transform player;

    public float spawnInterval = 1.5f;
    public float minDistanceFromPlayer = 5f;
    public int maxAttempts = 20;

    [Header("Map size")]
    public float mapWidth = 50f;
    public float mapHeight = 50f;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool foundPosition = false;

        for (int i = 0; i < maxAttempts; i++)
        {
            float x = Random.Range(-mapWidth / 2f, mapWidth / 2f);
            float z = Random.Range(-mapHeight / 2f, mapHeight / 2f);

            spawnPosition = new Vector3(x, 0f, z);

            float distanceToPlayer = Vector3.Distance(
                new Vector3(player.position.x, 0f, player.position.z),
                spawnPosition
            );

            if (distanceToPlayer >= minDistanceFromPlayer)
            {
                foundPosition = true;
                break;
            }
        }

        if (foundPosition)
        {
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
