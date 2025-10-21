using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(
    name: "Enemy Is Dead",
    story: "Check if [Enemy] has health <= 0",
    category: "Conditions",
    id: "9465674573af44dd5cb4d7f0790a7720")]
public partial class EnemyIsDeadCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;

    public override bool IsTrue()
    {
        // Safety checks
        if (Enemy == null || Enemy.Value == null)
        {
            return false;
        }

        // Try get the EnemyHealth component
        EnemyHealth health = Enemy.Value.GetComponent<EnemyHealth>();
        if (health == null)
        {
            Debug.LogWarning($"EnemyIsDeadCondition: {Enemy.Value.name} has no EnemyHealth component.");
            return false;
        }

        // Return true if the enemy is dead
        return health.IsDead || health.Health <= 0;
    }

    public override void OnStart() { }
    public override void OnEnd() { }
}
