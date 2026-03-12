using UnityEngine;

// Ghost stops, plays attack animation, and fires a burst of bullets at the player
public class AttackState : IState
{
    private GhostAI ghost;
    private float attackCooldown = 2f;
    private float attackTimer;

    public AttackState(GhostAI ghost)
    {
        this.ghost = ghost;
    }

    public void Enter()
    {
        ghost.agent.SetDestination(ghost.transform.position);
        ghost.animator.SetBool("isRunning", false);
        ghost.animator.SetTrigger("attack");
        ghost.FireBurst();
        attackTimer = 0f;
    }

    public void Execute()
    {
        // Rotate to face the player
        Vector3 direction = ghost.player.position - ghost.transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.01f)
        {
            ghost.transform.rotation = Quaternion.Slerp(
                ghost.transform.rotation,
                Quaternion.LookRotation(direction),
                10f * Time.deltaTime
            );
        }

        // Fire another burst on cooldown
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            ghost.animator.SetTrigger("attack");
            ghost.FireBurst();
        }

        // Player moved clearly out of attack range, chase them
        float distance = Vector3.Distance(ghost.transform.position, ghost.player.position);
        if (distance > ghost.attackRange + 2f)
        {
            ghost.ChangeState(new ChaseState(ghost));
        }
    }

    public void Exit()
    {
    }
}
