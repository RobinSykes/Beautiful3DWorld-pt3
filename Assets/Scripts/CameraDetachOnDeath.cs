using UnityEngine;
using System.Collections;

public class CameraDetachOnDeath : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public EnemyHealth playerHealth;
    public MonoBehaviour cameraFollowScript; // assign your follow script here
    public Animator playerAnimator; // assign your player animator here
    public GameObject enemySpawner;
    [Header("Settings")]
    public string deathAnimationName = "IsDead"; // exact name of the death animation state
    public float pauseDelay = 5f; // seconds after death before pause

    private bool triggered = false;
    private bool animatorLocked = false;

    void Start()
    {
        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        // Get player health
        if (player != null)
            playerHealth = player.GetComponentInChildren<EnemyHealth>();

        if (playerHealth == null)
            Debug.LogError("CameraDetachOnDeath: PlayerHealth reference missing!");

        // Auto-find camera follow script
        if (cameraFollowScript == null)
            cameraFollowScript = GetComponent<MonoBehaviour>();

        // Auto-find player animator if not assigned
        if (playerAnimator == null && player != null)
            playerAnimator = player.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (playerHealth == null) return;

        // When player dies
        if (playerHealth.Health <= 0 && !triggered)
        {
            triggered = true;
            // Disable camera follow script
            if (cameraFollowScript != null)
                cameraFollowScript.enabled = false;
            if (enemySpawner != null)
                enemySpawner.SetActive(false);
            // Disable all player input scripts
            if (player != null)
            {
                MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scripts)
                {
                    if (script != this && script != playerAnimator)
                        script.enabled = false;
                }
            }

            Debug.Log("Player inputs disabled, camera follow disabled.");

            // Start coroutine to wait for death animation to begin
            StartCoroutine(LockAnimatorWhenDeathPlays());

            // Pause game after delay
            StartCoroutine(PauseAfterDelay(pauseDelay));
        }
    }

    private IEnumerator LockAnimatorWhenDeathPlays()
    {
        if (playerAnimator == null) yield break;

        // Wait until death animation is playing
        while (!IsDeathAnimationPlaying())
        {
            yield return null;
        }

        animatorLocked = true;
        Debug.Log("Animator locked after death animation started.");
    }

    private bool IsDeathAnimationPlaying()
    {
        if (playerAnimator == null) return false;
        AnimatorStateInfo state = playerAnimator.GetCurrentAnimatorStateInfo(0);
        return state.IsName(deathAnimationName);
    }

    void LateUpdate()
    {
        if (animatorLocked && playerAnimator != null)
        {
            // Reset all triggers to prevent new animations from playing
            foreach (AnimatorControllerParameter param in playerAnimator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Trigger)
                    playerAnimator.ResetTrigger(param.name);
            }
        }
    }

    private IEnumerator PauseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Time.timeScale = 0f;
        Debug.Log($"Game paused {delay} seconds after player death.");
        Die();
    }
    public void Die()
    {
        // ... existing death logic ...

        // Trigger the UI overlay
        YouDiedUIOverlay overlay = FindFirstObjectByType<YouDiedUIOverlay>();
        if (overlay != null)
        {
            overlay.TriggerYouDied();
            Time.timeScale = 1f;
        }
    }

}
