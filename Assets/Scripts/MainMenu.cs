using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIDocument mainMenuDocument;

    private Button settingsButton;
    private Button achievementsButton;
    private Button playButton;

    private void Awake()
    {
        VisualElement root = mainMenuDocument.rootVisualElement;

        settingsButton = root.Q<Button>("SettingsButton");
        achievementsButton = root.Q<Button>("AchievementsButton");
        playButton = root.Q<Button>("PlayButton");

        settingsButton.clickable.clicked += ShowSettingsMenu;
        achievementsButton.clickable.clicked += ShowAchievementsMenu;
        playButton.clickable.clicked += Play;
    }

    private void ShowSettingsMenu()
    {
        SceneManager.LoadScene("SettingsMenu");
        Debug.Log("Show Settings Menu");
    }

    private void ShowAchievementsMenu()
    {
        //SceneManager.LoadScene("AchievementsMenu");
        Debug.Log("Show Achievements Menu");
    }

    private void Play()
    {
        // Destroy existing GameManager before reloading scene
        GameObject existingManager = GameObject.Find("GameManager");
        if (existingManager != null)
        {
            Destroy(existingManager);
            Debug.Log("Old GameManager destroyed before scene reload.");
        }

        // Reload MainWorld fresh
        SceneManager.LoadScene("MainWorld", LoadSceneMode.Single);
        Debug.Log("Playing MainWorld - scene reset");
    }

}
