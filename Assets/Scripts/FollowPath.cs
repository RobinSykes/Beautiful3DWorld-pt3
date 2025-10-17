using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] waypoints;
    public float speed = 2f;
    public float waypointProximity = 2f; // Distance threshold for reaching a waypoint

    [Header("Enemy Detection")]
    public LayerMask enemyLayer;          // Set this in the inspector to your "Enemy" layer
    public float detectionRadius = 20f;   // Pause path following if any enemy within this range

    [Header("Animation Settings")]
    public Animator animator;             // Assign your Animator in the Inspector
    private string speedParam = "SpeedMagnitude"; // BlendTree parameter name
    private float animationDampTime = 0.1f; // Smooth transitions

    private int currentIndex = 0;
    private bool returning = false;

    void Update()
    {
        // Detect enemies and pause if nearby
        if (IsEnemyNearby())
        {
            SetAnimationSpeed(0f);
            return;
        }

        if (waypoints == null || waypoints.Length == 0)
        {
            SetAnimationSpeed(0f);
            return;
        }

        Transform target = waypoints[currentIndex];
        Vector3 direction = (target.position - transform.position).normalized;

        float moveSpeed = speed;

        // Move toward the waypoint
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Update animation speed (based on whether we’re moving)
        float currentSpeed = direction.magnitude * moveSpeed;
        SetAnimationSpeed(currentSpeed);

        // Check if within proximity range
        if (Vector3.Distance(transform.position, target.position) < waypointProximity)
        {
            if (!returning)
            {
                currentIndex++;
                if (currentIndex >= waypoints.Length)
                {
                    returning = true;
                    currentIndex = waypoints.Length - 2; // Go back to previous waypoint
                }
            }
            else
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    returning = false;
                    currentIndex = 1; // Start moving forward again
                }
            }
        }

        // Rotate toward movement direction
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    private bool IsEnemyNearby()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer, QueryTriggerInteraction.Ignore);
        return hits != null && hits.Length > 0;
    }

    private void SetAnimationSpeed(float value)
    {
        if (animator != null)
        {
            // Normalize speed for BlendTree (e.g., 0 = idle, 1 = full walk)
            float normalizedSpeed = Mathf.InverseLerp(0f, speed, value);
            animator.SetFloat(speedParam, normalizedSpeed, animationDampTime, Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length - 1; i++)
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
    }
}
