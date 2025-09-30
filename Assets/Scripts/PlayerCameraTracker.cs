using UnityEngine;

public class PlayerCameraTracker : MonoBehaviour
{
    public Transform playerCamera;
    public float yoffset = 5f;


    void Update()
    {
        Vector3 newPosition = transform.position;
        newPosition.y = yoffset;

        this.transform.position = newPosition;
    }
}
