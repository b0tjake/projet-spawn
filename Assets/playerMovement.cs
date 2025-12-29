using System;
using System.Collections;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f;
    public float jumpForce = 5f;

    private float originalScaleX;
    private float originalScaleY;

    private bool grounded = false;
    private Animator anim;

    [SerializeField]
    GameObject playerAttackBox;
    private bool isCrouching = false;

    private int comboStep = 0;
    private float comboTimer = 0f;
    public float comboDelay = 1.0f;
    
    private float nextAttackTime = 0f;
    public float attackRate = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScaleX = transform.localScale.x;
        originalScaleY = transform.localScale.y;
        anim = GetComponent<Animator>();
        playerAttackBox.SetActive(false);
    }

    void Update()
    {
        isCrouching = Input.GetKey(KeyCode.S);
        anim.SetBool("crouching", isCrouching);

        // combo timer reset
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
        }
        else
        {
            comboStep = 0;
        }

// movements
        float move = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        if (Input.GetKeyDown("w") && grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            grounded = false;
        }

        // left and right flip
        if (move > 0)
            transform.localScale = new Vector3(Mathf.Abs(originalScaleX), Mathf.Abs(originalScaleY), 1);
        else if (move < 0)
            transform.localScale = new Vector3(-Mathf.Abs(originalScaleX), Mathf.Abs(originalScaleY), 1);

        bool isMovingInput = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
        anim.SetBool("moving", isMovingInput);


//Attack
        // We check Time.time >= nextAttackTime to prevent spamming, but allow combos
        if (Input.GetKeyDown(KeyCode.J) && Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    void Attack()
    {
        // play attack animation based on combo step
        comboStep++;
        
        // Loop reset cauz we have 5 attacks for nowa
        if (comboStep > 5) 
        {
            comboStep = 1;
        }

        // Play the animation immediately and from the start
        anim.Play("attack" + comboStep, -1, 0); 

        // 2. Set timers
        comboTimer = comboDelay; 
        nextAttackTime = Time.time + attackRate; 

        StopCoroutine("DisableHitbox"); // Stop existing one if clicking fast
        StartCoroutine(DisableHitbox());
    }

    IEnumerator DisableHitbox()
    {
        playerAttackBox.SetActive(true);
        yield return new WaitForSeconds(0.3f); // Adjust this to match animation swing
        playerAttackBox.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }
}