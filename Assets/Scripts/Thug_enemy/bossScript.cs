using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossController : EnemyBase 
{
    [Header("1. Boss Specifics")]
    public Slider bossHealthBar;       
    public Transform attackPoint;      

    [Header("2. Transformation")]
    public float transformDuration = 2.5f; 
    private bool hasStartedFight = false; 

    [Header("3. Boss Stats")]
    public float attackCooldown = 1.2f; 
    public float bossDamage = 40f;      
    public float attackRadius = 1.8f;   
    
    private float lastAttackTime;
    private float distanceToPlayer;
    private Rigidbody2D myRb; 
    private bool isDead = false;

    void Start()
    {
        base.Start(); 
        myRb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        
        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = currentHealth;
        }

        if (myRb != null)
        {
            myRb.mass = 50;
            myRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void Update()
    {
        if (isDead) return;

        if (player == null) return;

        // تحديث البار
        if (bossHealthBar != null) bossHealthBar.value = currentHealth;

        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // مرحلة التحول
        if (!hasStartedFight)
        {
            if (distanceToPlayer <= detectionRange) 
            {
                StartCoroutine(TransformRoutine());
            }
            return; 
        }

        // مرحلة القتال
        if (distanceToPlayer <= attackRange)
        {
            anim.SetBool("isMoving", false);
            if (Time.time > lastAttackTime + attackCooldown)
            {
                PerformAttack();
            }
        }
        else
        {
            MoveTowardsPlayer();
        }
    }


    public override void TakeDamage(float damage, Vector2 knockbackDir, float stunDuration)
    {
        base.TakeDamage(damage, knockbackDir, stunDuration);

        if (bossHealthBar != null) bossHealthBar.value = currentHealth;


        if (!isDead)
        {
            anim.SetTrigger("Hurt");
        }
    }

    protected override void Die()
    {
        isDead = true;

        if(myRb != null) 
        {
            myRb.linearVelocity = Vector2.zero;
            myRb.bodyType = RigidbodyType2D.Kinematic;
            GetComponent<Collider2D>().enabled = false; 
        }

        anim.SetTrigger("Die");

        // if(bossHealthBar != null) bossHealthBar.gameObject.SetActive(false);

        Destroy(gameObject, 5f);
    }


    void MoveTowardsPlayer()
    {
        if (isDead) return;

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);

        anim.SetBool("isMoving", true);
        
        Vector2 target = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    void PerformAttack()
    {
        if (isDead) return;
        lastAttackTime = Time.time;
        anim.SetTrigger("Attack"); 
    }

    IEnumerator TransformRoutine()
    {
        hasStartedFight = true; 
        if(myRb != null) myRb.linearVelocity = Vector2.zero;

        anim.SetTrigger("startFight");
        yield return new WaitForSeconds(transformDuration);
    }

    public void BossDealDamage()
    {
        if (isDead) return;

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius);
        foreach (Collider2D target in hitPlayer)
        {
            if (target.CompareTag("Player"))
            {
                PlayerHealth healthScript = target.GetComponent<PlayerHealth>();

                if(healthScript != null) 
                {
                    healthScript.TakeDamage(bossDamage);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange); 

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}