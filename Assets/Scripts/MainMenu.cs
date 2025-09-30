using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] UIDocument mainMenuDocument;

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
        print("Show Settings Menu");
    }
    private void ShowAchievementsMenu()
    {
        print("Show Achievements Menu");
    }
    private void Play()
    {
        SceneManager.LoadScene("MainWorld");
        print("playing");
    }
}
