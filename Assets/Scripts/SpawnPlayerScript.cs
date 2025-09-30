using UnityEngine;

public class SpawnPlayerScript : MonoBehaviour
{
    public GameObject[] spawnLocations;
    public GameObject player;

    private Vector3 respawnLocation;

    void Awake()
    {
        spawnLocations = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = (GameObject)Resources.Load("Player", typeof(GameObject));

        respawnLocation = player.transform.position;

        SpawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnPlayer()
    {
        int spawn = Random.Range(0, spawnLocations.Length);
        GameObject.Instantiate(player,spawnLocations[spawn].transform.position, Quaternion.identity);
    }
}
