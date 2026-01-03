using UnityEngine;
using System.Collections;

public class CharacterEffects : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // اسحب صورة الشخصية هنا
    public Color damageColor = Color.red; // لون الوجع
    private Color originalColor;

    void Start()
    {
        // بنحفظ اللون الأصلي عشان نرجعله
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    // دي الدالة اللي هنناديها لما الشخصية تنضرب
    public void PlayDamageEffect()
    {
        StartCoroutine(FlashRoutine());
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        // بنزق الشخصية لفوق ولورا شوية
        Vector2 knockbackDir = new Vector2(-transform.localScale.x, 0.5f); 
        rb.AddForce(knockbackDir * 5f, ForceMode2D.Impulse);
    }
    }

    IEnumerator FlashRoutine()
    {
        spriteRenderer.color = damageColor; // قلب أحمر
        yield return new WaitForSeconds(0.1f); // استنى لحظة صغيرة
        spriteRenderer.color = originalColor; // ارجع طبيعي
    }
}