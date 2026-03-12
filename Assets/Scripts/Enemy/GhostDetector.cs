using UnityEngine;

// Placed on a child object with a trigger collider to detect when the player enters the ghost's awareness area
public class GhostDetector : MonoBehaviour
{
    public GhostAI ghostAI;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ghostAI.OnPlayerDetected();
        }
    }
}
