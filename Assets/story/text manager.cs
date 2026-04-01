using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour
{
    public GameObject[] texts;
    private int currentIndex = 0;

    private PlayerInputActions.PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions.PlayerInputActions();

        // نخلي غير الأول باين
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].SetActive(i == 0);
        }
    }

    private void OnEnable()
    {
        inputActions.UI.Enable();
        inputActions.UI.Next.started += OnNext; // زر X ولا arrow
    }

    private void OnDisable()
    {
        inputActions.UI.Next.started -= OnNext;
        inputActions.UI.Disable();
    }

    private void OnNext(InputAction.CallbackContext context)
    {
        if (currentIndex < texts.Length - 1)
        {
            currentIndex++;
            texts[currentIndex].SetActive(true);
        }
        else
        {
            Debug.Log("Finished all texts, loading next scene...");
            SceneManager.LoadScene("0"); // غير "0" باسم المشهد اللي تبي تروح له
        }
    }
}