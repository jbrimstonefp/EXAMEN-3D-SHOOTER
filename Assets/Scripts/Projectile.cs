using UnityEngine;

// Handles individual projectile movement and collision, returns itself to the pool when done
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxLifetime = 5f;

    public bool isEnemyProjectile;

    private float aliveTimer;

    // Called by the pool when this projectile is activated
    public void Launch(Vector3 position, Vector3 direction)
    {
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(direction);
        aliveTimer = 0f;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Return to pool after 5 seconds of flight
        aliveTimer += Time.deltaTime;
        if (aliveTimer >= maxLifetime)
        {
            ReturnToPool();
        }
    }

    // Return to pool on collision, skip other triggers (like detection zones)
    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);
        if (isEnemyProjectile)
        {
            EnemyProjectilePool.Instance.Return(this);
        }
        else
        {
            ProjectilePool.Instance.Return(this);
        }
    }
}
