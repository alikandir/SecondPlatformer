using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private float jumpBufferTime = 0.001f;
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

        var jumpInputReleased = Input.GetButtonUp("Jump");

        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;

        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && coyoteCounter > 0f && jumpGraceCounter <= 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            coyoteCounter = 0f;
           
            jumpGraceCounter = jumpGraceTime;

        }
        if (jumpInputReleased && rb.velocity.y>0) 
        {
            rb.velocity = new Vector2(rb.velocity.x, y: 0f);
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
        Vector2 boxSize = new Vector2(0.60f, 0.6f);
        Vector2 origin = new Vector2(coll.bounds.center.x, coll.bounds.min.y - 0.3f);
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, coll.bounds.extents.y, jumpableGround);
       

        if (hit.collider != null)
        {
            Debug.DrawRay(origin, Vector2.down, Color.green);
                    }
        else
        {

            Debug.DrawRay(origin, Vector2.down, Color.red);
            Debug.Log(hit.collider);
        }
       
        return hit.collider != null;
    }


}