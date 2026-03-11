using System.Collections.Generic;
using UnityEngine;

// Pre-instantiates a set of projectiles and recycles them to avoid runtime allocations
public class ProjectilePool : MonoBehaviour
{
    // Static singleton so any script can access the pool without a reference
    public static ProjectilePool Instance { get; private set; }

    [SerializeField] private Projectile prefab;
    [SerializeField] private int poolSize = 30;

    private Stack<Projectile> available = new Stack<Projectile>();

    private void Awake()
    {
        Instance = this;

        // Pre-fill the pool with inactive projectiles
        for (int i = 0; i < poolSize; i++)
        {
            Projectile p = Instantiate(prefab, transform);
            p.gameObject.SetActive(false);
            available.Push(p);
        }
    }

    // Gets a projectile from the pool, launches it at the given position and direction
    public Projectile Get(Vector3 position, Vector3 direction)
    {
        if (available.Count == 0)
        {
            return null;
        }

        Projectile p = available.Pop();
        p.Launch(position, direction);
        return p;
    }

    // Returns a projectile back to the pool for reuse
    public void Return(Projectile p)
    {
        available.Push(p);
    }
}
