using TMPro;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static Checkpoint CurrentCheckpoint;
    
    private SpriteRenderer sr;
    
    private Canvas canvas;
    public TMP_Text text;

    public Color activeTextColor;
    
    public Sprite activatedSprite;

    private bool triggered = false;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        canvas = GetComponentInChildren<Canvas>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        triggered = true;
        canvas.enabled = true;
        text.color = activeTextColor;
        sr.sprite = activatedSprite;
        
        // set current checkpoint
        CurrentCheckpoint = this;
    }
}