using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    // dash
    public float dashSpeed = 20;
    private float dashTime;
    public float startDashTime = 0.1f;
    public bool hasDashed = false;


    // buffered jumping
    private bool jumping = false;
    public float jumpDelay = 0.25f;
    private float lastJumped;

    // coyote time
    private float lastValidGroundTouch;
    public float groundTouchedValidTil = 0.25f;

    [Header("Components")]
    public GameObject spriteHolder;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
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

    [SerializeField] private bool isBangingHeadLeft = false;
    [SerializeField] private bool isBangingHeadRight = false;
    [SerializeField] private bool isBangingHeadBoth = false;

    public Vector3 raycastOffsetLeft;
    public Vector3 raycastOffsetRight;
    public float lengthToGround = 0.83f;
    public float lengthToWall = 0.4f;
    public Vector3 cornerCorrectionOffsetLeft;
    public Vector3 cornerCorrectionOffsetRight;
    public Vector3 cornerCorrectionInnerOffset;
    public float cornerCorrectionLength = 0.25f;

    [Header("Particles")]
    public ParticleSystem dust;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.Raycast(transform.position - raycastOffsetLeft, Vector2.down, lengthToGround, groundLayer) || Physics2D.Raycast(transform.position + raycastOffsetRight, Vector2.down, lengthToGround, groundLayer); ;
        isOnLeftWall = Physics2D.Raycast(transform.position, Vector2.left, lengthToWall, groundLayer);
        isOnRightWall = Physics2D.Raycast(transform.position, Vector2.right, lengthToWall, groundLayer);
        isOnWall = isOnLeftWall || isOnRightWall;
        
        isBangingHeadLeft = Physics2D.Raycast(transform.position - cornerCorrectionOffsetLeft, Vector2.up, cornerCorrectionLength, groundLayer) && !Physics2D.Raycast(transform.position - cornerCorrectionOffsetLeft + cornerCorrectionInnerOffset, Vector2.up, cornerCorrectionLength, groundLayer);;
        isBangingHeadRight = Physics2D.Raycast(transform.position + cornerCorrectionOffsetRight, Vector2.up, cornerCorrectionLength, groundLayer) && !Physics2D.Raycast(transform.position + cornerCorrectionOffsetRight - cornerCorrectionInnerOffset, Vector2.up, cornerCorrectionLength, groundLayer);;
        isBangingHeadBoth = isBangingHeadLeft && isBangingHeadRight;


        if(!wasGrounded && isGrounded)
        {
            StartCoroutine(SqueezeSprites(new Vector2(1.25f, 0.8f), 0.05f));
        }

        if(isGrounded || grabbing)
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
        if ((isBangingHeadLeft || isBangingHeadRight) && !isBangingHeadBoth)
        {
            bc.enabled = false;

            Vector2 correctionVelocity = isBangingHeadLeft ? Vector2.right : Vector2.left;
            correctionVelocity.y = rb.velocity.y;

            rb.velocity = correctionVelocity;
        }
        else
        {
            if (!bc.enabled)
            {
                bc.enabled = true;
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        if(grabbing && isOnWall)
        {
            rb.gravityScale = 0;
            return;
        }

        if (isGrounded)
        {
            bool changingDirection = (input.x < 0 && rb.velocity.x > 0) || (input.x > 0 && rb.velocity.x < 0);
            rb.drag = Mathf.Abs(input.x) == 0 || changingDirection ? drag : 0;

            rb.gravityScale = 0;
            hasDashed = false;
        }
        else
        {
            rb.gravityScale = jumpingGravity;
            rb.drag = drag * 0.15f;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale *= fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !jumping)
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
        rb.velocity += Vector2.right * (input.x * runSpeed * Time.deltaTime);
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
        lastJumped = 0;

        dust.Play();
        StartCoroutine(SqueezeSprites(new Vector2(0.8f, 1.25f), 0.05f));
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

        bool oldDirection = spriteRenderer.flipX;
        spriteRenderer.flipX = input.x < 0.0f;

        if (oldDirection != spriteRenderer.flipX)
        {
            dust.Play();
        }
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
        Gizmos.DrawLine(transform.position + raycastOffsetRight, transform.position + raycastOffsetRight + Vector3.down * lengthToGround);
        Gizmos.DrawLine(transform.position - raycastOffsetLeft, transform.position - raycastOffsetLeft + Vector3.down * lengthToGround);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * lengthToWall);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * lengthToWall);
        
        Gizmos.DrawLine(transform.position + cornerCorrectionOffsetRight - cornerCorrectionInnerOffset, transform.position + cornerCorrectionOffsetRight - cornerCorrectionInnerOffset + Vector3.up * cornerCorrectionLength);
        Gizmos.DrawLine(transform.position - cornerCorrectionOffsetLeft + cornerCorrectionInnerOffset, transform.position - cornerCorrectionOffsetLeft + cornerCorrectionInnerOffset + Vector3.up * cornerCorrectionLength);

        Gizmos.DrawLine(transform.position + cornerCorrectionOffsetRight, transform.position + cornerCorrectionOffsetRight + Vector3.up * cornerCorrectionLength);
        Gizmos.DrawLine(transform.position - cornerCorrectionOffsetLeft, transform.position - cornerCorrectionOffsetLeft + Vector3.up * cornerCorrectionLength);
    }

    IEnumerator SqueezeSprites(Vector2 squeeze, float animTime)
    {
        Vector3 squeezedScale = squeeze;
        squeezedScale.z = 1.0f;

        float timePassed = 0;

        while(timePassed <= 1.0f)
        {
            timePassed += Time.deltaTime / animTime;
            spriteHolder.transform.localScale = Vector3.Lerp(Vector3.one, squeezedScale, timePassed);
            yield return null;
        }
        timePassed = 0;
        while(timePassed <= 1.0f)
        {
            timePassed += Time.deltaTime / animTime;
            spriteHolder.transform.localScale = Vector3.Lerp(squeezedScale, Vector3.one, timePassed);
            yield return null;
        }
    }
    
    public void OnMove(InputAction.CallbackContext ctx)
    {
        dir = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        switch (ctx.phase)
        {
            case InputActionPhase.Started:
                lastJumped = Time.time + jumpDelay;
                break;
            case InputActionPhase.Performed:
                jumping = true;
                break;
            default:
                jumping = false;
                break;
        }
    }

    public void OnGrab(InputAction.CallbackContext ctx)
    {
        switch (ctx.phase)
        {
            case InputActionPhase.Started:
            case InputActionPhase.Performed:
                if (!isOnWall)
                {
                    return;
                }
                grabbing = true;
                break;
            default:
                grabbing = false;
                break;
        }
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        switch (ctx.phase)
        {
            case InputActionPhase.Started:
            case InputActionPhase.Performed:
                Dash();
                break;
            default:
                break;
        }
    }

    public void Dash()
    {
        if (!hasDashed)
        {
            rb.velocity = Vector2.zero;
            dust.Play();
            rb.velocity += dir.normalized * dashSpeed;
            hasDashed = true;
        }
    }

}
