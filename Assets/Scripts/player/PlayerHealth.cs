using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;
    private bool isGuarding = false; // New variable to track state

    [Header("References")]
    public HealthBar healthBar;
    private playerMovement movementScript; 
    private Animator anim; // We need this cached

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        movementScript = GetComponent<playerMovement>();
        anim = GetComponent<Animator>(); // Cache animator

        if (healthBar != null) healthBar.SetMaxHealth(maxHealth);
        
        // Auto-find Game Manager
    }

    protected virtual void Update()
    {
        if (isDead) return;

        // 1. Start Guarding (Press Down)
        if (Input.GetKeyDown(KeyCode.H))
        {
            isGuarding = true;
            anim.SetBool("isGuarding", true); // Use BOOL, not Trigger
            
            // Optional: Stop movement while guarding?
            if(movementScript != null) movementScript.enabled = false;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        // 2. Stop Guarding (Release)
        if (Input.GetKeyUp(KeyCode.H))
        {
            isGuarding = false;
            anim.SetBool("isGuarding", false);

            // Re-enable movement
            if(movementScript != null) movementScript.enabled = true;
        }
    }

    // Updated TakeDamage to respect Guarding
    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        // If guarding, take reduced damage (or zero damage)
        if (isGuarding)
        {
            Debug.Log("Blocked!");
            // Optional: Play a "Block" sound effect here
            // damageAmount /= 2; // Example: Take half damage
            return; // Take NO damage
        }

        currentHealth -= damageAmount;
        Debug.Log("Damage received: " + damageAmount);

        if (healthBar != null) healthBar.SetHealth(currentHealth);

        anim.SetTrigger("Hurt");

        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Player died!");

        if (movementScript != null)
        {
            movementScript.enabled = false;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; 
        }

        anim.SetBool("isDead", true);

    }

    public float GetHealth() { return currentHealth; }
}