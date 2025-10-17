using System.Collections;
using Unity.Behavior;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float Health = 100;
    public float Maxhealth = 100;
    public Animator animator;
    [SerializeField] public FloatingHealthBar HealthBar;

    private BehaviorGraphAgent behaviorGraph;

    private void Awake()
    {
        HealthBar = GetComponentInChildren<FloatingHealthBar>();
        animator = GetComponentInChildren<Animator>();
        behaviorGraph = GetComponent<BehaviorGraphAgent>(); // or BehaviorExecutor if that's what you're using
    }

    void Start()
    {
        if (HealthBar != null)
            HealthBar.UpdateHealthBar(Health, Maxhealth);
    }

    public void TakeDamage(float damageAmount)
    {
        // --- NEW: check if player is blocking before taking damage ---
        PlayerAnimationController player = Object.FindFirstObjectByType<PlayerAnimationController>();
        if (player != null && player.IsBlocking())
        {
            Debug.Log($"{gameObject.name} tried to deal damage, but player is blocking!");
            return;
        }
        // -------------------------------------------------------------

        Health -= damageAmount;
        if (HealthBar != null)
            HealthBar.UpdateHealthBar(Health, Maxhealth);

        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        // Pick random death animation
        int randomDeath = Random.Range(0, 3);
        animator.SetInteger("DeathIndex", randomDeath);
        Debug.Log($"{gameObject.name} has died.");

        // Disable the behavior graph
        if (behaviorGraph != null)
        {
            behaviorGraph.enabled = false;
            Debug.Log("Behavior graph disabled on death.");
        }

        // Disable health bar
        if (HealthBar != null)
        {
            HealthBar.gameObject.SetActive(false);
            Debug.Log("Health bar hidden on death.");
        }

        // Trigger death animation
        if (animator != null)
            animator.SetTrigger("IsDead");

        StartCoroutine(HandleDeathSequence());
    }

    private IEnumerator HandleDeathSequence()
    {
        // Wait 3 seconds before sinking
        gameObject.layer = LayerMask.NameToLayer("Default");
        yield return new WaitForSeconds(3f);

        // Sink into ground over 2 seconds
        yield return StartCoroutine(SinkIntoGround(2f, 2f));

        // Wait 5 more seconds before destroying
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private IEnumerator SinkIntoGround(float duration, float distance)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos - new Vector3(0, distance, 0);

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
        transform.position = endPos;
    }
}
