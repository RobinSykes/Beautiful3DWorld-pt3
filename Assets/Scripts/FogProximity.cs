using UnityEngine;

public class FogProximity : MonoBehaviour
{
    public Transform player;             // Player transform
    public ParticleSystem playerFog;     // Particle System attached to player
    public float activationDistance = 20f; // Distance to trigger fog

    private Transform[] fogBoundaries;

    void Start()
    {
        if (!playerFog)
        {
            Debug.LogError("Player Fog ParticleSystem not assigned!");
            return;
        }

        playerFog.Stop(); // Make sure it starts disabled

        // Find all fog boundaries by tag
        GameObject[] boundaries = GameObject.FindGameObjectsWithTag("fogBoundary");
        fogBoundaries = new Transform[boundaries.Length];
        for (int i = 0; i < boundaries.Length; i++)
        {
            fogBoundaries[i] = boundaries[i].transform;
        }
    }

    void Update()
    {
        if (!player || fogBoundaries == null || fogBoundaries.Length == 0) return;

        bool nearAnyBoundary = false;

        // Check distance to all fog boundaries
        foreach (Transform boundary in fogBoundaries)
        {
            float distance = Vector3.Distance(player.position, boundary.position);
            if (distance <= activationDistance)
            {
                nearAnyBoundary = true;
                break; // No need to check others
            }
        }

        // Enable or disable fog based on proximity
        if (nearAnyBoundary)
        {
            if (!playerFog.isPlaying)
                playerFog.Play();
        }
        else
        {
            if (playerFog.isPlaying)
                playerFog.Stop();
        }
    }
}
