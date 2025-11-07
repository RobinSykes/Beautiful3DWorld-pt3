using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Background Music Settings")]
    [Tooltip("List of background music tracks to cycle through")]
    public List<AudioClip> musicTracks = new List<AudioClip>();

    [Tooltip("AudioSource that plays background music")]
    public AudioSource musicSource;

    [Tooltip("Crossfade duration (in seconds)")]
    public float crossfadeTime = 1.5f;

    [Header("Other Music Sources")]
    [Tooltip("Boss music source (if playing, background pauses)")]
    public AudioSource bossMusicSource;

    [Tooltip("Discovery music sources (if playing, background pauses)")]
    public List<AudioSource> discoveryMusicSources = new List<AudioSource>();

    private int currentTrackIndex = 0;
    private bool isFading = false;
    private bool isPausedForExternalMusic = false;
    private bool isBossActive = false;

    private const string VolumeKey = "BackGroundMusicVolume";

    private void Awake()
    {
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
            if (musicSource == null)
                musicSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.loop = false;
        musicSource.playOnAwake = false;

        // ? Apply saved volume immediately on startup
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        musicSource.volume = savedVolume;
        Debug.Log($"[BackgroundMusicManager] Loaded saved volume: {savedVolume}");
    }

    private void Start()
    {
        if (musicTracks.Count > 0)
            PlayTrack(currentTrackIndex);
        else
            Debug.LogWarning("BackgroundMusicManager: No background tracks assigned!");
    }

    private void Update()
    {
        // ? Stop everything if the boss is active
        if (isBossActive)
            return;

        // If any other music is active, pause background
        if (IsExternalMusicPlaying())
        {
            if (musicSource.isPlaying && !isPausedForExternalMusic)
            {
                StartCoroutine(FadeOutAndPause());
                isPausedForExternalMusic = true;
            }
            return;
        }

        // Resume background if it was paused and other music stopped
        if (!IsExternalMusicPlaying() && isPausedForExternalMusic)
        {
            isPausedForExternalMusic = false;
            StartCoroutine(FadeInAndResume());
        }

        // Cycle to next track when current finishes
        if (!musicSource.isPlaying && !isFading && !IsExternalMusicPlaying())
        {
            NextTrack();
        }
    }


    private bool IsExternalMusicPlaying()
    {
        if (bossMusicSource != null && bossMusicSource.isPlaying)
            return true;

        foreach (var discoverySource in discoveryMusicSources)
        {
            if (discoverySource != null && discoverySource.isPlaying)
                return true;
        }

        return false;
    }

    private void PlayTrack(int index)
    {
        if (index < 0 || index >= musicTracks.Count) return;

        musicSource.clip = musicTracks[index];
        musicSource.volume = 0f; // Start silent
        musicSource.Play();
        Debug.Log($"?? Playing background track: {musicTracks[index].name}");

        // Fade in over 2 seconds
        StartCoroutine(FadeInMusic(2f));
    }

    private IEnumerator FadeInMusic(float duration)
    {
        float targetVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, time / duration);
            yield return null;
        }

        musicSource.volume = targetVolume; // Ensure final volume is exact
    }


    private void NextTrack()
    {
        currentTrackIndex++;
        if (currentTrackIndex >= musicTracks.Count)
            currentTrackIndex = 0;

        StartCoroutine(CrossfadeToNext(musicTracks[currentTrackIndex]));
    }

    private IEnumerator CrossfadeToNext(AudioClip nextClip)
    {
        if (isFading) yield break;
        isFading = true;

        float startVolume = musicSource.volume;

        // Fade out
        for (float t = 0; t < crossfadeTime; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / crossfadeTime);
            yield return null;
        }
        musicSource.volume = 0f;

        // Switch to next track
        musicSource.clip = nextClip;
        musicSource.Play();

        // Fade back in
        for (float t = 0; t < crossfadeTime; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, startVolume, t / crossfadeTime);
            yield return null;
        }

        musicSource.volume = startVolume;
        isFading = false;
    }

    private IEnumerator FadeOutAndPause()
    {
        if (isFading) yield break;
        isFading = true;

        float startVolume = musicSource.volume;
        for (float t = 0; t < crossfadeTime; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / crossfadeTime);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Pause();
        isFading = false;
    }

    private IEnumerator FadeInAndResume()
    {
        if (isFading) yield break;
        isFading = true;

        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        musicSource.UnPause();

        for (float t = 0; t < crossfadeTime; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, savedVolume, t / crossfadeTime);
            yield return null;
        }

        musicSource.volume = savedVolume;
        isFading = false;
    }

    // ? Called by your volume slider (optional)
    public void ApplyVolume(float newVolume)
    {
        musicSource.volume = newVolume;
        PlayerPrefs.SetFloat(VolumeKey, newVolume);
        PlayerPrefs.Save();
        Debug.Log($"[BackgroundMusicManager] Volume updated: {newVolume}");
    }
    // ?? Instantly stop or fade out background music when the boss is summoned
    public void StopForBossSummon(bool fadeOut = true)
    {
        if (isBossActive)
            return; // already stopped

        isBossActive = true; // ?? Prevent restart

        if (musicSource.isPlaying)
            StartCoroutine(StopBackgroundMusicRoutine(fadeOut));
    }
    public void ResumeAfterBoss()
    {
        if (!isBossActive)
            return;

        isBossActive = false;
        Debug.Log("[BackgroundMusicManager] Resuming background music after boss fight.");

        StartCoroutine(FadeInAndResume());
    }


    private IEnumerator StopBackgroundMusicRoutine(bool fadeOut)
    {
        if (fadeOut)
        {
            float startVolume = musicSource.volume;
            float duration = crossfadeTime;

            for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                yield return null;
            }
        }

        musicSource.Stop();
        musicSource.volume = PlayerPrefs.GetFloat("BackGroundMusicVolume", 1f);
        Debug.Log("[BackgroundMusicManager] Background music stopped for boss summon.");
    }
}
