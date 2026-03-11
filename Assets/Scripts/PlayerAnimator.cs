using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private WeaponController weaponController;

    private Animator animator;
    private bool isDead;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetBool("IsMoving", playerController.IsMoving);
        animator.SetBool("IsSprinting", playerController.IsSprinting);
        animator.SetBool("IsMovingBackward", playerController.IsMovingBackward);
        animator.SetBool("IsFiring", weaponController.IsFiring);
        animator.SetBool("IsFiringStatic", weaponController.IsFiring && !playerController.IsMoving);
        animator.SetBool("IsReloading", weaponController.IsReloading);
        animator.SetBool("IsDead", isDead);
    }

    public void SetDead()
    {
        isDead = true;
    }
}
