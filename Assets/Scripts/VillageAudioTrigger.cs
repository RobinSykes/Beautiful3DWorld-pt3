using UnityEngine;

public class VillageAudioTrigger : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Distance (in meters) within which the player triggers the audio")]
    public float triggerDistance = 75f;

    [Header("Audio Settings")]
    [Tooltip("Audio sources to play when player is nearby (can be ambient, music, etc.)")]
    public AudioSource[] villageAudioSources;

    [Tooltip("Should the audio stop when player leaves range?")]
    public bool stopOnExit = true;

    [Header("References")]
    [Tooltip("The player GameObject (if not set, script will find one with tag 'Player')")]
    public Transform player;

    private bool hasTriggered = false;

    private void Start()
    {
        // Automatically find player if not manually assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning("VillageAudioTrigger: No Player found in scene!");
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= triggerDistance && !hasTriggered)
        {
            TriggerVillageAudio();
        }
        else if (distance > triggerDistance && hasTriggered && stopOnExit)
        {
            StopVillageAudio();
        }
    }

    private void TriggerVillageAudio()
    {
        hasTriggered = true;
        Debug.Log($"?? Player entered village range ({triggerDistance}m) — starting audio...");

        foreach (var audio in villageAudioSources)
        {
            if (audio != null && !audio.isPlaying)
            {
                audio.Play();
                Debug.Log($"Playing: {audio.clip?.name}");
            }
        }
    }

    private void StopVillageAudio()
    {
        hasTriggered = false;
        Debug.Log("?? Player left village range — stopping audio...");

        foreach (var audio in villageAudioSources)
        {
            if (audio != null && audio.isPlaying)
            {
                audio.Stop();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 1f, 0.4f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}
