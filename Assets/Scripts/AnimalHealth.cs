using System.Collections;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class AnimalHealth : MonoBehaviour
{
    public float Health = 100;
    public float Maxhealth = 100;
    [SerializeField] public FloatingHealthBar HealthBar;

    private Animator animator;
    private BehaviorGraphAgent behaviorGraph;
    private bool isDead = false;
    public bool IsDead => isDead;

    private CapsuleCollider capsuleCollider;
    private MeshCollider meshCollider;
    private NavMeshAgent navMeshAgent;
    private MonoBehaviour movementScript; // Optional: your animal movement script

    private void Awake()
    {
        HealthBar = GetComponentInChildren<FloatingHealthBar>();
        animator = GetComponentInChildren<Animator>();
        behaviorGraph = GetComponent<BehaviorGraphAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        meshCollider = GetComponent<MeshCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        // If you have a custom movement script (like AnimalMovement, AIController, etc.), reference it here:
        movementScript = GetComponent<MonoBehaviour>(); // replace with your specific movement script type if you have one
    }

    private void Start()
    {
        if (HealthBar != null)
            HealthBar.UpdateHealthBar(Health, Maxhealth);
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        PlayerAnimationController player = Object.FindFirstObjectByType<PlayerAnimationController>();
        if (player != null && player.IsBlocking())
        {
            Debug.Log($"{gameObject.name} tried to deal damage, but player is blocking!");
            return;
        }

        Health -= damageAmount;
        if (HealthBar != null)
            HealthBar.UpdateHealthBar(Health, Maxhealth);

        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Neutralize it (no AI, collisions, etc.)
        gameObject.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("Default");

        // Disable colliders
        if (capsuleCollider != null) capsuleCollider.enabled = false;
        if (meshCollider != null) meshCollider.enabled = false;

        // Stop movement
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
        }

        if (movementScript != null)
            movementScript.enabled = false;

        // Disable AI behavior
        if (behaviorGraph != null)
            behaviorGraph.enabled = false;

        // Disable health bar
        if (HealthBar != null)
            HealthBar.gameObject.SetActive(false);

        // Disable animator completely (no animation will play)
        if (animator != null)
            animator.enabled = false;

        Debug.Log($"{gameObject.name} has died — sinking immediately.");

        // Immediately start sinking
        StartCoroutine(SinkIntoGround(2f, 2f));
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

        transform.position = endPos;
        Destroy(gameObject);
    }
}
