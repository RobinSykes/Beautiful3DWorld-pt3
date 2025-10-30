using UnityEngine;
using UnityEngine.UIElements;

public class BackGroundMusicVolumeSlider : MonoBehaviour
{
    private Slider backGroundSlider;
    private AudioSource backGroundMusicSource;
    private float savedVolume = 1f;

    private void Awake()
    {
        var gameManager = GameObject.FindWithTag("BackGroundMusicManager");
        if (gameManager != null)
            backGroundMusicSource = gameManager.GetComponent<AudioSource>();

        savedVolume = PlayerPrefs.GetFloat("BackGroundMusicVolume", 1f);
    }

    private void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null) return;

        var root = uiDoc.rootVisualElement;
        backGroundSlider = root.Q<Slider>("BackGroundMusicSlider");
        if (backGroundSlider == null) return;

        backGroundSlider.SetValueWithoutNotify(savedVolume);
        backGroundSlider.RegisterValueChangedCallback(OnVolumeChanged);
    }

    private void OnDisable()
    {
        if (backGroundSlider != null)
            backGroundSlider.UnregisterValueChangedCallback(OnVolumeChanged);
    }

    private void OnVolumeChanged(ChangeEvent<float> evt)
    {
        savedVolume = evt.newValue;

        if (backGroundMusicSource != null)
            backGroundMusicSource.volume = savedVolume;

        PlayerPrefs.SetFloat("BackGroundMusicVolume", savedVolume);
        PlayerPrefs.Save();

        Debug.Log($"Background Music volume changed to {savedVolume}");
    }
}
