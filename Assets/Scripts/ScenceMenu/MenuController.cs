using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // دالة لفتح شاشة التحكم
    public void OpenControls()
    {
        SceneManager.LoadScene("ControlsScene");
    }

    // دالة لفتح شاشة الإعدادات
    public void OpenOptions()
    {
        SceneManager.LoadScene("OptionsScene");
    }

    // دالة للرجوع للمنيو الرئيسية
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // تأكد من سمية المنيو عندك
    }

    public void NewGame()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}