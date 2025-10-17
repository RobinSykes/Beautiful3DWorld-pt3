using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class YouDiedUIOverlay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup youDiedPanel;
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 2f;
    [Header("Main Menu Delay")]
    [SerializeField] private float mainMenuDelay = 5f;

    private bool hasShown = false;
    private float fadeTimer = 0f;

    // Add this method to explicitly trigger death
    public void TriggerYouDied()
    {
        if (hasShown) return;

        hasShown = true;
        fadeTimer = 0f;

        // Pause the game
        Time.timeScale = 0f;

        // Start fading
        StartCoroutine(FadeInAndReturnToMenu());
    }

    private IEnumerator FadeInAndReturnToMenu()
    {
        while (youDiedPanel.alpha < 1f)
        {
            fadeTimer += Time.unscaledDeltaTime;
            youDiedPanel.alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(mainMenuDelay);

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Load main menu
        SceneManager.LoadScene(0);
    }

    void Start()
    {
        if (youDiedPanel != null)
        {
            youDiedPanel.alpha = 0f;
            youDiedPanel.interactable = false;
            youDiedPanel.blocksRaycasts = false;
        }
    }
}
