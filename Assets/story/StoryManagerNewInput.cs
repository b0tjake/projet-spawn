using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class StoryManagerNewInput : MonoBehaviour
{
    public GameObject[] panels;
    private int currentIndex = 0;

    public AudioClip[] panelSounds;
    private AudioSource audioSource;

    private PlayerInputActions.PlayerInputActions inputActions;

    private Coroutine currentCoroutine; // 👈 باش نتحكمو فـ coroutine

    private void Awake()
    {
        inputActions = new PlayerInputActions.PlayerInputActions();
        audioSource = GetComponent<AudioSource>();

        // نخلي غير الأول باين
        for (int i = 0; i < panels.Length; i++)
            panels[i].SetActive(i == 0);

        PlaySound(0);
    }

    void PlaySound(int index)
    {
        if (index < panelSounds.Length)
        {
            audioSource.Stop();

            // 🛑 وقف coroutine قديم
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            audioSource.clip = panelSounds[index];
            audioSource.Play();

            // ▶️ start coroutine جديد
            currentCoroutine = StartCoroutine(WaitAudioForNext());
        }
    }

    IEnumerator WaitAudioForNext()
    {
        yield return new WaitWhile(() => audioSource.isPlaying);

        if (currentIndex < panels.Length - 1)
        {
            panels[currentIndex].SetActive(false);
            currentIndex++;
            panels[currentIndex].SetActive(true);

            PlaySound(currentIndex);
        }
        else
        {
            // آخر panel
            if (SceneManager.GetActiveScene().name == "xonekay")
                SceneManager.LoadScene("MainMenu");
            else
                SceneManager.LoadScene("0");
        }
    }

    private void OnEnable()
    {
        inputActions.UI.Enable();
        inputActions.UI.Next.started += OnNext;
        inputActions.UI.Back.started += OnBack;
    }

    private void OnDisable()
    {
        inputActions.UI.Next.started -= OnNext;
        inputActions.UI.Back.started -= OnBack;
        inputActions.UI.Disable();
    }

    private void OnNext(InputAction.CallbackContext context)
    {
        // 🛑 وقف coroutine باش ما يديرش auto
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        if (currentIndex >= panels.Length - 1)
        {
            if (SceneManager.GetActiveScene().name == "xonekay")
                SceneManager.LoadScene("MainMenu");
            else
                SceneManager.LoadScene("0");

            return;
        }

        panels[currentIndex].SetActive(false);
        currentIndex++;
        panels[currentIndex].SetActive(true);

        PlaySound(currentIndex);

        Debug.Log("Next panel: " + currentIndex);
    }

    private void OnBack(InputAction.CallbackContext context)
    {
        Debug.Log("BACK TRIGGERED");

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        if (currentIndex <= 0) return;

        panels[currentIndex].SetActive(false);
        currentIndex--;
        panels[currentIndex].SetActive(true);

        PlaySound(currentIndex);

        Debug.Log("Previous panel: " + currentIndex);
    }
}