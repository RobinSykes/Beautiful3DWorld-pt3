using UnityEngine;

public class WorldButtonBossBear : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public KeyCode interactKey2 = KeyCode.Mouse0;
    public GameObject promptUI; // Optional: a UI element that says "Press E"
    public UnityEngine.Events.UnityEvent onActivate; // Assign what happens when pressed in the Inspector

    private bool playerInRange = false;

    void Start()
    {

    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            Debug.Log("Button activated!");
            onActivate.Invoke();
        }
        if (playerInRange && Input.GetKeyDown(interactKey2))
        {
            Debug.Log("Button activated!");
            onActivate.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
