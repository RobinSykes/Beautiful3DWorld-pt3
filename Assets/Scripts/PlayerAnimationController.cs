using UnityEngine;
using System.Collections;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private LayerMask enemyLayer;

    private float nextAttackTime = 0f;
    private bool isBlocking = false;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Left-click: Attack
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            TriggerRandomAttack();
            StartCoroutine(DelayedDamage(1f)); // Wait 1 second before dealing damage
            nextAttackTime = Time.time + attackCooldown;
        }

        // Right-click: Block / Invincibility
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool("blockAttack", true);
            isBlocking = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("blockAttack", false);
            isBlocking = false;
        }
    }

    private void TriggerRandomAttack()
    {
        int randomAttack = Random.Range(0, 3);
        string triggerName = randomAttack switch
        {
            0 => "Attack_A",
            1 => "Attack_B",
            2 => "Attack_C",
            _ => "Attack_A"
        };

        animator.SetTrigger(triggerName);
        Debug.Log($"Triggered {triggerName}");
    }

    private IEnumerator DelayedDamage(float delay)
    {
        yield return new WaitForSeconds(delay);
        DealDamage();
    }

    private void DealDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log($"Dealt {attackDamage} damage to {enemy.name}");
            }
        }
    }

    public bool IsBlocking()
    {
        return isBlocking;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
