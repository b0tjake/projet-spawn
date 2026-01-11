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
        // فاش اللاعب يضغط على أي زر
        if (Input.anyKeyDown) 
        {
            // ديه للقائمة الرئيسية (MainMenu) اللي رقمها 1
            SceneManager.LoadScene(1); 
        }
    }
}