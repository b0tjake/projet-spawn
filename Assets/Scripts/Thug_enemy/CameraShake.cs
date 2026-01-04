using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // هاد السطر كيخلينا نعيطو للسكريب من أي بلاصة بلا ما نربطوه (Singleton)
    public static CameraShake instance;

    private Vector3 originalPos;

    void Awake()
    {
        instance = this;
    }

    // هادي هي الدالة اللي غانعيطو ليها باش تبدا الرعشة
    // duration: شحال المدة، magnitude: شحال القوة
    public void Shake(float duration, float magnitude)
    {
        originalPos = transform.localPosition;
        StopAllCoroutines(); // كنحبسو أي رعشة قديمة باش ما يتخلطوش
        StartCoroutine(DoShake(duration, magnitude));
    }

    IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // كنختارو بلاصة عشوائية قريبة بزاف من البلاصة الأصلية
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // كنحركو الكاميرا لهاديك البلاصة العشوائية
            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            // كنتسناو حتى للـ Frame الجاي
            yield return null;
        }

        // فاش كتسالي المدة، كنرجعو الكاميرا لبلاصتها الأصلية
        transform.localPosition = originalPos;
    }
}