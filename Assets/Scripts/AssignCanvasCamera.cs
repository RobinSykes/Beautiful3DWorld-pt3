using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class AssignCanvasCamera : MonoBehaviour
{
    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
        {
            Camera mainCam = Camera.main;

            if (mainCam != null)
            {
                canvas.worldCamera = mainCam;
            }
            else
            {
                Debug.LogWarning($"{name}: No MainCamera found to assign to world-space Canvas!");
            }
        }
    }
}
