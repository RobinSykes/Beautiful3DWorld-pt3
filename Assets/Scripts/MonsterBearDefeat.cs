using UnityEngine;
using System.Collections;

public class MonsterBearDefeat : MonoBehaviour
{
    public AudioSource bossBearDefeated;
    public float fadeInDuration = 2.5f;

    private void Start()
    {
        if (bossBearDefeated != null)
            StartCoroutine(FadeInAudio(bossBearDefeated, fadeInDuration));
        else
            Debug.LogWarning("No AudioSource assigned for bossBearDefeated!");
    }

    private IEnumerator FadeInAudio(AudioSource audioSource, float duration)
    {
        yield return new WaitForSeconds(2f);
        float targetVolume = audioSource.volume; // store the final volume
        audioSource.volume = 0f; // start silent
        audioSource.Play();

        float elapsed = 0f;

        while (elapsed < duration)
        {
            audioSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume; // ensure final volume is exact
    }
}
