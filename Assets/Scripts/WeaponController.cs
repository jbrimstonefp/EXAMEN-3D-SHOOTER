using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Shooting")]
    [SerializeField] private float shootingRate = 10f;

    [Header("Reload")]
    [SerializeField] private float reloadDuration = 2.5f;

    public bool IsFiring;
    public bool IsReloading;

    public int CurrentMagazine { get { return currentMagazine; } }
    public int ReserveAmmo { get { return reserveAmmo; } }

    private InputAction attackAction;
    private InputAction reloadAction;

    private float shootingCooldown;
    private float reloadTimer;

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
        ShootingSpeed();
        ReloadTimer();
        Shooting();
        Reload();
    }

    private void ShootingSpeed()
    {
        shootingCooldown -= Time.deltaTime;
    }

    private void ReloadTimer()
    {
        if (IsReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
            {
                FinishReload();
            }
        }
    }

    private void Shooting()
    {
        if (attackAction.IsPressed() && shootingCooldown <= 0f && currentMagazine > 0 && !IsReloading)
        {
            IsFiring = true;
            Shoot();
        }
        else
        {
            IsFiring = false;
        }
    }

    private void Reload()
    {
        if (reloadAction.WasPressedThisFrame() && !IsReloading)
        {
            StartReload();
        }
    }

    private void Shoot()
    {
        currentMagazine--;
        shootingCooldown = 1f / shootingRate;

        ProjectilePool.Instance.Get(muzzlePoint.position, cameraTransform.forward);
    }

    private void StartReload()
    {
        int bulletsNeeded = magazineSize - currentMagazine;
        if (bulletsNeeded > 0 && reserveAmmo > 0)
        {
            IsReloading = true;
            reloadTimer = reloadDuration;
        }
    }

    private void FinishReload()
    {
        int bulletsNeeded = magazineSize - currentMagazine;
        int bulletsToLoad = Mathf.Min(bulletsNeeded, reserveAmmo);

        currentMagazine += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;
        IsReloading = false;
    }

}
