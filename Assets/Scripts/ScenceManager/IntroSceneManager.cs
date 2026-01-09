using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroSceneManager : MonoBehaviour
{
    public GameObject pressAnyKeyText; // جر النص اللي صايبنا لهنا فـ Inspector
    private bool canStart = false;

    void Start()
    {
        // فالبداية النص كيكون طافي
        pressAnyKeyText.SetActive(false);
        // كانساينو 3 ثواني (مدة الأنميشن) عاد نشعلوه
        StartCoroutine(EnableStart());
    }

    IEnumerator EnableStart()
    {
        yield return new WaitForSeconds(3.0f); // نفس مدة الأنميشن ديالك
        pressAnyKeyText.SetActive(true);
        canStart = true;
    }

    void Update()
    {
        // إلا سالات الأنميشن واللاعب ورك على أي بوطون فـ الكلافي أو المانيطا
        if (canStart && Input.anyKeyDown)
        {
            LoadGameScene();
        }
    }

    void LoadGameScene()
    {
        // كيدوزك لـ Scene ديال اللعب اللي سميتها "01" أو "Game"
        SceneManager.LoadScene("01"); 
    }
}