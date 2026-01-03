using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHealth(float health)
    {
        // هاد السطر كيتأكد واش السلايدر راكب قبل ما يخدم
        if (slider != null) 
        {
            slider.maxValue = health;
            slider.value = health;
        }
    }

    public void SetHealth(float health)
    {
        if (slider != null)
        {
            slider.value = health;
        }
        else 
        {
            Debug.LogError("عشيري، راه نسيتي ما جريتيش الـ Slider للخانة ديالو فـ HealthBarCanvas!");
        }
    }
}