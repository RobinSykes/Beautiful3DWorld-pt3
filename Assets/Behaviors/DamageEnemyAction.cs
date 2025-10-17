using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Damage Enemy", story: "Damage [Target]", category: "Action", id: "e1a6e71d0b2d4aec0dd9a455857b403f")]
public partial class DamageEnemyAction : Action
{

    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        GameObject enemy = Target?.Value;
        EnemyHealth healthComponent = enemy?.GetComponent<EnemyHealth>();

        if (healthComponent != null)
        {
            healthComponent.TakeDamage(20f); // Apply 10 damage
            Debug.Log("Enemy Took Damage");
        }
        else
        {
            Debug.LogWarning("Target does not have an EnemyHealth component.");
        }

        return Status.Running;
    }

}

