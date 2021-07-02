using TMPro;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject coinPrefab;
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
        SoundManagerScript.PlaySound("Checkpoint");
        triggered = true;
        canvas.enabled = true;
        text.color = activeTextColor;
        sr.sprite = activatedSprite;

        for (int i = -2; i < 3; ++i)
        {
            GameObject coin = Instantiate(coinPrefab, transform.position, transform.rotation);
            Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
            coinRb.AddForce(new Vector2(i / 2.0f, 12.0f), ForceMode2D.Impulse);
        }
        
        
        // set current checkpoint
        CurrentCheckpoint = this;
    }
}