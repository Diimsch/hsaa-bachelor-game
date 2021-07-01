using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Dialog _dialog;
    private bool _interacting;

    public string signText;
    // Start is called before the first frame update
    void Start()
    {
        _dialog = dialogCanvas.GetComponent<Dialog>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        sr.sprite = activatableSprite;
        canvas.enabled = true;
        SignManager.currentSign = this;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _interacting = false;
        canvas.enabled = false;
        sr.sprite = normalSprite;
        text.color = normalTextColor;
        _dialog.Clear();
        SignManager.currentSign = null;
    }

    public void Interact()
    {
        if (_interacting)
        {
            sr.sprite = activatableSprite;
            text.color = normalTextColor;
            _dialog.Clear();
            _interacting = false;
        }
        else
        {
            sr.sprite = activatedSprite;
            text.color = activeTextColor;
            _interacting = true;
            _dialog.PushText(signText);
        }
    }
}
