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
        Debug.Log("Show Settings Menu");
    }

    private void ShowAchievementsMenu()
    {
        Debug.Log("Show Achievements Menu");
    }

    private void Play()
    {
        // Always reload MainWorld scene fresh
        SceneManager.LoadScene("MainWorld", LoadSceneMode.Single);
        Debug.Log("Playing MainWorld - scene reset");
    }
}
