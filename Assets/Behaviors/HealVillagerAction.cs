using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "heal villager", story: "Heal [Self]", category: "Action", id: "ea8c8747813100dc56e78dc97889233a")]
public partial class HealVillagerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeField] private int healAmount = 5;        // Heal amount per tick
    [SerializeField] private float healInterval = 5f;   // Seconds between heals

    private EnemyHealth villagerHealth;
    private float timer = 0f;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            Debug.LogWarning("HealVillagerAction: Self is null.");
            return Status.Failure;
        }

        villagerHealth = Self.Value.GetComponent<EnemyHealth>();
        if (villagerHealth == null)
        {
            Debug.LogWarning("HealVillagerAction: No EnemyHealth component found on Self.");
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (villagerHealth == null) return Status.Failure;

        // Only heal if health is not full
        if (villagerHealth.Health < villagerHealth.Maxhealth)
        {
            timer += Time.deltaTime;
            if (timer >= healInterval)
            {
                villagerHealth.Health += healAmount;
                if (villagerHealth.Health > villagerHealth.Maxhealth)
                    villagerHealth.Health = villagerHealth.Maxhealth;

                if (villagerHealth.HealthBar != null)
                    villagerHealth.HealthBar.UpdateHealthBar(villagerHealth.Health, villagerHealth.Maxhealth);

                timer = 0f;
            }

            return Status.Running; // Keep running until full health
        }

        return Status.Success; // Fully healed
    }

    protected override void OnEnd()
    {
        timer = 0f;
    }
}
