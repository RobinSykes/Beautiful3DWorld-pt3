using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
//using UnityEngine.WSA;
//using ithappy.Animals_FREE;
using StarterAssets;

public class MonsterBearDefeat : MonoBehaviour
{
    public AudioSource bossBearDefeated;
    public float fadeInDuration = 2.5f;
    private Button endGameButton;
    private Button keepPlayingButton;
    public UIDocument uiDocument; // Assign in Inspector
    private VisualElement endGameMenu;
    public GameObject endgame;
    public Animator animator;
    private void OnEnable()
    {
        endgame.SetActive(true);
    }
    private void Start()
    {
        endGameMenu = uiDocument.rootVisualElement.Q<VisualElement>("EndGameMenu");

        // ? Get Buttons
        endGameButton = endGameMenu.Q<Button>("EndGameButton");
        keepPlayingButton = endGameMenu.Q<Button>("KeepPlayingButton");

        // ? Add Click Handlers
        if (endGameButton != null)
            endGameButton.clicked += OnEndGameClicked;

        if (keepPlayingButton != null)
            keepPlayingButton.clicked += OnKeepPlayingClicked;

        if (bossBearDefeated != null)
            StartCoroutine(FadeInAudio(bossBearDefeated, fadeInDuration));

        StartCoroutine(FadeInEndMenu(endGameMenu, fadeInDuration));
    }


    private IEnumerator FadeInAudio(AudioSource audioSource, float duration)
    {
        yield return new WaitForSeconds(2f);
        float targetVolume = audioSource.volume;
        audioSource.volume = 0f;
        audioSource.Play();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            audioSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeInEndMenu(VisualElement menu, float duration)
    {
        yield return new WaitForSeconds(3f);
        
        // Show menu first
        menu.style.display = DisplayStyle.Flex;
        menu.style.opacity = 0f;
        //enable cursor 
        UnityEngine.Cursor.visible = true;                       
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        //disable player movement so they cannot move during end screen
        var controller = FindFirstObjectByType<ThirdPersonController>();
        if (controller != null)
            controller.enabled = false;
        var animationController = FindFirstObjectByType<PlayerAnimationController>();
        if (animationController != null)
            animationController.enabled = false;

        if (animator != null)
            animator.SetFloat("Speed", 0f);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            menu.style.opacity = Mathf.Lerp(0f, 1f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        menu.style.opacity = 1f;
    }
    private void OnEndGameClicked()
    {
        Debug.Log("End Game Button Pressed");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        endgame.SetActive(false);
    }

    private void OnKeepPlayingClicked()
    {
        Debug.Log("Keep Playing Button Pressed");
        endGameMenu.style.display = DisplayStyle.None;

        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        var controller = FindFirstObjectByType<ThirdPersonController>();
        if (controller != null)
            controller.enabled = true;

        var animationController = FindFirstObjectByType<PlayerAnimationController>();
        if (animationController != null)
            animationController.enabled = true;
        endgame.SetActive(false);
        StartCoroutine(ReturnPlayerToOriginalPosition());
    }
    private IEnumerator ReturnPlayerToOriginalPosition()
    {
        var summon = FindFirstObjectByType<SummonBearBoss>();
        if (summon == null)
        {
            Debug.LogWarning("No SummonBearBoss found in scene.");
            yield break;
        }

        // Fade to black
        if (summon.blackoutCanvas != null)
            yield return StartCoroutine(summon.FadeCanvas(summon.blackoutCanvas, 0f, 1f, 0.5f));

        // Teleport back to stored location
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Disable movement controllers
            foreach (var mb in player.GetComponentsInChildren<MonoBehaviour>())
            {
                if (mb.GetType().Name.Contains("Controller") || mb.GetType().Name.Contains("Movement"))
                    mb.enabled = false;
            }
            foreach (var cc in player.GetComponentsInChildren<CharacterController>())
                cc.enabled = false;

            // ? Restore player original position and rotation
            player.transform.position = SummonBearBoss.lastPlayerPosition;
            player.transform.rotation = SummonBearBoss.lastPlayerRotation;

            // Re-enable controllers
            StartCoroutine(summon.ReenableControllers(player.transform));
        }

        // Fade from black
        if (summon.blackoutCanvas != null)
            yield return StartCoroutine(summon.FadeCanvas(summon.blackoutCanvas, 1f, 0f, 0.5f));

        // Hide end screen UI and lock cursor
        endGameMenu.style.display = DisplayStyle.None;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("Player returned to previous location successfully.");
    }



}
