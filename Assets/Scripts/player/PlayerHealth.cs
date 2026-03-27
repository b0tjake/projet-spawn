using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;
    private bool isGuarding = false; 

    [Header("References")]
    public HealthBar healthBar;
    public GameOverManager gameManager; // <--- (1) أضفنا هذا المتغير
    private playerMovement movementScript; 
    private Animator anim; 

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        movementScript = GetComponent<playerMovement>();
        anim = GetComponent<Animator>(); 

        if (healthBar != null) healthBar.SetMaxHealth(maxHealth);
        
        // (2) البحث التلقائي عن الماناجر إذا نسيت تركبه
        if (gameManager == null) 
            gameManager = FindFirstObjectByType<GameOverManager>();
    }

protected virtual void Update()
{
    if (isDead) return;

    if (movementScript.dontFall)
    {
        Die();
        currentHealth = 0;
        if (healthBar != null) healthBar.SetHealth(currentHealth);
    }
}

    public void SetGuard(bool value)
{
    if (isDead) return;

    isGuarding = value;
    anim.SetBool("isGuarding", value);

    if (value)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}
    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        if (isGuarding)
        {
            Debug.Log("Blocked!");
            return; 
        }

        if (movementScript != null && movementScript.isSpecialAttacking)
    {
        Debug.Log("Immune during Special Attack!");
        return;
    }

        currentHealth -= damageAmount;
        // Debug.Log("Damage received: " + damageAmount);

        if (healthBar != null) healthBar.SetHealth(currentHealth);

        anim.SetTrigger("Hurt");

        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Player died!");

        // 1. Stop Movement
        if (movementScript != null)
        {
            movementScript.enabled = false;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; 
        }

        // 2. Play Animation
        anim.SetBool("isDead", true);

        // 3. Trigger Game Over Screen (هذا هو السطر المهم جداً!)
        if (gameManager != null)
        {
            gameManager.TriggerGameOver();
        }
    }

    public float GetHealth() { return currentHealth; }
}