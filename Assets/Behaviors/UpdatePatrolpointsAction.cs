using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Update Patrol Points",
    story: "Update [PatrolPoints] with new points from [RandomPatrolPoints]",
    category: "Action",
    id: "3aa69c83bb62d457bea1ebf4d1730b76")]
public partial class UpdatePatrolpointsAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> PatrolPoints;
    [SerializeReference] public BlackboardVariable<RandomPointsInBoxCollider> RandomPatrolPoints;

    protected override Status OnStart()
    {
        if (RandomPatrolPoints == null || RandomPatrolPoints.Value == null)
        {
            return Status.Failure;
        }

        var spawner = RandomPatrolPoints.Value;
        List<GameObject> newPatrolPoints = new List<GameObject>();
        foreach (var t in spawner.patrolPoints)
        {
            if (t != null)
                newPatrolPoints.Add(t.gameObject);
        }
        if (spawner.patrolPoints == null || spawner.patrolPoints.Count == 0)
        {
            Debug.LogWarning("Spawner has no patrol points!");
            return Status.Failure;
        }

        PatrolPoints.Value = newPatrolPoints;

        Debug.Log($"[UpdatePatrolpointsAction] Updated PatrolPoints with {newPatrolPoints.Count} new points.");

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {

    }
}
