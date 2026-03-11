using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public bool IsMoving;
    public bool IsSprinting;
    public bool IsMovingBackward;
    public bool IsGrounded;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.6f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private WeaponController weaponController;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private CharacterController controller;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        var playerMap = inputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        jumpAction = playerMap.FindAction("Jump");
        sprintAction = playerMap.FindAction("Sprint");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
    }

    private Vector2 moveInput;

    private void Update()
    {
        ReadInput();
        ComputeStateProperties();
        Rotation();
        Gravity();
        Jump();
        Movement();
    }

    private void ReadInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();
    }

    private void ComputeStateProperties()
    {
        IsGrounded = controller.isGrounded;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            IsMoving = true;
        }
        else
        {
            IsMoving = false;
        }

        if (sprintAction.IsPressed() && moveInput.y > 0f)
        {
            IsSprinting = true;
        }
        else
        {
            IsSprinting = false;
        }

        if (moveInput.y < -0.1f)
        {
            IsMovingBackward = true;
        }
        else
        {
            IsMovingBackward = false;
        }
    }

    private Vector3 GetMoveDirection()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * moveInput.y + right * moveInput.x;
        if (moveDir.sqrMagnitude > 1f)
        {
            moveDir.Normalize();
        }

        return moveDir;
    }

    private void Rotation()
    {
        Vector3 moveDir = GetMoveDirection();

        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else if (weaponController != null && weaponController.IsFiring)
        {
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();
            transform.rotation = Quaternion.LookRotation(cameraForward);
        }
    }

    private void Gravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
    }

    private void Jump()
    {
        if (jumpAction.WasPressedThisFrame() && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void Movement()
    {
        Vector3 moveDir = GetMoveDirection();

        if (IsSprinting)
        {
            controller.Move((moveDir * moveSpeed * sprintMultiplier + velocity) * Time.deltaTime);
        }
        else
        {
            controller.Move((moveDir * moveSpeed + velocity) * Time.deltaTime);
        }
    }
}

