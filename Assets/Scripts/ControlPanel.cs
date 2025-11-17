using UnityEngine;

public class ControlPanel : MonoBehaviour
{
    public GameObject controlPanel;
    private bool controlPanelVisible = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && controlPanel != null)
        {
            if (controlPanelVisible)
            {
                controlPanel.SetActive(true);
                controlPanelVisible = false;
            }
            else
            {
                controlPanel.SetActive(false);
                controlPanelVisible = true;
            }
        }
    }
}
