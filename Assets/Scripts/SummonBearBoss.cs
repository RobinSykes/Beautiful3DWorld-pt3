using UnityEngine;
using System.Collections;
using TMPro;

public class SummonBearBoss : MonoBehaviour
{
    public GameObject BearBoss; // Prefab of the boss
    public Transform spawnPoint; // Optional: assign a spawn location in the Inspector
    public CanvasGroup bossText; // UI text CanvasGroup for fade effects
    public CanvasGroup SummonBossText;
    public GameObject SummonBoss;
    public float spawnDelay = 5f; // Time before boss spawns

    private float fadeInDuration = 1f;   // Fade in 1 second
    private float visibleDuration = 2f;  // Stay visible for 2 seconds
    private float fadeOutDuration = 1f;  // Fade out 1 second

    public void SpawnBearBoss()
    {
        SummonBoss.SetActive(false);
        // Start fade sequence immediately
        if (bossText != null)
            StartCoroutine(FadeTextSequence());

        // Start boss spawn delay
        StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        // Spawn boss after delay
        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
        Instantiate(BearBoss, position, rotation);

        Debug.Log("Bear Boss has been summoned!");
    }

    private IEnumerator FadeTextSequence()
    {
        bossText.gameObject.SetActive(true);
        bossText.alpha = 0f;

        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            bossText.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        bossText.alpha = 1f;

        // Stay visible
        yield return new WaitForSeconds(visibleDuration);

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            bossText.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        bossText.alpha = 0f;

        bossText.gameObject.SetActive(false);
    }
    public void HideSummonBossText()
    {
        if (SummonBossText != null)
            StartCoroutine(FadeOutSummonBossText());
    }

    private IEnumerator FadeOutSummonBossText()
    {
        float duration = 5f;
        float startAlpha = SummonBossText.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            SummonBossText.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        SummonBossText.alpha = 0f;
    }


}
