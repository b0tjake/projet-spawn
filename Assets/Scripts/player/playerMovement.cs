using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public AudioClip hitSound;
    private AudioSource audioSource;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 5f;
    private Coroutine jumpCoroutine;
    [SerializeField] private GameObject playerAttackBox;

    [Header("Fly Kick")]
    private float flyKickSpeed = 7f;
    private float flyKickDuration = 0.5f;
    private bool isFlyKicking = false;
    private float defaultGravityScale;
    [SerializeField] private bool flyKickLimits = false;

    [Header("Gun Attack")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 15f;

    [Header("Gun Burst Settings")]
    public int burstCount = 7;
    public float timeBetweenShots = 0.1f;

    [Header("Rage UI")]
    public Image[] rageSkulls;
    public float maxRage = 4.0f;
    #endregion

    #region Private State
    private Rigidbody2D rb;
    private Animator anim;
    private float originalScaleX;
    private float originalScaleY;

    [SerializeField] private bool grounded = false;
    public bool dontFall = false;
    public bool isSpecialAttacking = false;
    private bool isCrouching = false;

    private int comboStep = 0;
    private float comboTimer = 0f;
    private float nextAttackTime = 0f;
    private float rageValue = 0f;

    private string inputString = "";
    private float currentMoveInput = 0f;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        originalScaleX = transform.localScale.x;
        originalScaleY = transform.localScale.y;
        defaultGravityScale = rb.gravityScale;

        if (playerAttackBox != null) playerAttackBox.SetActive(false);
    }

    void Update()
    {
        flyKickLimiter();
        HandleCooldowns();

        if (!isSpecialAttacking && !anim.GetBool("isGuarding") && !isFlyKicking)
        {
            Move(currentMoveInput);

            if (currentMoveInput != 0)
                Flip(currentMoveInput);
        }
        else if (anim.GetBool("isGuarding"))
        {
            Move(0);
        }

        anim.SetBool("moving", currentMoveInput != 0 && !isFlyKicking && !isSpecialAttacking);
        anim.SetBool("crouching", isCrouching);

        if (!grounded && rb.gravityScale == defaultGravityScale)
            anim.SetBool("jump", true);

        if (grounded)
            anim.SetBool("jump", false);

        foreach (char c in Input.inputString)
        {
            inputString += c;
            if (inputString.Length > 20)
                inputString = inputString.Substring(inputString.Length - 20);

            if (inputString.ToLower().Contains("rehitemup"))
            {
                inputString = "";
                Time.timeScale = 1f;
                Scene activeScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(activeScene.name);
            }
        }
    }

    // Methods Called By Input Script
    public void SetMoveInput(float dir)
    {
        currentMoveInput = dir;
    }

    public void SetCrouch(bool value)
    {
        isCrouching = value;
    }

    public void tryJump()
    {
        if (isSpecialAttacking || anim.GetBool("isGuarding") || isFlyKicking) return;

        if (grounded)
            jumpCoroutine = StartCoroutine(Jump());
    }

    public void punch()
    {
        if (isSpecialAttacking || anim.GetBool("isGuarding") || isFlyKicking) return;

        if (grounded && Time.time >= nextAttackTime)
            Attack("attack");
    }

    public void kick()
    {
        if (isSpecialAttacking || anim.GetBool("isGuarding")) return;

        if (!grounded && !flyKickLimits && !isFlyKicking)
        {
            StartCoroutine(PerformFlyingKick());
        }
        else if (grounded && Time.time >= nextAttackTime)
        {
            Attack("kickAttack");
        }
    }

    public void special1()
    {
        if (isSpecialAttacking || !grounded) return;
        if (rageValue >= 1.0f)
            StartCoroutine(SpecialAttackInput());
    }

    public void special2()
    {
        if (isSpecialAttacking || !grounded) return;
        if (rageValue >= 1.0f)
            StartCoroutine(GunSpecialAttack());
    }

