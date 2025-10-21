using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Set Speed In Self",
    story: "Set NavMesh speed in [Self] to 5",
    category: "Action/Navigation",
    id: "1e5a579483ea94557a677f49c1cc781a")]
public partial class SetSpeedInSelfAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        if (Self?.Value == null)
        {
            Debug.LogWarning("SetSpeedInSelfAction: Self is not assigned.");
            return Status.Failure;
        }

        NavMeshAgent agent = Self.Value.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogWarning("SetSpeedInSelfAction: Self has no NavMeshAgent component.");
            return Status.Failure;
        }

        agent.speed = 5f;
        Debug.Log($"{Self.Value.name}'s NavMeshAgent speed set to 5.");

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        // Nothing to update — this action just sets the value once
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}
