using UnityEngine;
using UnityEngine.SceneManagement;

public class menuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject settingsUI;
    
    private bool isMenuOpen = false;

    void Start()
    {
        menuUI.SetActive(false);
        settingsUI.SetActive(false);
    }

    void Update()
    {
        // Use a null check to prevent errors
        if (inputManager.instance != null && inputManager.instance.menuOpensClose)
        {
            if(!isMenuOpen) OpenMenu();
            else CloseMenu();
        }
    }

    public void OpenMenu()
    {
        menuUI.SetActive(true);
        isMenuOpen = true;
        // Time.timeScale = 0f;

        // Unlock cursor so you can click the Spawn buttons
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseMenu()
    {
        menuUI.SetActive(false);
        settingsUI.SetActive(false);
        isMenuOpen = false;
        // Time.timeScale = 1f; // Resume the game

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnSettingsButton()
    {
                Time.timeScale = 1f;
                Scene activeScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(activeScene.name);
    }

    public void OnResumeButton()
    {
        CloseMenu();
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}