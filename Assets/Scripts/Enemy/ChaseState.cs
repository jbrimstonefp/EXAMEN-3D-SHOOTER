using UnityEngine;

// Ghost pursues the player at run speed, continues to last known position if sight is lost
public class ChaseState : IState
{
    private GhostAI ghost;
    private float lostSightTimer;
    private Vector3 lastKnownPosition;
    private bool hasLineOfSight;

    public ChaseState(GhostAI ghost)
    {
        this.ghost = ghost;
    }

    public void Enter()
    {
        ghost.agent.speed = 4f;
        ghost.agent.stoppingDistance = ghost.attackRange;
        ghost.animator.SetTrigger("surprised");
        ghost.animator.SetBool("isRunning", true);
        ghost.SetAggroColor();
        lostSightTimer = 0f;
        hasLineOfSight = true;
        lastKnownPosition = ghost.player.position;
    }

    public void Execute()
    {
        // Close enough to attack
        if (ghost.PlayerInAttackRange() && ghost.HasLineOfSight())
        {
            ghost.ChangeState(new AttackState(ghost));
        }
        else if (ghost.PlayerInRange() && ghost.HasLineOfSight())
        {
            // Can see the player — chase directly
            hasLineOfSight = true;
            lostSightTimer = 0f;
            lastKnownPosition = ghost.player.position;
            ghost.agent.SetDestination(ghost.player.position);
        }
        else
        {
            // Lost sight — path to last known position
            hasLineOfSight = false;
            lostSightTimer += Time.deltaTime;
            ghost.agent.SetDestination(lastKnownPosition);

            // Give up after 5 seconds without line of sight
            if (lostSightTimer >= ghost.lostSightTimeout)
            {
                ghost.ChangeState(new PatrolState(ghost));
            }
        }
    }

    public void Exit()
    {
    }
}
