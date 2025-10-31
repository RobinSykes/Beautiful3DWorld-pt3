using System.Collections;
using Unity.Behavior;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public float Health = 100f;
    public float Maxhealth = 100f;

    [Header("References")]
    public Animator animator;
    public FloatingHealthBar HealthBar;
    public ParticleSystem skeletonHit, villagerHit, bossHit;
    public AudioSource deathAudio, bossAudio;

    private BehaviorGraphAgent behaviorGraph;
    private CapsuleCollider capsuleCollider;
    private bool isDead;

    public bool IsDead => isDead;

    private void Awake()
    {
        HealthBar = GetComponentInChildren<FloatingHealthBar>();
        animator = GetComponentInChildren<Animator>();
        behaviorGraph = GetComponent<BehaviorGraphAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        deathAudio = GetComponentInChildren<AudioSource>();
    }

    private void Start()
    {
        HealthBar?.UpdateHealthBar(Health, Maxhealth);
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        // ?? Block check
        var playerController = FindFirstObjectByType<PlayerAnimationController>();
        if (playerController != null && playerController.IsBlocking())
        {
            GetComponent<PlayerAudio>()?.BlockAudio();
            Debug.Log($"{gameObject.name} tried to deal damage, but player is blocking!");
            return;
        }

        // ?? Hit effects by tag
        switch (tag)
        {
            case "Player":
                GetComponent<PlayerAnimationController>()?.PlayBloodParticle();
                break;
            case "Villager":
                villagerHit?.Play();
                break;
            case "Enemy":
                skeletonHit?.Play();
                break;
            case "Boss":
                bossHit?.Play();
                break;
        }

        // ?? Damage
        Health -= damageAmount;
        HealthBar?.UpdateHealthBar(Health, Maxhealth);

        // ?? Player hit feedback
        if (CompareTag("Player"))
        {
            var playerAudio = GetComponent<PlayerAudio>();
            playerAudio?.HitAudio();

            GameObject boss = GameObject.FindWithTag("Boss");
            if (boss && Vector3.Distance(transform.position, boss.transform.position) <= 20f)
            {
                playerAudio?.HitByBoss();
                Debug.Log("Player hit by boss (within 20f).");
            }
        }

        if (Health <= 0) Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        KillManager.Instance?.AddKill(transform.position);
        capsuleCollider.enabled = false;

        animator.SetInteger("DeathIndex", Random.Range(0, 3));
        Debug.Log($"{gameObject.name} has died.");

        HealthBar?.gameObject.SetActive(false);
        if (behaviorGraph) behaviorGraph.enabled = false;


        switch (tag)
        {
            case "Villager":
            case "Enemy":
                PlayDeathAudio();
                animator.SetTrigger("IsDead");
                StartCoroutine(HandleDeathSequence());
                break;

            case "Player":
                PlayDeathAudio();
                animator.SetTrigger("IsDead");
                animator.SetBool("Dead", true);
                StartCoroutine(SinkPlayerIntoGround(2f, 2f));
                break;

            case "Boss":
                animator.SetTrigger("IsDead");
                if (bossAudio) StartCoroutine(FadeOutBossAudio(bossAudio, 1.5f));
                EnableBearDefeat();
                StartCoroutine(HandleDeathSequence(2f));
                break;
        }
        gameObject.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    private void PlayDeathAudio()
    {
        if (!deathAudio) return;
        deathAudio.pitch = Random.Range(0.8f, 1.2f);
        deathAudio.Play();
        Debug.Log($"Death audio played with pitch {deathAudio.pitch}");
    }

    private void EnableBearDefeat()
    {
        var gm = GameObject.FindWithTag("GameManager");
        if (gm != null)
        {
            var bearDefeat = gm.GetComponent<MonsterBearDefeat>();
            if (bearDefeat != null)
            {
                bearDefeat.enabled = true;  // ? assign, not call as method
                Debug.Log("MonsterBearDefeat enabled.");
            }
        }
    }


    private IEnumerator FadeOutBossAudio(AudioSource source, float duration)
    {
        float startVol = source.volume, time = 0f;
        while (time < duration)
        {
            if (!source) yield break;
            source.volume = Mathf.Lerp(startVol, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        source.Stop();
        yield return new WaitForSeconds(1f);
        FindFirstObjectByType<SummonBearBoss>()?.OnBossDefeated();
    }

    private IEnumerator HandleDeathSequence(float extraDelay = 0f)
    {
        yield return new WaitForSeconds(3f + extraDelay);
        yield return SinkIntoGround(2f, 2f);
        yield return new WaitForSeconds(7f);
        Destroy(gameObject);
    }

    private IEnumerator SinkIntoGround(float duration, float distance)
    {
        float time = 0f;
        Vector3 start = transform.position, end = start - new Vector3(0, distance, 0);
        while (time < duration)
        {
            transform.position = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
        Destroy(gameObject);
    }

    private IEnumerator SinkPlayerIntoGround(float duration, float distance)
    {
        yield return new WaitForSeconds(3f);
        yield return SinkIntoGround(duration, distance);
    }
}
