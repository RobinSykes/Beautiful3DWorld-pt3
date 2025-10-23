using UnityEngine;
using System.Collections;
using TMPro;

public class SummonBearBoss : MonoBehaviour
{
    public GameObject BearBoss;
    public Transform spawnPoint;
    public CanvasGroup bossText;
    public CanvasGroup SummonBossText;
    public GameObject SummonBoss;
    public Transform teleportPlayerArea;
    public CanvasGroup blackoutCanvas;
    public float spawnDelay = 5f;

    private float fadeInDuration = 1f;
    private float visibleDuration = 2f;
    private float fadeOutDuration = 1f;

    public void SpawnBearBoss()
    {
        Debug.Log("[SummonBearBoss] SpawnBearBoss triggered.");
        SummonBoss.SetActive(false);
        StartCoroutine(TeleportSequence());
    }

    private IEnumerator TeleportSequence()
    {
        // Fade to black
        if (blackoutCanvas != null)
        {
            yield return StartCoroutine(FadeCanvas(blackoutCanvas, 0f, 1f, 0.5f));
        }



        // Wait a moment for everything to settle
        yield return new WaitForSeconds(0.2f);
        // Teleport player
        Debug.Log("[SummonBearBoss] Calling TeleportPlayer()...");
        TeleportPlayer();
        // Rotate camera to player forward
        AlignCameraToPlayer();

        // Fade back in
        if (blackoutCanvas != null)
        {
            yield return StartCoroutine(FadeCanvas(blackoutCanvas, 1f, 0f, 0.5f));
        }

        // Text sequence + boss spawn
        if (bossText != null)
            StartCoroutine(FadeTextSequence());

        StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator FadeCanvas(CanvasGroup canvas, float from, float to, float duration)
    {
        float elapsed = 0f;
        canvas.gameObject.SetActive(true);
        canvas.alpha = from;

        while (elapsed < duration)
        {
            canvas.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvas.alpha = to;

        // Hide when fully transparent
        if (to <= 0f)
            canvas.gameObject.SetActive(false);
    }

    public void TeleportPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[SummonBearBoss] No player found with tag 'Player'.");
            return;
        }

        Transform playerRoot = player.transform;
        if (teleportPlayerArea == null)
        {
            Debug.LogWarning("[SummonBearBoss] No teleport destination assigned.");
            return;
        }

        // Disable movement + controllers on all child components
        foreach (var mb in playerRoot.GetComponentsInChildren<MonoBehaviour>())
        {
            if (mb.GetType().Name.Contains("Controller") || mb.GetType().Name.Contains("Movement"))
                mb.enabled = false;
        }

        foreach (var cc in playerRoot.GetComponentsInChildren<CharacterController>())
            cc.enabled = false;

        Debug.Log($"[SummonBearBoss] Player position BEFORE teleport: {playerRoot.position}");
        playerRoot.position = teleportPlayerArea.position;
        playerRoot.rotation = teleportPlayerArea.rotation;
        Debug.Log($"[SummonBearBoss] Player position AFTER teleport: {playerRoot.position}");

        StartCoroutine(ReenableControllers(playerRoot));
    }

    private IEnumerator ReenableControllers(Transform playerRoot)
    {
        yield return new WaitForSeconds(0.1f);

        foreach (var cc in playerRoot.GetComponentsInChildren<CharacterController>())
            cc.enabled = true;

        foreach (var mb in playerRoot.GetComponentsInChildren<MonoBehaviour>())
        {
            if (mb.GetType().Name.Contains("Controller") || mb.GetType().Name.Contains("Movement"))
                mb.enabled = true;
        }

        Debug.Log("[SummonBearBoss] Player re-enabled successfully after teleport.");
    }

    private void AlignCameraToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        // Smoothly align camera to player’s forward
        mainCam.transform.rotation = Quaternion.Lerp(
            mainCam.transform.rotation,
            Quaternion.LookRotation(player.transform.forward),
            1f
        );

        Debug.Log("[SummonBearBoss] Camera aligned to player forward direction.");
    }

    private IEnumerator SpawnAfterDelay()
    {
        Debug.Log($"[SummonBearBoss] Waiting {spawnDelay} seconds before spawning boss...");
        yield return new WaitForSeconds(spawnDelay);

        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        Instantiate(BearBoss, position, rotation);
        Debug.Log($"[SummonBearBoss] Bear Boss spawned at {position}");
    }

    private IEnumerator FadeTextSequence()
    {
        bossText.gameObject.SetActive(true);
        bossText.alpha = 0f;

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            bossText.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        bossText.alpha = 1f;
        yield return new WaitForSeconds(visibleDuration);

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
