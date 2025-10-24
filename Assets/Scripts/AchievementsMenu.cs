using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.UIElements;
public class AchievementsMenu : MonoBehaviour
{
    [SerializeField] private UIDocument AchievementsMenuDocument;
    private Button button;
    private void Awake()
    {
        AchievementsMenuDocument = GetComponent<UIDocument>();
        button = AchievementsMenuDocument.rootVisualElement.Q("MainMenuButton") as Button;
        button.RegisterCallback<ClickEvent>(OnPlayGameClick);
    }
    private void OnDisable()
    {
        button.UnregisterCallback<ClickEvent>(OnPlayGameClick);
    }
    private void OnPlayGameClick(ClickEvent evt)
    {
        Debug.Log("Back to main menu");
        SceneManager.LoadScene(0);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
