using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Input")]
    [SerializeField]
    private Vector2 dir;
    [SerializeField]
    private bool grabbing = false;

    [Header("Movement")]
    public float runSpeed = 10.0f;
    public float maxSpeed = 7.0f;
    public float jumpForce = 10.0f;
    public float climbSpeed = 3.0f;
    public float drag = 4.0f;
    public float jumpingGravity = 1.0f;
    public float fallMultiplier = 2.5f;

    // buffered jumping
    public float jumpDelay = 0.25f;
    private float lastJumped;

    // coyote time
    private float lastValidGroundTouch;
    public float groundTouchedValidTil = 0.25f;

    [Header("Components")]
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;
    public LayerMask groundLayer;

    [Header("Collision")]
    [SerializeField]
    private bool isGrounded = false;
    [SerializeField]
    private bool isOnWall = false;
    [SerializeField]
    private bool isOnLeftWall = false;
    [SerializeField]
    private bool isOnRightWall = false;

    public float lengthToGround = 0.83f;
    public float lengthToWall = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, lengthToGround, groundLayer);
        isOnLeftWall = Physics2D.Raycast(transform.position, Vector2.left, lengthToWall, groundLayer);
        isOnRightWall = Physics2D.Raycast(transform.position, Vector2.right, lengthToWall, groundLayer);
        isOnWall = isOnLeftWall || isOnRightWall;
        dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        grabbing = Input.GetButton("Grab");

        if(Input.GetButtonDown("Jump"))
        {
            lastJumped = Time.time + jumpDelay;
        }

        if(isGrounded)
        {
            lastValidGroundTouch = Time.time + groundTouchedValidTil;
        }
    }

    private void FixedUpdate()
    {
        Vector2 currentDirection = dir;

        UpdateDirection(currentDirection);
        UpdateAnimator(currentDirection);

        MoveCharacter(currentDirection);

        if(lastJumped > Time.time && lastValidGroundTouch > Time.time)
        {
            lastValidGroundTouch = 0;
            Jump();
        }

        UpdatePhysics(currentDirection);
    }

    private void UpdatePhysics(Vector2 input)
    {
        if(grabbing && isOnWall)
        {
            rb.gravityScale = 0;
            return;
        }

        if (isGrounded)
        {
            bool changingDirection = (input.x < 0 && rb.velocity.x > 0) || (input.x > 0 && rb.velocity.x < 0);
            rb.drag = Mathf.Abs(input.x) < 0.4f || changingDirection ? drag : 0;

            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = jumpingGravity;
            rb.drag = drag * 0.15f;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale *= fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale *= fallMultiplier / 2;
            }
        }
    }

    private void MoveCharacter(Vector2 input)
    {
        if(grabbing && isOnWall)
        {
            rb.velocity = new Vector2(0, input.y * climbSpeed);
            return;
        }
        rb.AddForce(Vector2.right * input.x * runSpeed);
        if(Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }

    private void Jump()
    {
        animator.SetTrigger("isJumping");
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void UpdateDirection(Vector2 input)
    {
        if(grabbing)
        {
            if(isOnLeftWall)
            {
                spriteRenderer.flipX = false;
            } else if(isOnRightWall)
            {
                spriteRenderer.flipX = true;
            }
            return;
        }
        if(input.x == 0.0f)
        {
            return;
        }
        spriteRenderer.flipX = input.x < 0.0f;
    }

    private void UpdateAnimator(Vector2 input)
    {
        animator.SetBool("isRunning", Mathf.Abs(rb.velocity.x) > 0.1f);
        animator.SetBool("isFalling", rb.velocity.y < -0.001f);
        animator.SetBool("isGrabbing", grabbing && isOnWall);
        animator.SetBool("isClimbing", grabbing && isOnWall && Mathf.Abs(rb.velocity.y) > 0.1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * lengthToGround);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * lengthToWall);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * lengthToWall);
    }
}
