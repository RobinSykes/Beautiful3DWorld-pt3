using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    private DropdownField difficultyDropdown;
    private Button backButton;

    private void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;

        difficultyDropdown = root.Q<DropdownField>("DifficultyDropdown");
        backButton = root.Q<Button>("BackButton");

        difficultyDropdown.choices = new System.Collections.Generic.List<string> { "Easy", "Medium", "Hard" };

        // Default to Easy if no saved value exists
        string savedDifficulty = PlayerPrefs.GetString("GameDifficulty", "Easy");
        difficultyDropdown.value = savedDifficulty;

        difficultyDropdown.RegisterValueChangedCallback(evt =>
        {
            PlayerPrefs.SetString("GameDifficulty", evt.newValue);
            PlayerPrefs.Save();
            Debug.Log($"[SettingsManager] Difficulty set to {evt.newValue}");
        });

        backButton.clicked += OnBackPressed;
    }

    private void OnBackPressed()
    {
        // Save before leaving
        PlayerPrefs.Save();
        Debug.Log("[SettingsManager] Returning to main menu...");
        SceneManager.LoadScene("MainMenu"); // or your actual main menu scene name
    }
}
