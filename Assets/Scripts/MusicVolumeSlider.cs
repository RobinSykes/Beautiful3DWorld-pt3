using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class MusicVolumeSlider : MonoBehaviour
{
    private Slider musicSlider;
    private AudioSource musicSource;
    private float savedVolume = 1f;

    private void Awake()
    {
        // Get GameManager audio
        var gameManager = GameObject.FindWithTag("GameManager");
        if (gameManager != null)
            musicSource = gameManager.GetComponent<AudioSource>();

        if (musicSource == null)
        {
            Debug.LogWarning("MusicVolumeSlider: No AudioSource found on GameManager!");
        }

        // Load saved volume (or default to 1)
        savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (musicSource != null)
            musicSource.volume = savedVolume;
    }

    private void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogError("MusicVolumeSlider: No UIDocument found!");
            return;
        }

        var root = uiDoc.rootVisualElement;
        musicSlider = root.Q<Slider>("MusicVolumeSlider");

        if (musicSlider == null)
        {
            Debug.LogWarning("MusicVolumeSlider: Could not find 'MusicVolumeSlider'!");
            return;
        }

        // Set to last saved value
        musicSlider.SetValueWithoutNotify(savedVolume);

        // Re-register callback each time the menu reopens
        musicSlider.RegisterValueChangedCallback(OnVolumeChanged);
    }

    private void OnDisable()
    {
        if (musicSlider != null)
            musicSlider.UnregisterValueChangedCallback(OnVolumeChanged);
    }

    private void OnVolumeChanged(ChangeEvent<float> evt)
    {
        savedVolume = evt.newValue;

        if (musicSource != null)
            musicSource.volume = savedVolume;

        PlayerPrefs.SetFloat("MusicVolume", savedVolume);
        PlayerPrefs.Save();

        Debug.Log($"Music volume changed to {savedVolume}");
    }
}
