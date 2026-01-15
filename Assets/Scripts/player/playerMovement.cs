using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour
{
    #region Variables - Settings
    [Header("Combat Settings")]
    public Transform attackPoint;
    public float attackRange = 1.53f;
    public LayerMask enemyLayers;
    public float damage = 20f;
    public float stuneDuration = 0.5f;

    public float attackRate = 0.24f;
    public float knockbackForce = 5f;
    public float comboDelay = 1.0f;
    
    [Header("Audio Settings")]
    public AudioClip hitSound;       // جر صوة الدربة لهنا فـ Unity
    private AudioSource audioSource; // هادا غانجيبوه أوتوماتيكيا

    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 5f;
    private Coroutine jumpCoroutine;
    [SerializeField] private GameObject playerAttackBox;
    #endregion

    #region Variables - Private State
    private Rigidbody2D rb;
    private Animator anim;
    private float originalScaleX;
    private float originalScaleY;
    private bool grounded = false;
    private bool isCrouching = false;
    private bool isSpecialAttacking = false;
    
    private int comboStep = 0;
    private float comboTimer = 0f;
    private float nextAttackTime = 0f;
    #endregion
    
    
    #region fly kick settings
    private float flyKickSpeed = 7f;
    private float flyKickDuration = 0.5f;
    private bool isFlyKicking = false;
    private float defaultGravityScale;

    #endregion

    private float rageValue = 0f;

[Header("Rage UI")]
public Image[] rageSkulls;
public float maxRage = 4.0f;

    #region Unity Callbacks
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // كنجيبو الـ Component ديال الصوت
        
        originalScaleX = transform.localScale.x;
        originalScaleY = transform.localScale.y;
        defaultGravityScale = rb.gravityScale;
        
        if (playerAttackBox != null) playerAttackBox.SetActive(false);
    }

    void Update()
    {
        HandleCooldowns();
        HandleInput();

        if(isFlyKicking) return;

        if (!grounded && rb.gravityScale == defaultGravityScale) 
        {
            anim.SetBool("jump", true);
        }
        // إلا كنا في الأرض، نحيدو Jump
        if (grounded) anim.SetBool("jump", false);
    }
    #endregion

    #region Input & Logic Handling
    private void HandleInput()
    {
        if(isFlyKicking) return;
        // 1. Crouch Logic
        isCrouching = Input.GetKey(KeyCode.S);
        anim.SetBool("crouching", isCrouching);

        // 2. Movement & Flip
        float move = Input.GetAxisRaw("Horizontal");
        Move(move);
        if (move != 0) Flip(move);

        // 3. Jump
        if (Input.GetKeyDown(KeyCode.W) && grounded) {
            jumpCoroutine = StartCoroutine(Jump());
        }

        // 4. Attack
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.K) && grounded) Attack("kickAttack");
            else if (Input.GetKeyDown(KeyCode.J) && grounded) Attack("attack");
        }

        if(!grounded && !isFlyKicking && Input.GetKeyDown(KeyCode.K))
        {

          StartCoroutine(performFlyingKick());
        }
        //special attack 1
        if(Input.GetKeyDown(KeyCode.O) && rageValue >= 1.0f)
        {
            StartCoroutine(SpecialAttackInput());
        }

        // Update Animations
        anim.SetBool("moving", move != 0);
    }
IEnumerator performFlyingKick()
{
    isFlyKicking = true;
    grounded = true;

    // Force animation state
    anim.SetBool("flyKick", true);
    anim.SetBool("jump", false);
    anim.SetBool("moving", false);
    anim.SetBool("crouching", false);

    rb.gravityScale = 0;
    rb.linearVelocity = Vector2.zero;

    float direction = transform.localScale.x > 0 ? 1 : -1;
    rb.linearVelocity = new Vector2(direction * flyKickSpeed, -2);
    attackRange = 2.3f; 

    if (playerAttackBox != null)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {

            StartCoroutine(ApplyDamage(enemy));
        }
        playerAttackBox.SetActive(true);
        }
        

    // IMPORTANT — use realtime so HitStop doesn't shrink duration
    yield return new WaitForSecondsRealtime(flyKickDuration);

    stopFlyingKick();
}
private void stopFlyingKick()
{
    if (!isFlyKicking) return;

    isFlyKicking = false;
    // grounded = false;

    anim.SetBool("flyKick", false);
    anim.SetBool("glide", true);

    rb.gravityScale = defaultGravityScale;
    attackRange = 1.53f;

    // give a small fall push
    rb.linearVelocity = new Vector2(0, -2f);

    if (playerAttackBox != null)
        playerAttackBox.SetActive(false);
}


    private void HandleCooldowns()
    {
        if (comboTimer > 0) comboTimer -= Time.deltaTime;
        else comboStep = 0;
    }
    #endregion

    #region Actions (Move, Jump, Attack)
    private void Move(float dir)
    {
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
    }

    IEnumerator Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        grounded = false; 
        yield return new WaitForSeconds(1f);
        if(grounded) yield break;
        rb.gravityScale = 0.5f;
        anim.SetBool("jump", false);
        anim.SetBool("glide", true);

    }

    private void Flip(float dir)
    {
        float x = (dir > 0) ? Mathf.Abs(originalScaleX) : -Mathf.Abs(originalScaleX);
        transform.localScale = new Vector3(x, originalScaleY, 1);
    }

    private void Attack(string attackPrefix)
    {
        // 1. Animation Logic
        ProcessAttackAnimation(attackPrefix);

        // 2. Hit Detection
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {

            StartCoroutine(ApplyDamage(enemy));
        }

        // 3. Post-Attack Timer
        comboTimer = comboDelay;
        StopCoroutine("DisableHitbox");
        StartCoroutine(DisableHitbox());
    }


    #endregion

    #region Combat Helpers
    private void ProcessAttackAnimation(string attackPrefix)
    {
        if (isCrouching && grounded)
        {
            string crouchAnim = (attackPrefix == "kickAttack") ? "crouchKickAttack" : "crouchAttack";
            anim.Play(crouchAnim, -1, 0f);
            comboStep = 0;
            nextAttackTime = Time.time + attackRate + 0.3f;
        }
        else
        {
            comboStep++;
            if (comboStep > 5) comboStep = 1;
            anim.Play(attackPrefix + comboStep, -1, 0f);
            nextAttackTime = Time.time + (comboStep == 5 ? attackRate + 0.5f : attackRate);
        }
    }

        
     IEnumerator ApplyDamage(Collider2D enemy)
    {
        EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();

        if (enemyScript != null)
        {
            
            // 1. حساب الدم والدفع
            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
            enemyScript.TakeDamage(damage, knockbackDir, stuneDuration);

            // 2. تفعيل الصوت (مع شوية تنويع فـ Pitch باش ما يملش اللاعب)
            if (audioSource != null && hitSound != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f); 
                audioSource.PlayOneShot(hitSound);
            }

            // 3. التأثيرات البصرية (HitStop + Camera Shake)
            StartCoroutine(HitStop(0.08f));
            if (CameraShake.instance != null) CameraShake.instance.Shake(0.05f, 0.2f);
            
            Debug.Log($"Hit: {enemy.name}");

            yield return new WaitForSeconds(0.1f);

        }
    }
    #endregion

    public void IncreaseRage(float amount)
    {
        rageValue += amount;
        rageValue = Mathf.Clamp(rageValue, 0f, maxRage);
        UpdateRageUI();
        Debug.Log($"Rage increased by {amount}. Current Rage: {rageValue}");
    }
