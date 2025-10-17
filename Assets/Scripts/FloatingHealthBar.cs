using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider Slider;
    [SerializeField] private Camera Camera;
    private void Awake()
    {
        // Try to get a reference to the player's camera if none is set
        if (Camera == null)
        {
            // Option 1: Find by tag (recommended)
            GameObject playerCameraObj = GameObject.FindGameObjectWithTag("MainCamera");

            if (playerCameraObj != null)
            {
                Camera = playerCameraObj.GetComponent<Camera>();
            }
            else
            {
                Debug.LogWarning("FloatingHealthBar could not find a MainCamera in the scene.");
            }
        }
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        Slider.value = currentValue / maxValue;
    }

    private void LateUpdate()
    {
        // Face toward the camera
        if (Camera != null)
        {
            transform.rotation = Camera.transform.rotation;
        }
    }
}
