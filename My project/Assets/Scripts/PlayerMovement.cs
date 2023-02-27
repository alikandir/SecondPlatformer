using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private Animator anim;
    private SpriteRenderer sprite;

    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float jumpGraceTime = 0.1f; // jumpCooldown



    private float dirX = 0f;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private float jumpGraceCounter;


    private enum MovementState { Idle, Running, Jumping, Falling }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        jumpGraceCounter -= Time.deltaTime;

        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;

        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;

        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        if (jumpBufferCounter > 0f && coyoteCounter > 0f && jumpGraceCounter <= 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            coyoteCounter = 0f;
            jumpBufferCounter = 0f;
            jumpGraceCounter = jumpGraceTime;

        }
    
       

        UpdateAnimationState();
    }
    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.Running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.Running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.Idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.Jumping;
        }
        else if (rb.velocity.y < -1f)
        {
            state = MovementState.Falling;
        }

        anim.SetInteger("state", (int)state);
    }
    private bool IsGrounded()
    {
        Vector2 boxSize = new Vector2(0.60f, 1.12f); // half the size of the collider
        RaycastHit2D hit = Physics2D.BoxCast(coll.bounds.center, boxSize, 0f, Vector2.down, 0.2f, jumpableGround);
        return hit.collider != null;
        
    }


}