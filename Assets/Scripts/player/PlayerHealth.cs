using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    [Header("References")]
    public HealthBar healthBar;          // link m3a ui slider script
    private playerMovement movementScript; // link m3a player movement script

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        // lga lplayer movement script
        movementScript = GetComponent<playerMovement>();

        // initialisation dyal health bar
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log("damage received: " + damageAmount);
        // updae health bar
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
            
            Debug.Log("Damage taken: " + damageAmount); 
        }

        // Play Hurt Animation
        // GetComponent<Animator>().SetTrigger("Hurt");

        Debug.Log("Player took damage! Health remaining: " + currentHealth);

        // moot
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        Debug.Log("Player died!");

        // ila mat lplayer myt7rkx
        if (movementScript != null)
        {
            movementScript.enabled = false;
            // wa9f sliding physics
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; 
        }

        // 2. Play Death Animation
        GetComponent<Animator>().SetBool("isDead", true);

        // 3. Disable had script
        this.enabled = false;
    }

    public float GetHealth()
    {
        return currentHealth;
    }
}