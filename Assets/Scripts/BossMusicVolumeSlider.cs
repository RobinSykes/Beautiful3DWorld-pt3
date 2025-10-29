using UnityEngine;
using UnityEngine.UIElements;

public class BossMusicVolumeSlider : MonoBehaviour
{
    private Slider bossMusicSlider;
    private AudioSource bossMusicSource;
    private float savedVolume = 1f;

    private void Awake()
    {
        var boss = GameObject.FindWithTag("Boss");
        if (boss != null)
            bossMusicSource = boss.GetComponent<AudioSource>();

        if (bossMusicSource == null)
            Debug.LogWarning("No AudioSource found on Boss!");
    }

    private void Start()
    {
        // Load saved volume and apply
        savedVolume = PlayerPrefs.GetFloat("BossMusicVolume", 1f);
        if (bossMusicSource != null)
            bossMusicSource.volume = savedVolume;
    }
    private void Update()
    {
        if (bossMusicSource == null)
        {
            // Try to find the boss again if it appeared later
            var boss = GameObject.FindWithTag("Boss");
            if (boss != null)
            {
                bossMusicSource = boss.GetComponent<AudioSource>();
                if (bossMusicSource != null)
                {
                    bossMusicSource.volume = savedVolume;
                    Debug.Log("BossMusicVolume reapplied to new AudioSource");
                }
            }
        }
        else if (!Mathf.Approximately(bossMusicSource.volume, savedVolume))
        {
            // Reapply saved volume if it got reset somehow
            bossMusicSource.volume = savedVolume;
        }
    }

    private void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null) return;

        var root = uiDoc.rootVisualElement;
        bossMusicSlider = root.Q<Slider>("BossMusicSlider");
        if (bossMusicSlider == null) return;

        bossMusicSlider.SetValueWithoutNotify(savedVolume);
        bossMusicSlider.RegisterValueChangedCallback(OnVolumeChanged);
    }

    private void OnDisable()
    {
        if (bossMusicSlider != null)
            bossMusicSlider.UnregisterValueChangedCallback(OnVolumeChanged);
    }

    private void OnVolumeChanged(ChangeEvent<float> evt)
    {
        savedVolume = evt.newValue;

        if (bossMusicSource != null)
            bossMusicSource.volume = savedVolume;

        PlayerPrefs.SetFloat("BossMusicVolume", savedVolume);
        PlayerPrefs.Save();

        Debug.Log($"BossMusicVolume changed to {savedVolume}");
    }
}
