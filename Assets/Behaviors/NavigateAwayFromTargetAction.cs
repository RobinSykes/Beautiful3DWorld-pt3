using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Navigate Away From Target",
    story: "Make [Self] run 50 units away from [Target]",
    category: "Action/Navigation",
    id: "5644d5a0091a01cfa977fbe3e848b025")]
public partial class NavigateAwayFromTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private NavMeshAgent agent;
    private Vector3 destination;
    private bool destinationSet = false;

    protected override Status OnStart()
    {
        if (Self?.Value == null || Target?.Value == null)
        {
            Debug.LogWarning("NavigateAwayFromTargetAction: Self or Target not assigned.");
            return Status.Failure;
        }

        agent = Self.Value.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogWarning("NavigateAwayFromTargetAction: Self has no NavMeshAgent.");
            return Status.Failure;
        }

        Vector3 from = Self.Value.transform.position;
        Vector3 awayDir = (from - Target.Value.transform.position).normalized;

        // Base flee distance
        float fleeDistance = 50f;

        // Try to find a valid NavMesh position — test several angles if needed
        Vector3 bestPoint = Vector3.zero;
        bool found = false;

        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f; // Try every 45 degrees around the away direction
            Vector3 dir = Quaternion.Euler(0, angle, 0) * awayDir;
            Vector3 candidate = from + dir * fleeDistance;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 25f, NavMesh.AllAreas))
            {
                bestPoint = hit.position;
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.LogWarning($"{Self.Value.name}: Could not find valid NavMesh point to flee to.");
            return Status.Failure;
        }

        // Set destination and rotation
        destination = bestPoint;
        agent.isStopped = false;
        agent.SetDestination(destination);
        destinationSet = true;

        Quaternion lookRotation = Quaternion.LookRotation(destination - from, Vector3.up);
        Self.Value.transform.rotation = lookRotation;

        Debug.Log($"{Self.Value.name} is fleeing to {destination}");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!destinationSet || agent == null)
            return Status.Failure;

        // Face movement direction while running
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(agent.velocity.normalized, Vector3.up);
            Self.Value.transform.rotation = Quaternion.Slerp(Self.Value.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // Check if destination reached
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            agent.isStopped = true;
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (agent != null)
            agent.isStopped = true;

        destinationSet = false;
    }
}
