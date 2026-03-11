using UnityEngine;
using UnityEngine.InputSystem;

// Handles firing the M4, consuming ammo from magazine, and reloading from reserve
public class WeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Transform cameraTransform;

    [Header("Ammo")]
    [SerializeField] private int currentMagazine;
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private int reserveAmmo = 60;

    [Header("Fire Rate")]
    [SerializeField] private float fireRate = 10f;

    private InputAction attackAction;
    private InputAction reloadAction;

    private float fireCooldown;

    private void Awake()
    {
        var playerMap = inputActions.FindActionMap("Player");
        attackAction = playerMap.FindAction("Attack");
        reloadAction = playerMap.FindAction("Reload");

        currentMagazine = magazineSize;
    }

    private void OnEnable()
    {
        attackAction.Enable();
        reloadAction.Enable();
    }

    private void OnDisable()
    {
        attackAction.Disable();
        reloadAction.Disable();
    }

    private void Update()
    {
        // Tick down the fire cooldown
        fireCooldown -= Time.deltaTime;

        // Hold to fire (automatic)
        if (attackAction.IsPressed() && fireCooldown <= 0f)
            Fire();

        // Reload with Interact key (E)
        if (reloadAction.WasPressedThisFrame())
            Reload();
    }

    // Fires a projectile from the muzzle in the camera's forward direction
    private void Fire()
    {
        if (currentMagazine > 0)
        {
            currentMagazine--;
            fireCooldown = 1f / fireRate;

            // Shoot in the direction the camera is looking
            ProjectilePool.Instance.Get(muzzlePoint.position, cameraTransform.forward);
        }
    }

    // Moves bullets from reserve into the magazine
    private void Reload()
    {
        int bulletsNeeded = magazineSize - currentMagazine;
        int bulletsToLoad = Mathf.Min(bulletsNeeded, reserveAmmo);

        currentMagazine += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;
    }

}
