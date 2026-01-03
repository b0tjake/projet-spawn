using UnityEngine;

public class ThugEnemy : EnemyBase
{
    [Header("Combat Settings")]
    [SerializeField] private float timeBetweenAttacks = 2f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float damage = 10f;

    private float attackCooldown = 0f;

    protected override void HandleStates()
    {
        // 1. أول حاجة كنشوفو واش مات
        if (isDead) return;

        // 2. كنحسبو المسافة مرة وحدة فقط
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool isMoving = false; 

        switch (currentState)
        {
            case EnemyState.Idle:
                if (distanceToPlayer < detectionRange) currentState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                MoveTowardsPlayer();
                isMoving = true;
                if (distanceToPlayer <= attackRange) currentState = EnemyState.Attack;
                else if (distanceToPlayer > detectionRange * 1.5f) currentState = EnemyState.Idle;
                break;

            case EnemyState.Attack:
                AttackBehavior(distanceToPlayer);
                break;
        }

        FacePlayer();
        
        if(anim != null) anim.SetBool("IsMoving", isMoving);
    }

    void MoveTowardsPlayer()
    {
        if (currentState == EnemyState.Attack) return;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    void AttackBehavior(float dist)
    {
        if (dist > attackRange) { currentState = EnemyState.Chase; return; }

        attackCooldown -= Time.deltaTime; // كنقصو الوقت

        if (attackCooldown <= 0) // عاد كنهجمو
        {
            PerformAttack();
            attackCooldown = timeBetweenAttacks; // كنرجعو الوقت لـ 2 ثواني
        }
    }

    void PerformAttack()
    {
        if(anim != null) anim.SetTrigger("Attack");

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) 
            {
                ph.TakeDamage(damage);
            }
        }
    }

    void FacePlayer()
    {
        if (player.position.x > transform.position.x) transform.rotation = Quaternion.Euler(0, 0, 0);
        else transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}