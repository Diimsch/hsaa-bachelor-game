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
    public GameObject spriteHolder;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
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

    public Vector3 raycastOffsetLeft;
    public Vector3 raycastOffsetRight;
    public float lengthToGround = 0.83f;
    public float lengthToWall = 0.4f;

    [Header("Particles")]
    public ParticleSystem dust;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.Raycast(transform.position - raycastOffsetLeft, Vector2.down, lengthToGround, groundLayer) || Physics2D.Raycast(transform.position + raycastOffsetRight, Vector2.down, lengthToGround, groundLayer); ;
        isOnLeftWall = Physics2D.Raycast(transform.position, Vector2.left, lengthToWall, groundLayer);
        isOnRightWall = Physics2D.Raycast(transform.position, Vector2.right, lengthToWall, groundLayer);
        isOnWall = isOnLeftWall || isOnRightWall;
        dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        grabbing = Input.GetButton("Grab");

        if(!wasGrounded && isGrounded)
        {
            StartCoroutine(SqueezeSprites(new Vector2(1.25f, 0.8f), 0.05f));
        }

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
            rb.drag = Mathf.Abs(input.x) == 0 || changingDirection ? drag : 0;
            Debug.Log(input.x);

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
        rb.velocity += Vector2.right * input.x * runSpeed * Time.deltaTime;
        //rb.AddForce(Vector2.right * input.x * runSpeed);
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
}
