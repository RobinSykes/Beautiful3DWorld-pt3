using UnityEngine;

public class PlayWalkParticles : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject particleObject;   // The GameObject holding your ParticleSystem
    [SerializeField] private float walkThreshold = 0.1f;  // When Speed > this, walking particles play

    private ParticleSystem ps;

    private void Start()
    {
        // Auto-assign references if not set
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (particleObject == null)
            particleObject = GetComponentInChildren<ParticleSystem>()?.gameObject;

        if (particleObject != null)
            ps = particleObject.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (animator == null || ps == null)
            return;

        float speed = animator.GetFloat("SpeedMagnitude");
        var emission = ps.emission;

        if (speed > walkThreshold)
        {
            emission.enabled = true;
            if (!ps.isPlaying)
                ps.Play();
        }
        else
        {
            emission.enabled = false;
        }
    }

}
