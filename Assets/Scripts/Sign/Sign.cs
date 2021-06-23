using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour
{
    private SpriteRenderer sr;

    public Canvas dialogCanvas;
    public Canvas canvas;
    public TMP_Text text;

    public Color activeTextColor;
    public Color normalTextColor;

    public Sprite normalSprite;
    public Sprite activatableSprite;
    public Sprite activatedSprite;

    private Dialog dialog;

    private bool triggered;
    private bool interacting;
    // Start is called before the first frame update
    void Start()
    {
        dialog = dialogCanvas.GetComponent<Dialog>();
        sr = GetComponent<SpriteRenderer>();
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
        dialog.Clear();
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
                    dialog.Clear();
                    interacting = false;
                }
                else
                {
                    sr.sprite = activatedSprite;
                    text.color = activeTextColor;
                    interacting = true;
                    dialog.PushText("Game Controls:\nJump - [SPACE]\nClimb - [K]\nBoost - [J]");
                }
                break;
        }
    }
}