using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuBackGroundMusic : MonoBehaviour
{
    private static MenuBackGroundMusic instance;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Prevent duplicates when returning to menu multiple times
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Fade out ONLY when entering "MainWorld"
        if (scene.name == "MainWorld")
        {
            StartCoroutine(FadeOutAndDestroy(1f));
        }
    }

    private IEnumerator FadeOutAndDestroy(float duration)
    {
        float startVolume = audioSource.volume;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
