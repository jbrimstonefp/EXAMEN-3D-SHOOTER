using System.Collections.Generic;
using UnityEngine;

// Pre-instantiates ghost enemies and manages their lifecycle via object pooling
public class GhostPool : MonoBehaviour
{
    public static GhostPool Instance { get; private set; }

    public int poolSize = 7;
    public GameObject ghostPrefab;
    public Transform[] spawnPoints;
    public Transform[] patrolNodes;
    public Transform player;

    private Stack<GameObject> available = new Stack<GameObject>();

    private void Awake()
    {
        Instance = this;

        // Pre-instantiate all ghosts (disabled)
        for (int i = 0; i < poolSize; i++)
        {
            GameObject ghost = Instantiate(ghostPrefab, transform);
            ghost.SetActive(false);

            // Wire up shared references
            GhostAI ai = ghost.GetComponent<GhostAI>();
            ai.player = player;
            ai.patrolNodes = patrolNodes;

            available.Push(ghost);
        }

        // Activate all ghosts at random spawn points
        for (int i = 0; i < poolSize; i++)
        {
            if (available.Count > 0)
            {
                Pop();
            }
        }
    }

    // Activates a ghost from the pool at a random spawn point
    public GameObject Pop()
    {
        if (available.Count > 0)
        {
            GameObject ghost = available.Pop();
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            ghost.transform.position = spawnPoint.position;
            ghost.transform.rotation = spawnPoint.rotation;
            ghost.SetActive(true);

            GhostAI ai = ghost.GetComponent<GhostAI>();
            ai.ResetGhost();

            return ghost;
        }
        return null;
    }

    // Disables a ghost and returns it to the pool (no respawning)
    public void Push(GameObject ghost)
    {
        ghost.SetActive(false);
        // Ghost stays in the stack but won't be reactivated — once killed, it's gone
    }
}
