using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup backgroundCanvasGroup; // الخلفية (GOP)
    public CanvasGroup textCanvasGroup;       // النص (You Died)

    [Header("Settings")]
    public float bgFadeDuration = 1.0f;   // سرعة ظهور الدم
    public float textFadeDuration = 1.0f; // سرعة ظهور النص
    public float waitBeforeRestart = 3f;  // الانتظار قبل الإعادة

    public void TriggerGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        // 0. البداية: تأكد أن كل شيء مخفي
        backgroundCanvasGroup.alpha = 0f;
        textCanvasGroup.alpha = 0f;
        backgroundCanvasGroup.gameObject.SetActive(true);

        // 1. انتظار صغير
        yield return new WaitForSeconds(0.5f);

        // ---------------------------------------------
        // المرحلة الأولى: ظهور الخلفية (الدم)
        // ---------------------------------------------
        float timer = 0f;
        while (timer < bgFadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            backgroundCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / bgFadeDuration);
            yield return null;
        }
        backgroundCanvasGroup.alpha = 1f;

        // ---------------------------------------------
        // المرحلة الثانية: ظهور النص (You Died)
        // ---------------------------------------------
        timer = 0f;
        while (timer < textFadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            textCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / textFadeDuration);
            yield return null;
        }
        textCanvasGroup.alpha = 1f;

        // ---------------------------------------------
        // المرحلة الثالثة: الانتظار وإعادة التشغيل
        // ---------------------------------------------
        yield return new WaitForSecondsRealtime(waitBeforeRestart);
        RestartGame();
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}