//Core Gameplay
    private void HandleCooldowns()
    {
        if (comboTimer > 0) comboTimer -= Time.deltaTime;
        else comboStep = 0;
    }

    private void Move(float dir)
    {
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
    }

    IEnumerator Jump()
    {
        flyKickLimits = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        grounded = false;

        yield return new WaitForSeconds(1f);

        if (grounded) yield break;

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
        ProcessAttackAnimation(attackPrefix);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            StartCoroutine(ApplyDamage(enemy));
        }

        comboTimer = comboDelay;
        StopCoroutine("DisableHitbox");
        StartCoroutine(DisableHitbox());
    }

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


    // Specials
    IEnumerator GunSpecialAttack()
    {
        isSpecialAttacking = true;
        anim.SetBool("spAttack", true);
        anim.Play("gunAttack");
        IncreaseRage(-1.0f);

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < burstCount; i++)
        {
            ShootBullet();

            if (CameraShake.instance != null)
                CameraShake.instance.Shake(0.05f, 0.1f);

            yield return new WaitForSeconds(timeBetweenShots);
        }

        yield return new WaitForSeconds(0.3f);

        isSpecialAttacking = false;
        anim.SetBool("spAttack", false);
    }

    IEnumerator SpecialAttackInput()
    {
        isSpecialAttacking = true;
        anim.SetBool("spAttack", true);
        anim.SetBool("specialAttack1", true);

        IncreaseRage(-1.0f);

        yield return new WaitForSeconds(4.0f);

        isSpecialAttacking = false;
        anim.SetBool("spAttack", false);
        anim.SetBool("specialAttack1", false);
    }

    public void ShootBullet()
    {
        GameObject b = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        float dir = transform.localScale.x > 0 ? 1 : -1;
        float spread = Random.Range(-0.5f, 0.5f);
        Vector2 velocity = new Vector2(dir * bulletSpeed, spread);

        b.GetComponent<Rigidbody2D>().linearVelocity = velocity;

        if (dir < 0) b.transform.localScale = new Vector3(-1, 1, 1);
    }
    // FlyKick
    public void flyKickLimiter()
    {
        if (grounded)
            flyKickLimits = true;
    }

    IEnumerator PerformFlyingKick()
    {
        isFlyKicking = true;
        flyKickLimits = true;

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

        yield return new WaitForSecondsRealtime(flyKickDuration);

        StopFlyingKick();
    }

    private void StopFlyingKick()
    {
        if (!isFlyKicking) return;

        isFlyKicking = false;
        flyKickLimits = true;

        anim.SetBool("flyKick", false);
        anim.SetBool("glide", true);

        rb.gravityScale = defaultGravityScale;
        attackRange = 1.53f;
        rb.linearVelocity = new Vector2(0, -2f);

        if (playerAttackBox != null)
            playerAttackBox.SetActive(false);
    }


    // Damage / Rage / Collision
    IEnumerator ApplyDamage(Collider2D enemy)
    {
        EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();

        if (enemyScript != null)
        {
            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
            enemyScript.TakeDamage(damage, knockbackDir, stuneDuration);

            if (audioSource != null && hitSound != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(hitSound);
            }

            StartCoroutine(HitStop(0.08f));
            if (CameraShake.instance != null)
                CameraShake.instance.Shake(0.05f, 0.2f);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void IncreaseRage(float amount)
    {
        rageValue += amount;
        rageValue = Mathf.Clamp(rageValue, 0f, maxRage);
        UpdateRageUI();
    }

    private void UpdateRageUI()
    {
        int currentIndex = Mathf.FloorToInt(rageValue / 0.25f) - 1;

        for (int i = 0; i < rageSkulls.Length; i++)
        {
            bool isMilestone = (i + 1) % 4 == 0;
            float milestoneValue = (i + 1) * 0.25f;
            bool milestoneComplete = rageValue >= milestoneValue;

            rageSkulls[i].enabled = (i == currentIndex || (isMilestone && milestoneComplete));
        }
    }

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
        anim.SetBool("flyKick", false);
        anim.SetBool("grounded", grounded);

        if (jumpCoroutine != null)
            StopCoroutine(jumpCoroutine);

        if (collision.gameObject.CompareTag("dontFall"))
            dontFall = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isFlyKicking)
        {
            if (((1 << other.gameObject.layer) & enemyLayers) != 0 || other.CompareTag("Enemy"))
            {
                StartCoroutine(ApplyDamage(other));
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}