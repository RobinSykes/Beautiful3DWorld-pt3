using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillagerSpawner : MonoBehaviour
{
    [Header("Villager Settings")]
    [SerializeField] private GameObject[] villagerPrefabs; // Assign your 5 villager prefabs here
    [SerializeField] private int maxPerType = 1;           // Only 1 villager per type

    [Header("Spawn Area")]
    [SerializeField] private BoxCollider villageBounds;    // Assign your village BoxCollider
    [SerializeField] private LayerMask groundLayer;        // Set this to your "Ground" layer
    [SerializeField] private float minSpawnHeight = 25f;

    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 30f;

    private List<GameObject> spawnedVillagers = new List<GameObject>();
    private int[] spawnCount;

    private void Start()
    {
        if (villageBounds == null)
        {
            villageBounds = GetComponent<BoxCollider>();
            if (villageBounds == null)
            {
                Debug.LogError("No BoxCollider assigned or found for village bounds!");
                return;
            }
        }

        if (villagerPrefabs == null || villagerPrefabs.Length == 0)
        {
            Debug.LogError("No villager prefabs assigned!");
            return;
        }

        spawnCount = new int[villagerPrefabs.Length];
        SpawnInitialVillagers();
    }

    private void Update()
    {
        CheckForDeadVillagers();
    }

    private void SpawnInitialVillagers()
    {
        for (int i = 0; i < villagerPrefabs.Length; i++)
        {
            if (spawnCount[i] >= maxPerType)
                continue;

            Vector3 spawnPos = GetValidSpawnPoint();
            if (spawnPos != Vector3.zero)
            {
                GameObject villager = Instantiate(villagerPrefabs[i], spawnPos, Quaternion.identity);
                spawnedVillagers.Add(villager);
                spawnCount[i]++;
                Debug.Log($"Spawned villager type {i} at {spawnPos}");
            }
        }
    }

    private Vector3 GetValidSpawnPoint()
    {
        // Try multiple times to find a valid spawn point
        for (int i = 0; i < 10; i++)
        {
            // Random position within BoxCollider bounds
            Vector3 center = villageBounds.transform.TransformPoint(villageBounds.center);
            Vector3 size = villageBounds.size;
            Vector3 randomPos = center + new Vector3(
                Random.Range(-size.x / 2f, size.x / 2f),
                size.y / 2f + 50f, // start ray above box
                Random.Range(-size.z / 2f, size.z / 2f)
            );

            // Raycast down to find ground layer
            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 200f, groundLayer))
            {
                // Ensure hit point is inside the box collider bounds and above height limit
                if (villageBounds.bounds.Contains(hit.point) && hit.point.y >= minSpawnHeight)
                {
                    return hit.point;
                }
            }
        }

        Debug.LogWarning("Failed to find valid ground inside village bounds.");
        return Vector3.zero;
    }

    private void CheckForDeadVillagers()
    {
        for (int i = spawnedVillagers.Count - 1; i >= 0; i--)
        {
            GameObject villager = spawnedVillagers[i];

            if (villager == null)
            {
                HandleVillagerDeath(i, null);
                continue;
            }

            EnemyHealth health = villager.GetComponent<EnemyHealth>();
            if (health != null && health.Health <= 0)
            {
                HandleVillagerDeath(i, villager);
            }
        }
    }

    private void HandleVillagerDeath(int index, GameObject villager)
    {
        int typeIndex = GetVillagerTypeIndex(villager);
        if (typeIndex >= 0)
            spawnCount[typeIndex] = Mathf.Max(0, spawnCount[typeIndex] - 1);

        if (villager != null)
            Destroy(villager);

        spawnedVillagers.RemoveAt(index);
        StartCoroutine(RespawnVillager(respawnDelay, typeIndex));
    }

    private IEnumerator RespawnVillager(float delay, int typeIndex)
    {
        yield return new WaitForSeconds(delay);

        if (typeIndex < 0 || typeIndex >= villagerPrefabs.Length)
            yield break;

        if (spawnCount[typeIndex] >= maxPerType)
            yield break;

        Vector3 spawnPos = GetValidSpawnPoint();
        if (spawnPos == Vector3.zero)
            yield break;

        GameObject newVillager = Instantiate(villagerPrefabs[typeIndex], spawnPos, Quaternion.identity);
        spawnedVillagers.Add(newVillager);
        spawnCount[typeIndex]++;
        Debug.Log($"Respawned villager type {typeIndex} at {spawnPos}");
    }

    private int GetVillagerTypeIndex(GameObject villager)
    {
        if (villager == null)
            return -1;

        for (int i = 0; i < villagerPrefabs.Length; i++)
        {
            if (villager.name.Contains(villagerPrefabs[i].name))
                return i;
        }

        return -1;
    }
}
