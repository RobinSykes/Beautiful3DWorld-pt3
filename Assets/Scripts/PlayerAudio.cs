using UnityEngine;
using UnityEngine.Audio;

public class PlayerAudio : MonoBehaviour
{
     public AudioSource attackAudio;
     public AudioSource hitAudio;
     public AudioSource blockAudio;
    public AudioSource hitByBossAudio;
     public GameObject pauseMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AttackAudio()
    {
        // Prevent playing sound if game is paused or pause menu is open
        if (pauseMenu != null && pauseMenu.activeSelf)
        {
            Debug.Log("Attack sound blocked — game is paused.");
            return;
        }

        Debug.Log("Attack Sound");
        if (attackAudio != null)
        {
            attackAudio.pitch = Random.Range(0.8f, 1.2f);
            attackAudio.Play();
            Debug.Log($"Attack sound played with pitch {attackAudio.pitch}");
        }
    }

    public void HitAudio() 
    {
        if (hitAudio != null) 
        {
            
            hitAudio.pitch = Random.Range(0.8f, 1.2f);
            hitAudio.Play();
            Debug.Log($"Hit sound played with pihtc {hitAudio.pitch}");
        }
    }
    public void BlockAudio()
    {
        if (blockAudio != null)
        {
            blockAudio.pitch = Random.Range(0.8f, 1.2f);
            blockAudio.Play();
            Debug.Log($"Block sound played with pitch{blockAudio.pitch}");
        }
    }
    public void HitByBoss()
    {
        hitByBossAudio.pitch = Random.Range(0.8f, 1.2f);
        hitByBossAudio.Play();
        Debug.Log($"HitByBoss sound played with pitch{hitByBossAudio.pitch}");
    }
}
