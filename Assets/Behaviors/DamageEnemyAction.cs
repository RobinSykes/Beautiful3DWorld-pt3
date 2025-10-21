using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Damage Enemy",
    story: "Damage [Target]",
    category: "Action",
    id: "e1a6e71d0b2d4aec0dd9a455857b403f")]
public partial class DamageEnemyAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        GameObject enemy = Target?.Value;

        if (enemy == null)
        {
            Debug.LogWarning("DamageEnemyAction: Target is null.");
            return Status.Failure;
        }

        EnemyHealth healthComponent = enemy.GetComponent<EnemyHealth>();

        if (healthComponent == null)
        {
            Debug.LogWarning($"DamageEnemyAction: {enemy.name} has no EnemyHealth component.");
            return Status.Failure;
        }

        if (healthComponent.IsDead)
        {
            Debug.Log($"{enemy.name} is already dead — skipping damage.");
            return Status.Failure;
        }

        healthComponent.TakeDamage(20f);
        Debug.Log($"DamageEnemyAction: {enemy.name} took 20 damage.");

        return Status.Success;
    }
}
