using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class BossController : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private bool isDead = false;
    private bool isAttacking = false;

    public GameObject magicProjectilePrefab; // Prefab del proyectil mágico
    public float health = 100f;
    public float attackRange = 2f;
    public float chaseRange = 10f;
    public float visionRange = 15f;
    public float attackDamage = 20f;
    public float maxHealth = 100f;
    private float currentHealth;

    private PlayerHealth playerHealth;
    private AudioSource audioSource;
    public AudioClip musicClip;

    public TextMeshProUGUI healthText;
    public string sceneToLoad = "WinScene";

    // Parámetros del Animator
    public bool isSpinningKick;
    public bool isDeadBool;
    public bool isMagicAttack;
    public bool isScreaming;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerHealth = player.GetComponent<PlayerHealth>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= visionRange)
        {
            if (distanceToPlayer <= chaseRange)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(musicClip);
                }

                healthText.enabled = true;

                if (distanceToPlayer > attackRange + 0.5f)
                {
                    ChasePlayer();
                }
                else if (distanceToPlayer <= attackRange)
                {
                    StopAndAttack();
                }
            }
        }
        else
        {
            StopChase();
        }

        UpdateHealthText();
    }

    private void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
        animator.SetBool("isRunning", true);
    }

    private void StopAndAttack()
    {
        agent.isStopped = true;
        animator.SetBool("isRunning", false);

        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("isPunching");
            StartCoroutine(AttackCooldown());
        }
    }

    private void StopChase()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        healthText.enabled = false;
        agent.isStopped = true;
        animator.SetBool("isRunning", false);
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        agent.isStopped = true;
        animator.SetBool("isDead", true);
        healthText.enabled = false;

        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        float deathAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(deathAnimationLength);
        SceneManager.LoadScene(sceneToLoad);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Attack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    private void UpdateHealthText()
    {
        healthText.text = "Health: " + currentHealth.ToString("F0");
    }

    // Métodos para activar los ataques
    public void PerformSpinningKick()
    {
        animator.SetTrigger("isSpinningKick");
        isSpinningKick = true; // Activamos la variable para el estado
    }

    public void PerformScreaming()
    {
        animator.SetTrigger("isScreaming");
        isScreaming = true; // Activamos la variable para el estado
    }

    public void PerformMagicAttack()
    {
        animator.SetTrigger("isMagicAttack");
        isMagicAttack = true; // Activamos la variable para el estado
    }

    // Para el ataque de patada giratoria
    public void SpinningKickAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            playerHealth.TakeDamage(attackDamage * 1.5f); // Daño de patada giratoria
        }
    }

    // Para el grito
    public void RoarAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange * 1.5f)
        {
            playerHealth.TakeDamage(attackDamage); // Daño por grito
        }
    }

    // Para el ataque mágico
    public void MagicAttack()
    {
        // Aquí puedes agregar lógica de proyectil de magia
        MagicProjectile magicProjectile = Instantiate(magicProjectilePrefab, transform.position, Quaternion.identity).GetComponent<MagicProjectile>();
        magicProjectile.SetDamage(attackDamage);
        // Aquí puedes definir la trayectoria del proyectil o su comportamiento
    }
}
