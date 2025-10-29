using System.Collections;
using Unity.Behavior;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float Health = 100;
    public float Maxhealth = 100;
    public Animator animator;
    public FloatingHealthBar HealthBar;
    private BehaviorGraphAgent behaviorGraph;
    private bool isDead = false;
    public bool IsDead => isDead;

    private CapsuleCollider capsuleCollider;
    public AudioSource deathAudio;

    private void Awake()
    {
        HealthBar = GetComponentInChildren<FloatingHealthBar>();
        animator = GetComponentInChildren<Animator>();
        behaviorGraph = GetComponent<BehaviorGraphAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        deathAudio = GetComponentInChildren<AudioSource>();
    }

    void Start()
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
            PlayerAudio playerAudio = GetComponent<PlayerAudio>();
            if (playerAudio != null)
                playerAudio.BlockAudio();
            Debug.Log($"{gameObject.name} tried to deal damage, but player is blocking!");
                return;
            }
            if (gameObject.CompareTag("Player"))
            {
                PlayerAnimationController playerAnimationController = gameObject.GetComponent<PlayerAnimationController>();
                if (playerAnimationController != null)
                {
                    playerAnimationController.PlayBloodParticle();
                }
            }

        Health -= damageAmount;
            if (HealthBar != null)
                HealthBar.UpdateHealthBar(Health, Maxhealth);
        if (gameObject.CompareTag("Player"))
        {
            PlayerAudio playerAudio = GetComponent<PlayerAudio>();
            if (playerAudio != null)
                playerAudio.HitAudio();
        }
            if (Health <= 0)
                Die();
        
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        deathAudio.pitch = Random.Range(0.8f, 1.2f);
        deathAudio.Play();
        Debug.Log($"Deathaudio played with pitch {deathAudio.pitch}");
        KillManager.Instance?.AddKill(transform.position);
        gameObject.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("Default");
        if (capsuleCollider != null)
            capsuleCollider.enabled = false;

        int randomDeath = Random.Range(0, 3);
        animator.SetInteger("DeathIndex", randomDeath);
        Debug.Log($"{gameObject.name} has died.");
        if (behaviorGraph != null)
        {
            behaviorGraph.enabled = false;
            Debug.Log("Behavior graph disabled on death.");
        }

        if (HealthBar != null)
        {
            HealthBar.gameObject.SetActive(false);
            Debug.Log("Health bar hidden on death.");
        }

        if (animator != null)
            animator.SetTrigger("IsDead");

        StartCoroutine(HandleDeathSequence());
    }

    private IEnumerator HandleDeathSequence()
    {
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(SinkIntoGround(2f, 2f));
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

        transform.position = endPos;
        Destroy(gameObject);
    }
}
