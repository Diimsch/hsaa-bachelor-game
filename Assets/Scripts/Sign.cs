using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour
{
    private SpriteRenderer sr;

    public Dialog dialog;
    public Canvas canvas;
    public TMP_Text text;

    public Color activeTextColor;
    public Color normalTextColor;

    public Sprite normalSprite;
    public Sprite activatableSprite;
    public Sprite activatedSprite;

    private bool triggered;

    private bool interacting;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        sr.sprite = activatableSprite;
        canvas.enabled = true;
        triggered = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        triggered = false;
        interacting = false;
        canvas.enabled = false;
        sr.sprite = normalSprite;
        text.color = normalTextColor;
        Dialog.Instance.Clear();
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!triggered)
        {
            return;
        }
        
        switch (ctx.phase)
        {
            case InputActionPhase.Started:
                if (interacting)
                {
                    sr.sprite = activatableSprite;
                    text.color = normalTextColor;
                    Dialog.Instance.Clear();
                    interacting = false;
                }
                else
                {
                    sr.sprite = activatedSprite;
                    text.color = activeTextColor;
                    interacting = true;
                    Dialog.Instance.PushText("Game Controls:\nJump - [SPACE]\nClimb - [K]");
                }
                break;
        }
    }
}