private void UpdateRageUI()
{
    // 1. Calculate our current "Step" index
    // 0.25 -> index 0 | 1.0 -> index 3 | 1.25 -> index 4, etc.
    int currentIndex = Mathf.FloorToInt(rageValue / 0.25f) - 1;

    for (int i = 0; i < rageSkulls.Length; i++)
    {
        // A: Is this a "Full Skull" milestone? (Indices 3, 7, 11, 15)
        bool isMilestone = (i + 1) % 4 == 0;
        
        // B: Have we passed this milestone?
        float milestoneValue = (i + 1) * 0.25f;
        bool milestoneComplete = rageValue >= milestoneValue;

        // LOGIC:
        // Show the image if:
        // - It is EXACTLY the current progress (currentIndex)
        // - OR it is a Milestone skull that we have already finished
        if (i == currentIndex || (isMilestone && milestoneComplete))
        {
            rageSkulls[i].enabled = true;
        }
        else
        {
            rageSkulls[i].enabled = false;
        }
    }
}        
    #region special Attacks;
    IEnumerator SpecialAttackInput(){
        {
            isSpecialAttacking = true;
            anim.SetBool("spAttack", true); 
            anim.SetBool("specialAttack1", true);            
            // damage = 50f;

            // Reduce rage
            IncreaseRage(-1.0f);
            yield return new WaitForSeconds(1.0f);
            anim.SetBool("spAttack", false);
                anim.SetBool("specialAttack1", false);            
    // damage = 20f;
        }
    }
    

public void ExecuteBombExplosionHit()
{
    if (CameraShake.instance != null) CameraShake.instance.Shake(0.15f, 0.4f);
    

    float explosionRadius = 5f; 
    float explosionDamage = 50f;

    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayers);

    foreach (Collider2D enemy in hitEnemies)
    {
        EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
        if (enemyScript != null)
        {
            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
            enemyScript.TakeDamage(explosionDamage, knockbackDir, 1.0f);
        }
    }
    
    Debug.Log("Special Attack Explosion Triggered!");
}
    #endregion
    

    #region Coroutines & Gizmos
    IEnumerator DisableHitbox()
    {
        if (playerAttackBox != null) playerAttackBox.SetActive(true);
        if (!isFlyKicking)
        {
        yield return new WaitForSeconds(0.2f);
        if (playerAttackBox != null) playerAttackBox.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(flyKickDuration);
            if (playerAttackBox != null) playerAttackBox.SetActive(false);
        }
    }

    IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) grounded = true;
        rb.gravityScale = defaultGravityScale;
        anim.SetBool("glide", false);
        anim.SetBool("jump", false);

        if(jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    #endregion


    private void OnTriggerEnter2D(Collider2D other)
    {
        // الجاسوس رقم 1: هل لمسنا شيئاً أصلاً؟
        Debug.Log("🔥 Touch! Something entered the trigger: " + other.name);

        if (isFlyKicking)
        {
            // الجاسوس رقم 2: هل نحن في حالة طيران؟
            Debug.Log("✈️ Checking if object is enemy...");

            // التحقق من الطبقة أو التاج
            if (((1 << other.gameObject.layer) & enemyLayers) != 0 || other.CompareTag("Enemy"))
            {
                // الجاسوس رقم 3: نعم، إنه عدو!
                Debug.Log("⚔️ Enemy Found! Dealing Damage to: " + other.name);
                
                StartCoroutine(ApplyDamage(other));
            }
            else
            {
                Debug.Log("Object is NOT an enemy. Tag: " + other.tag + " | Layer: " + other.gameObject.layer);
            }
        }
    }
}