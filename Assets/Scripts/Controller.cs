using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private PlayerCollision pc;
    private Rigidbody2D rb;

    public const float MaxRun = 90f;
    public const float RunForce = 10f;

    public const float JumpForce = 15f;
    public const float JumpCancelVelocity = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pc = GetComponent<PlayerCollision>();
    }

    private void Update()
    {
        ProcessInput();
    }


    private void ProcessInput()
    {
        Run();

        if(Input.GetButtonDown("Jump") && pc.onGround)
        {
            Jump();
        }

        if(Input.GetButtonUp("Jump") && rb.velocity.y > JumpCancelVelocity)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpCancelVelocity);
        }
    }

    private void Run()
    {
        float direction = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(Mathf.Clamp(direction * RunForce, -MaxRun, MaxRun), rb.velocity.y);
    }

    private void Jump()
    {
        Vector2 dir = Vector2.up * JumpForce;
        rb.AddForce(dir, ForceMode2D.Impulse);
    }
}
