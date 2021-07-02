using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class GhostTrail : MonoBehaviour
{
    private Player player;
    public Transform ghostsParent;
    public Color trailColor;
    public Color fadeColor;
    public float ghostInterval;
    public float fadeTime;


    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void ShowGhost()
    {
        Sequence s = DOTween.Sequence();

        for (int i = 0; i < ghostsParent.childCount; i++)
        {
            Transform currentGhost = ghostsParent.GetChild(i);
            SpriteRenderer ghostSpriteRenderer = currentGhost.GetComponent<SpriteRenderer>();
            s.AppendCallback(() => currentGhost.position = player.transform.position);
            s.AppendCallback(() => ghostSpriteRenderer.flipX = player.spriteRenderer.flipX);
            s.AppendCallback(() => ghostSpriteRenderer.sprite = player.spriteRenderer.sprite);
            s.Append(ghostSpriteRenderer.material.DOColor(trailColor, 0));
            s.AppendCallback(() => FadeSprite(ghostSpriteRenderer));
            s.AppendInterval(ghostInterval);
        }
    }

    private void FadeSprite(SpriteRenderer ghostSpriteRenderer)
    {
        ghostSpriteRenderer.material.DOKill();
        ghostSpriteRenderer.material.DOColor(fadeColor, fadeTime);
    }
}
