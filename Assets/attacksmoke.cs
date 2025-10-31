using UnityEngine;

public class Attacksmoke : StateMachineBehaviour
{
    public ParticleSystem attackSmokeEffect; // assign in inspector or dynamically

    // Called when the state starts
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackSmokeEffect = animator.GetComponentInChildren<ParticleSystem>();

    }
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackSmokeEffect == null)
        {
            // Try to find the particle system on the same GameObject
        }

        if (attackSmokeEffect != null)
        {
            attackSmokeEffect.Play();
        }
        else
        {
            Debug.LogWarning("No ParticleSystem assigned or found for attack smoke!");
        }
    }
}
