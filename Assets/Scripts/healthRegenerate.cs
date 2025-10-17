using UnityEngine;

public class HealthRegenerate : MonoBehaviour
{
    [Header("Regeneration Settings")]
    public int regenAmount = 5; // health points per tick
    public float regenInterval = 5f; // seconds between each tick

    private EnemyHealth enemyHealth;
    private float timer = 0f;

    void Awake()
    {
        enemyHealth = GetComponentInChildren<EnemyHealth>();
        if (enemyHealth == null)
        {
            Debug.LogError("HealthRegenerate: No EnemyHealth component found on this GameObject.");
        }
    }

    void Update()
    {
        if (enemyHealth == null) return;

        // Only heal if health is not full
        if (enemyHealth.Health < enemyHealth.Maxhealth)
        {
            timer += Time.deltaTime;
            if (timer >= regenInterval)
            {
                Heal();
                timer = 0f;
            }
        }
    }

    private void Heal()
    {
        enemyHealth.Health += regenAmount;
        if (enemyHealth.Health > enemyHealth.Maxhealth)
            enemyHealth.Health = enemyHealth.Maxhealth;

        // Update health bar if available
        if (enemyHealth.HealthBar != null)
            enemyHealth.HealthBar.UpdateHealthBar(enemyHealth.Health, enemyHealth.Maxhealth);
    }
}
