using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private Animator anim;

    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private float coyoteTime = 0.2f;

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;


    private SpriteRenderer sprite;
    private float dirX = 0f;


    private float coyoteCounter;

    private enum MovementState { Idle, Running, Jumping, Falling }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider2D>();
        coyoteCounter = coyoteTime;
    }

    // Update is called once per frame
    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && coyoteCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

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
        RaycastHit2D hit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
        return hit.collider != null;
    }
}