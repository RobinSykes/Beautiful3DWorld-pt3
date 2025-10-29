using UnityEngine;
using UnityEngine.UIElements;

public class sfxVolumeSlider : MonoBehaviour
{
    private Slider musicSlider;
    private AudioSource[] playerAudioSources;  // All AudioSources on player
    private float savedVolume = 1f;

    private void Awake()
    {
        // Find the player
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerAudioSources = player.GetComponents<AudioSource>();

        if (playerAudioSources == null || playerAudioSources.Length == 0)
        {
            Debug.LogWarning("SFXVolumeSlider: No AudioSources found on Player!");
        }

        // Load saved volume (default 1)
        savedVolume = PlayerPrefs.GetFloat("SFXVolumeSlider", 1f);

        // Apply saved volume to all AudioSources
        if (playerAudioSources != null)
        {
            foreach (var source in playerAudioSources)
                source.volume = savedVolume;
        }
    }

    private void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null) return;

        var root = uiDoc.rootVisualElement;
        musicSlider = root.Q<Slider>("SFXVolumeSlider");
        if (musicSlider == null) return;

        // Set slider to saved value
        musicSlider.SetValueWithoutNotify(savedVolume);

        // Register callback
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

        if (playerAudioSources != null)
        {
            foreach (var source in playerAudioSources)
                source.volume = savedVolume;
        }

        PlayerPrefs.SetFloat("SFXVolumeSlider", savedVolume);
        PlayerPrefs.Save();

        Debug.Log($"Player audio volume changed to {savedVolume}");
    }
}
