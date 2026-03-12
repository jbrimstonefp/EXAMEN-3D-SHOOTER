using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Main ghost enemy controller — runs a state machine (Patrol → Chase → Attack)
public class GhostAI : MonoBehaviour
{
    public Transform player;
    public Transform[] patrolNodes;
    public NavMeshAgent agent;
    public Animator animator;
    public Transform firePointLeft;
    public Transform firePointRight;
    public int health = 5;
    public float detectionRange = 15f;
    public float attackRange = 5f;
    public float lostSightTimeout = 5f;
    public LayerMask playerLayer;

    private IState currentState;
    private Renderer ghostRenderer;
    private Coroutine damagePopCoroutine;
    private Color originalColor;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ghostRenderer = GetComponentInChildren<Renderer>();
        originalColor = ghostRenderer.material.GetColor("_MainColor");
    }

    private void Start()
    {
        ChangeState(new PatrolState(this));
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
    }

    // Transitions from the current state to a new one
    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter();
    }

    // Returns a random patrol waypoint
    public Transform GetRandomPatrolNode()
    {
        int index = Random.Range(0, patrolNodes.Length);
        return patrolNodes[index];
    }

    // Checks if the player is within detection range
    public bool PlayerInRange()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= detectionRange;
    }

    // Checks if the player is within attack range
    public bool PlayerInAttackRange()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= attackRange;
    }

    // Raycasts from the ghost to the player to check for obstacles
    public bool HasLineOfSight()
    {
        Vector3 direction = player.position - transform.position;
        Ray ray = new Ray(transform.position + Vector3.up, direction);
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, detectionRange, ~0, QueryTriggerInteraction.Ignore);
        return hitSomething && hit.transform == player;
    }

    // Reduces health and kills the ghost when it reaches zero
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (damagePopCoroutine != null)
        {
            StopCoroutine(damagePopCoroutine);
            transform.localScale = Vector3.one;
        }
        damagePopCoroutine = StartCoroutine(DamagePopRoutine());
        if (health <= 0)
        {
            Die();
        }
    }

    // Quick scale pop effect when taking damage
    private IEnumerator DamagePopRoutine()
    {
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * 1.3f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale;
    }

    // Plays dissolve animation and returns to the pool
    private void Die()
    {
        animator.SetTrigger("dissolve");
        // Small delay before returning to pool so the dissolve animation can play
        Invoke("ReturnToPool", 1.5f);
    }

    private void ReturnToPool()
    {
        GhostPool.Instance.Push(gameObject);
    }

    // Fires a burst of 6 projectiles at the player
    public void FireBurst()
    {
        StartCoroutine(FireBurstRoutine());
    }

    private IEnumerator FireBurstRoutine()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 dirLeft = (player.position - firePointLeft.position).normalized;
            Vector3 dirRight = (player.position - firePointRight.position).normalized;
            EnemyProjectilePool.Instance.Get(firePointLeft.position, dirLeft);
            EnemyProjectilePool.Instance.Get(firePointRight.position, dirRight);
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Tints the ghost material to a red-pink hue (used during chase/attack)
    public void SetAggroColor()
    {
        ghostRenderer.material.SetColor("_MainColor", new Color(1f, 0.3f, 0.4f));
    }

    // Restores the ghost material to its original color
    public void ResetColor()
    {
        ghostRenderer.material.SetColor("_MainColor", originalColor);
    }

    // Called by GhostDetector when the player is inside the trigger area
    public void OnPlayerDetected()
    {
        // Detection is handled by the states via PlayerInRange + HasLineOfSight
        // This method exists as a hook for future trigger-based logic
    }

    // Detects player projectile hits (tagged "Projectile", not "EnemyProjectile")
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            TakeDamage(2);
        }
    }

    // Resets health when reactivated from the pool
    public void ResetGhost()
    {
        health = 5;
        currentState = null;
        ChangeState(new PatrolState(this));
    }
}
