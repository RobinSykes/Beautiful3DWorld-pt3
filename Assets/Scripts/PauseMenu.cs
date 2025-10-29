using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject pauseMenuUI;  // PauseMenu panel
    public MonoBehaviour cameraController; // Camera movement script
    public AudioSource gameMusic; // GameManager music
    public AudioSource sfxSound;
    public AudioSource bossMusic;

    private UIDocument pauseUIDoc;
    private Button menuButton;
    private Slider musicSlider;
    private Slider sfxSlider;
    private Slider bossSlider;
    private bool isPaused = false;

    private void Start()
    {
        if (pauseMenuUI != null)
            pauseUIDoc = pauseMenuUI.GetComponent<UIDocument>();

        if (pauseUIDoc == null)
            Debug.LogError("PauseMenu: No UIDocument found on pauseMenuUI!");

        // Apply saved settings immediately on game start
        ApplySavedSettings();
    }

    private void Update()
    {
        // Detect Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freeze game
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None; // Unlock cursor

        if (cameraController != null)
            cameraController.enabled = false;

        // Fetch buttons and sliders AFTER enabling UI
        if (pauseUIDoc != null)
        {
            var root = pauseUIDoc.rootVisualElement;

            // Menu button
            menuButton = root.Q<Button>("MenuButton");
            if (menuButton != null)
            {
                menuButton.clicked -= OnMenuButtonClicked;
                menuButton.clicked += OnMenuButtonClicked;
            }

            // Music slider
            musicSlider = root.Q<Slider>("MusicVolumeSlider");
            if (musicSlider != null)
            {
                musicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MusicVolume", 1f));
                musicSlider.RegisterValueChangedCallback(evt =>
                {
                    if (gameMusic != null)
                        gameMusic.volume = evt.newValue;
                    PlayerPrefs.SetFloat("MusicVolume", evt.newValue);
                    PlayerPrefs.Save();
                });
            }

            // SFX slider
            sfxSlider = root.Q<Slider>("SFXVolumeSlider");
            if (sfxSlider != null)
            {
                sfxSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFXVolume", 1f));
                sfxSlider.RegisterValueChangedCallback(evt =>
                {
                    if (sfxSound != null)
                        sfxSound.volume = evt.newValue;
                    PlayerPrefs.SetFloat("SFXVolume", evt.newValue);
                    PlayerPrefs.Save();
                });
            }

            // Boss music slider
            bossSlider = root.Q<Slider>("BossMusicSlider");
            if (bossSlider != null)
            {
                bossSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("BossMusicVolume", 1f));
                bossSlider.RegisterValueChangedCallback(evt =>
                {
                    if (bossMusic != null)
                        bossMusic.volume = evt.newValue;
                    PlayerPrefs.SetFloat("BossMusicVolume", evt.newValue);
                    PlayerPrefs.Save();
                });
            }
        }

        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        if (cameraController != null)
            cameraController.enabled = true;

        isPaused = false;
    }

    private void OnMenuButtonClicked()
    {
        if (gameMusic != null) gameMusic.Stop();
        if (sfxSound != null) sfxSound.Stop();
        if (bossMusic != null) bossMusic.Stop();

        Time.timeScale = 1f;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private void ApplySavedSettings()
    {
        if (gameMusic != null)
            gameMusic.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        if (sfxSound != null)
            sfxSound.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (bossMusic != null)
            bossMusic.volume = PlayerPrefs.GetFloat("BossMusicVolume", 1f);
    }
}
