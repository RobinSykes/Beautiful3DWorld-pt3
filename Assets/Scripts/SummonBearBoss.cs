using UnityEngine;
using System.Collections;
using TMPro;
using StarterAssets;

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

    public static Vector3 lastPlayerPosition;
    public static Quaternion lastPlayerRotation;

    public void SpawnBearBoss()
    {
        Debug.Log("[SummonBearBoss] SpawnBearBoss triggered.");
        SummonBoss.SetActive(false);
        StartCoroutine(TeleportSequence());

        Object.FindFirstObjectByType<BackgroundMusicManager>()?.StopForBossSummon();
    }

    private IEnumerator TeleportSequence()
    {
        if (blackoutCanvas != null)
            yield return StartCoroutine(FadeCanvas(blackoutCanvas, 0f, 1f, 0.5f));

        yield return new WaitForSeconds(0.2f);

        TeleportPlayer();
        AlignCameraToPlayer();

        if (blackoutCanvas != null)
            yield return StartCoroutine(FadeCanvas(blackoutCanvas, 1f, 0f, 0.5f));

        if (bossText != null)
            StartCoroutine(FadeTextSequence());

        StartCoroutine(SpawnAfterDelay());
    }

    public IEnumerator FadeCanvas(CanvasGroup canvas, float from, float to, float duration)
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

        lastPlayerPosition = player.transform.position;
        lastPlayerRotation = player.transform.rotation;

        Transform playerRoot = player.transform;

        if (teleportPlayerArea == null)
        {
            Debug.LogWarning("[SummonBearBoss] No teleport destination assigned.");
            return;
        }

        // Disable player movement systems
        var tpc = playerRoot.GetComponentInChildren<ThirdPersonController>();
        if (tpc) tpc.enabled = false;

        var pac = playerRoot.GetComponentInChildren<PlayerAnimationController>();
        if (pac) pac.enabled = false;

        var pm = playerRoot.GetComponentInChildren<PlayerMovement>();
        if (pm) pm.enabled = false;

        // Assign player reference to spawners (without disabling them)
        EnemySpawner[] spawners =
            Object.FindObjectsByType<EnemySpawner>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        Transform playerTransform = player.transform;

        foreach (var spawner in spawners)
        {
            spawner.player = playerTransform;
        }

        Debug.Log($"[SummonBearBoss] Player BEFORE teleport: {playerRoot.position}");

        playerRoot.position = teleportPlayerArea.position;
        playerRoot.rotation = teleportPlayerArea.rotation;

        Debug.Log($"[SummonBearBoss] Player AFTER teleport: {playerRoot.position}");

        StartCoroutine(ReenableControllers(playerRoot));
    }

    public IEnumerator ReenableControllers(Transform playerRoot)
    {
        yield return new WaitForSeconds(0.1f);

        foreach (var cc in playerRoot.GetComponentsInChildren<CharacterController>())
            cc.enabled = true;

        var tpc = playerRoot.GetComponentInChildren<ThirdPersonController>();
        if (tpc) tpc.enabled = true;

        var pac = playerRoot.GetComponentInChildren<PlayerAnimationController>();
        if (pac) pac.enabled = true;

        var pm = playerRoot.GetComponentInChildren<PlayerMovement>();
        if (pm) pm.enabled = true;

        Debug.Log("[SummonBearBoss] Player re-enabled successfully after teleport.");
    }

    private void AlignCameraToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        mainCam.transform.rotation = Quaternion.LookRotation(player.transform.forward);

        Debug.Log("[SummonBearBoss] Camera aligned to player.");
    }

    private IEnumerator SpawnAfterDelay()
    {
        Debug.Log($"[SummonBearBoss] Waiting {spawnDelay} seconds before spawning boss...");
        yield return new WaitForSeconds(spawnDelay);

        Vector3 position = spawnPoint ? spawnPoint.position : transform.position;
        Quaternion rotation = spawnPoint ? spawnPoint.rotation : Quaternion.identity;

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

    public void OnBossDefeated()
    {
        var bgMusic = Object.FindFirstObjectByType<BackgroundMusicManager>();
        bgMusic?.ResumeAfterBoss();
    }
}
