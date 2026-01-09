using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected float attackRange = 1.5f;

    protected Transform player;
    protected float currentHealth;
    private bool isStuned = false;
    protected bool isDead = false;
    protected Animator anim; 

    public enum EnemyState { Idle, Chase, Attack, Die }
    protected EnemyState currentState = EnemyState.Idle;

    [Header("UI Reference")]
    public HealthBar healthBar;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>(); 
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (healthBar != null) healthBar.SetMaxHealth(maxHealth);
    }

    protected virtual void Update()
    {
        if(isStuned) return;
        if (player == null) 
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
            return; 
        }

        if (isDead) return;

        // تصحيح: تجربة الموت بـ Space (زدنا اتجاه وقوة وهميين غير باش يخدم السطر)
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            TakeDamage(maxHealth, Vector2.zero, 0f); 
        }

        HandleStates();
    }

    protected virtual void HandleStates() { }

    // الدالة الرئيسية اللي كيعيط ليها اللاعب (فيها 3 ديال المتغيرات)
    public virtual void TakeDamage(float damageAmount, Vector2 knockbackDirection, float stuneDuration)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        if (healthBar != null) healthBar.SetHealth(currentHealth);

        // 1. تشغيل أنميشن الوجع
        if (anim != null) anim.SetTrigger("Hurt");

        // 2. تطبيق الـ Knockback
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        // if (rb != null && knockbackForce > 0)
        // {
        //     // كنمسحو السرعة القديمة باش الدفع يبان نقي
        //     rb.linearVelocity = Vector2.zero; 
        //     rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        // }


        if (gameObject.activeInHierarchy)
        {
            StopCoroutine("StuneCoroutine");
            StartCoroutine(StuneCoroutine(stuneDuration));
        }

        if (currentHealth <= 0) Die();

        
    }

    IEnumerator StuneCoroutine(float duration)
    {
        isStuned = true;
        yield return new WaitForSeconds(duration);
        isStuned = false;
    }

    // هادي دالة إضافية (Overload) باش إلا بغيتي تنقص ليه الدم بلا دفع (مثلا بسم أو نار)
    public virtual void TakeDamage(float damageAmount)
    {
        TakeDamage(damageAmount, Vector2.zero, 0f);
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

         anim.SetTrigger("Thug_Die"); 

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Static;

        if (healthBar != null) healthBar.gameObject.SetActive(false); // غبّر البارا فاش كيموت

        this.enabled = false; 
        Destroy(gameObject, 4f);
    }
}