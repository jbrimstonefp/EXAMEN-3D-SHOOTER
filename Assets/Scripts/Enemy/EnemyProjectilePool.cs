using System.Collections.Generic;
using UnityEngine;

// Separate pool for ghost projectiles tagged "EnemyProjectile"
public class EnemyProjectilePool : MonoBehaviour
{
    public static EnemyProjectilePool Instance { get; private set; }

    [SerializeField] private Projectile prefab;
    [SerializeField] private int poolSize = 50;

    private Stack<Projectile> available = new Stack<Projectile>();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            Projectile p = Instantiate(prefab, transform);
            p.gameObject.SetActive(false);
            available.Push(p);
        }
    }

    public void Get(Vector3 position, Vector3 direction)
    {
        if (available.Count > 0)
        {
            Projectile p = available.Pop();
            p.Launch(position, direction);
        }
    }

    public void Return(Projectile p)
    {
        available.Push(p);
    }
}
