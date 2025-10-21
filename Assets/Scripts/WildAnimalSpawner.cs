using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WildAnimalSpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int maxPerType = 2;
    [SerializeField] private int totalEnemies = 8;

    [Header("Spawn Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float minSpawnDistance;
    [SerializeField] private float maxSpawnDistance = 150f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float minSpawnHeight = 25f;

    [Header("Despawn Settings")]
    [SerializeField] private float maxDistanceFromPlayer = 200f;

    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 30f;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private int[] spawnCount;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;

            if (player == null)
            {
                Debug.LogError("Player reference not found! Assign it in the Inspector or tag the player as 'Player'.");
                return;
            }
        }

        spawnCount = new int[enemyPrefabs.Length];
        SpawnEnemies();
    }

    void Update()
    {
        CheckEnemyDistance();
        CheckForDeadEnemies();
    }

    private void SpawnEnemies()
    {
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogError("No enemy prefabs assigned!");
            return;
        }

        int totalSpawned = spawnedEnemies.Count;

        while (totalSpawned < totalEnemies)
        {
            int enemyType = Random.Range(0, enemyPrefabs.Length);
            if (spawnCount[enemyType] >= maxPerType)
                continue;

            Vector3 spawnPos = GetValidGroundPosition();
            if (spawnPos == Vector3.zero) continue; // skip invalid

            GameObject newEnemy = Instantiate(enemyPrefabs[enemyType], spawnPos, Quaternion.identity);
            spawnedEnemies.Add(newEnemy);
            spawnCount[enemyType]++;

            Debug.Log($"Spawned enemy type {enemyType} at {spawnPos}");
            totalSpawned++;
        }
    }

    private Vector3 GetValidGroundPosition()
    {
        // Attempt several times to find a valid height
        for (int i = 0; i < 10; i++)
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector3 spawnPos = player.position + direction * distance;

            if (Physics.Raycast(spawnPos + Vector3.up * 100f, Vector3.down, out RaycastHit hit, 300f, groundLayer))
            {
                if (hit.point.y >= minSpawnHeight)
                {
                    return hit.point; // valid spawn
                }
            }
        }

        Debug.LogWarning("Failed to find valid spawn height above " + minSpawnHeight);
        return Vector3.zero; // invalid position
    }

    private void CheckEnemyDistance()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = spawnedEnemies[i];
            if (enemy == null) continue;

            float dist = Vector3.Distance(enemy.transform.position, player.position);
            if (dist > maxDistanceFromPlayer)
            {
                EnemyHealth health = enemy.GetComponent<EnemyHealth>();
                if (health != null && health.Health > 0)
                {
                    health.TakeDamage(health.Health);
                    Debug.Log($"{enemy.name} wandered too far and died.");
                }
            }
        }
    }

    private void CheckForDeadEnemies()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = spawnedEnemies[i];

            if (enemy == null)
            {
                spawnedEnemies.RemoveAt(i);
                StartCoroutine(RespawnEnemy(respawnDelay));
                continue;
            }

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null && health.Health <= 0)
            {
                spawnedEnemies.RemoveAt(i);
                StartCoroutine(RespawnEnemy(respawnDelay));

                int typeIndex = GetEnemyTypeIndex(enemy);
                if (typeIndex >= 0)
                    spawnCount[typeIndex] = Mathf.Max(0, spawnCount[typeIndex] - 1);
            }
        }
    }

    private IEnumerator RespawnEnemy(float delay)
    {
        yield return new WaitForSeconds(delay);

        int enemyType = Random.Range(0, enemyPrefabs.Length);
        if (spawnCount[enemyType] >= maxPerType)
        {
            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                if (spawnCount[i] < maxPerType)
                {
                    enemyType = i;
                    break;
                }
            }
        }

        Vector3 spawnPos = GetValidGroundPosition();
        if (spawnPos == Vector3.zero) yield break; // failed

        GameObject newEnemy = Instantiate(enemyPrefabs[enemyType], spawnPos, Quaternion.identity);
        spawnedEnemies.Add(newEnemy);
        spawnCount[enemyType]++;

        Debug.Log($"Respawned enemy type {enemyType} at {spawnPos}");
    }

    private int GetEnemyTypeIndex(GameObject enemy)
    {
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (enemy.name.Contains(enemyPrefabs[i].name))
                return i;
        }
        return -1;
    }
}
