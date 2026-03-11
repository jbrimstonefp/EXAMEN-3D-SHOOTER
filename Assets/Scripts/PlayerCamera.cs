using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 70f;
    [SerializeField] private Transform cameraTarget;


    [SerializeField] private InputActionAsset inputActions;
    private InputAction lookAction;
    private float yaw;
    private float pitch = 15f;

    private void Awake()
    {
        var playerMap = inputActions.FindActionMap("Player");
        lookAction = playerMap.FindAction("Look");
    }

    private void OnEnable()
    {
        lookAction.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        lookAction.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraTarget.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
