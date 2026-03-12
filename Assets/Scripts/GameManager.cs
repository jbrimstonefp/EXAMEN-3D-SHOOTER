using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, Playing, Victory, Death }

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject hudPanel;
    public GameObject victoryPanel;
    public GameObject deathPanel;

    [Header("Player References")]
    public PlayerCamera playerCamera;
    public PlayerController playerController;
    public WeaponController weaponController;

    private GameState currentState;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ChangeState(GameState.Menu);
    }

    private void Update()
    {
        if (currentState == GameState.Playing)
        {
            if (GhostPool.Instance.ActiveGhostCount <= 0)
            {
                ChangeState(GameState.Victory);
            }
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        mainMenuPanel.SetActive(false);
        hudPanel.SetActive(false);
        victoryPanel.SetActive(false);
        deathPanel.SetActive(false);

        if (newState == GameState.Menu)
        {
            mainMenuPanel.SetActive(true);
            SetPlayerControls(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 1f;
        }
        else if (newState == GameState.Playing)
        {
            hudPanel.SetActive(true);
            SetPlayerControls(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
            GhostPool.Instance.ActivateGhosts();
        }
        else if (newState == GameState.Victory)
        {
            victoryPanel.SetActive(true);
            SetPlayerControls(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else if (newState == GameState.Death)
        {
            deathPanel.SetActive(true);
            SetPlayerControls(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }

    private void SetPlayerControls(bool enabled)
    {
        playerCamera.enabled = enabled;
        playerController.enabled = enabled;
        weaponController.enabled = enabled;
    }

    public void StartGame()
    {
        ChangeState(GameState.Playing);
    }

    public void OnPlayerDeath()
    {
        ChangeState(GameState.Death);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
