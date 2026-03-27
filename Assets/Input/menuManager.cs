using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class menuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject firstSelectedButton;

    private bool isMenuOpen = false;

    void Start()
    {
        menuUI.SetActive(false);
        settingsUI.SetActive(false);
    }

    void Update()
    {
        if (inputManager.instance != null && inputManager.instance.menuOpensClose)
        {
            if (!isMenuOpen) OpenMenu();
            else CloseMenu();
        }
    }

    public void OpenMenu()
    {
        menuUI.SetActive(true);
        isMenuOpen = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    public void CloseMenu()
    {
        menuUI.SetActive(false);
        settingsUI.SetActive(false);
        isMenuOpen = false;
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        EventSystem.current.SetSelectedGameObject(null);
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