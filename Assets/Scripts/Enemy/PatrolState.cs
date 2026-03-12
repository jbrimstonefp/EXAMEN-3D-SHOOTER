using UnityEngine;

// Ghost patrols between random waypoints until it detects the player
public class PatrolState : IState
{
    private GhostAI ghost;

    public PatrolState(GhostAI ghost)
    {
        this.ghost = ghost;
    }

    public void Enter()
    {
        ghost.agent.speed = 2f;
        ghost.agent.stoppingDistance = 0.5f;
        ghost.animator.SetBool("isRunning", true);
        ghost.ResetColor();
        SetNewDestination();
    }

    public void Execute()
    {
        // Pick a new patrol node when close to the current one
        if (ghost.agent.remainingDistance <= 0.5f)
        {
            SetNewDestination();
        }

        // Transition to chase if the player is detected
        if (ghost.PlayerInRange() && ghost.HasLineOfSight())
        {
            ghost.ChangeState(new ChaseState(ghost));
        }
    }

    public void Exit()
    {
    }

    private void SetNewDestination()
    {
        Transform node = ghost.GetRandomPatrolNode();
        ghost.agent.SetDestination(node.position);
    }
}
