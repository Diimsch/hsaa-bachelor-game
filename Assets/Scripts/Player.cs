using System;
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
    private IEnumerator grabbing = null;
    private int stamina = 100;

    [Header("Movement")]
    public float runSpeed = 10.0f;
    public float maxSpeed = 7.0f;
    public float jumpForce = 10.0f;
    public float climbSpeed = 3.0f;
    public float drag = 4.0f;
    public float jumpingGravity = 1.0f;
    public float fallMultiplier = 2.5f;

    // dash
    public float dashSpeed = 10;
    private float dashTime;
    public float startDashTime = 0.1f;
    public bool hasDashed = false;
    private bool dashing = false;


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
    private bool isGroundedLeft = false;
    [SerializeField]
    private bool isGroundedRight = false;
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
    
    // climb ledge
    public Vector3 climbLedgeOffset;
    public bool feetTouchingWall = false;
    private IEnumerator climbingLedge = null;

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
        isGroundedLeft = Physics2D.Raycast(transform.position - raycastOffsetLeft, Vector2.down, lengthToGround, groundLayer);
        isGroundedRight = Physics2D.Raycast(transform.position + raycastOffsetRight, Vector2.down, lengthToGround, groundLayer); ;
        isGrounded = isGroundedLeft || isGroundedRight;

        isOnLeftWall = Physics2D.Raycast(transform.position, Vector2.left, lengthToWall, groundLayer);
        isOnRightWall = Physics2D.Raycast(transform.position, Vector2.right, lengthToWall, groundLayer);
        isOnWall = isOnLeftWall || isOnRightWall;
        
        isBangingHeadLeft = Physics2D.Raycast(transform.position - cornerCorrectionOffsetLeft, Vector2.up, cornerCorrectionLength, groundLayer) && !Physics2D.Raycast(transform.position - cornerCorrectionOffsetLeft + cornerCorrectionInnerOffset, Vector2.up, cornerCorrectionLength, groundLayer);;
        isBangingHeadRight = Physics2D.Raycast(transform.position + cornerCorrectionOffsetRight, Vector2.up, cornerCorrectionLength, groundLayer) && !Physics2D.Raycast(transform.position + cornerCorrectionOffsetRight - cornerCorrectionInnerOffset, Vector2.up, cornerCorrectionLength, groundLayer);;
        isBangingHeadBoth = isBangingHeadLeft && isBangingHeadRight;
        
        bool feetOnLeftWall = Physics2D.Raycast(transform.position + climbLedgeOffset, Vector2.left, lengthToWall, groundLayer);
        bool feetOnRightWall = Physics2D.Raycast(transform.position + climbLedgeOffset, Vector2.right, lengthToWall, groundLayer);
        feetTouchingWall = feetOnLeftWall || feetOnRightWall;


        if(!wasGrounded && isGrounded)
        {
            StartCoroutine(SqueezeSprites(new Vector2(1.25f, 0.8f), 0.05f));
        }

        if (isGrounded || isOnWall)
        {
            lastValidGroundTouch = Time.time + groundTouchedValidTil;
        }
        
        if(isGrounded && grabbing == null)
        {
            stamina = 100;
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

        if (dashing)
        {
            return;
        }

        if(grabbing != null && isOnWall)
        {
            rb.gravityScale = 0;
            return;
        }

        if (isGrounded)
        {
            bool changingDirection = (input.x < 0 && rb.velocity.x > 0) || (input.x > 0 && rb.velocity.x < 0);
            // rb.drag = Mathf.Abs(input.x) == 0 || changingDirection ? drag : 0;

            rb.gravityScale = 0;
            hasDashed = false;
        }
        else
        {
            rb.gravityScale = jumpingGravity;
            // rb.drag = drag * 0.15f;
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
        //climbing up.
        if(grabbing != null)
        {
            // still on wall, we can climb
            if (isOnWall)
            {
                rb.velocity = new Vector2(0, input.y * climbSpeed);
                return;
            }
            
            // only feet left on wall, we have to climb the ledge
            if(feetTouchingWall)
            {
                if (climbingLedge != null)
                {
                    return;
                }

                climbingLedge = ClimbLedge();
                StartCoroutine(climbingLedge);
                return;
            }
        }
        rb.velocity += Vector2.right * (input.x * runSpeed * Time.deltaTime);
        if(Mathf.Abs(rb.velocity.x) > maxSpeed && !dashing)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }

    private IEnumerator ClimbLedge()
    {
        while (feetTouchingWall)
        {
            rb.AddForce(Vector2.up, ForceMode2D.Impulse);
            yield return new WaitForEndOfFrame();
        }

        bc.enabled = false;

        while (!isGroundedLeft || !isGroundedRight)
        {
            Vector2 velocity = spriteRenderer.flipX ? Vector2.right : Vector2.left;
            velocity *= runSpeed * Time.deltaTime;
            rb.velocity += velocity;
            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector2.zero;
        bc.enabled = true;
        StopCoroutine(grabbing);
        climbingLedge = null;
        grabbing = null;
    }
    
    private void Jump()
    {
        Vector2 jumpDirection = Vector2.up;
        // walljump

        bool changeDirection = false;
        if (isOnWall && !isGrounded)
        {
            jumpDirection += isOnLeftWall ? Vector2.right : Vector2.left;
            changeDirection = true;
        } 
        animator.SetTrigger("isJumping");
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        lastJumped = 0;

        dust.Play();
        StartCoroutine(SqueezeSprites(new Vector2(0.8f, 1.25f), 0.05f));

        if (changeDirection)
        {
            UpdateDirection(jumpDirection);
        }
    }

    private void UpdateDirection(Vector2 input)
    {
        if(grabbing != null)
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
        animator.SetBool("isGrabbing", grabbing != null && isOnWall);
        animator.SetBool("isClimbing", grabbing != null && isOnWall && Mathf.Abs(rb.velocity.y) > 0.1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + raycastOffsetRight, transform.position + raycastOffsetRight + Vector3.down * lengthToGround);
        Gizmos.DrawLine(transform.position - raycastOffsetLeft, transform.position - raycastOffsetLeft + Vector3.down * lengthToGround);
        
        //climbing (hands)
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * lengthToWall);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * lengthToWall);
        
        //climbing (ledge)
        Gizmos.DrawLine(transform.position + climbLedgeOffset, transform.position + climbLedgeOffset + Vector3.left * lengthToWall);
        Gizmos.DrawLine(transform.position + climbLedgeOffset, transform.position + climbLedgeOffset + Vector3.right * lengthToWall);
        
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
                if (!isOnWall || stamina <= 0)
                {
                    return;
                }
                Debug.Log("start climbing.");
                grabbing = Grab();
                StartCoroutine(grabbing);
                break;
            case InputActionPhase.Performed:
                if (!isOnWall)
                {
                    if (grabbing != null)
                    {
                        StopCoroutine(grabbing);
                        grabbing = null;
                    }
                }
                else
                {
                    if (grabbing == null && stamina > 0)
                    {
                        grabbing = Grab();
                        StartCoroutine(grabbing);
                    }
                }
                break;
            default:
                if (grabbing != null)
                {
                    StopCoroutine(grabbing);
                    grabbing = null;
                }
                break;
        }
    }

    private IEnumerator Grab()
    {
        while (stamina > 0)
        {
            stamina -= 0;
            yield return new WaitForSeconds(0.1f);
        }
        grabbing = null;
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        switch (ctx.phase)
        {
            case InputActionPhase.Started:
            case InputActionPhase.Performed:
                StartCoroutine(Dash());
                break;
            default:
                break;
        }
    }

    private IEnumerator Dash()
    {
        if (!hasDashed)
        {
            rb.velocity = Vector2.zero;
            dust.Play();

            Vector2 dashingDirection;

            if (dir.Equals(Vector2.zero))
            {
                dashingDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
                rb.velocity += dashingDirection * dashSpeed;
            }
            else
            {
                dashingDirection = dir;
                rb.velocity += dir.normalized * dashSpeed;
            }

            hasDashed = true;
            dashing = true;
            float gravityScale = rb.gravityScale;
            rb.gravityScale = 0;
            yield return new WaitForSeconds(0.05f);
            rb.gravityScale = gravityScale;
            dashing = false;
        }
    }
    
    

}
