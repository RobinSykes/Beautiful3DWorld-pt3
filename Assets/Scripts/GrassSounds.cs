using StarterAssets;
using UnityEngine;

public class GrassSounds : MonoBehaviour
{
    public GameObject targetObject; // Assign in Inspector
    private ThirdPersonController thirdPersonController;
    public AudioSource grassSounds;
    public AudioClip[] grassSoundsClip;
    private float soundTimer = 0f;
    private float delayBetweenSounds = 0.6f; // sound delay
    private bool isGrounded = true;

    void Start()
    {
        thirdPersonController = targetObject.GetComponent<ThirdPersonController>();
    }

    void Update()
    {
        CheckGrounded();

        if (Input.GetKey(KeyCode.W) && isGrounded)
        {
            soundTimer += Time.deltaTime;

            if (soundTimer >= delayBetweenSounds)
            {
                if (grassSounds != null && grassSoundsClip.Length > 0)
                {
                    AudioClip activeGrassSounds = grassSoundsClip[Random.Range(0, grassSoundsClip.Length)];
                    grassSounds.PlayOneShot(activeGrassSounds);
                    soundTimer = 0f;
                }
            }
        }
        else
        {
            soundTimer = delayBetweenSounds;
        }
    }

    void CheckGrounded()
    {
        isGrounded = thirdPersonController.isGrounded;
    }
}
