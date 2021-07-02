using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class GhostTrail : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    public Transform ghostsParent;
    public Color trailColor;
    public Color fadeColor;
    public float ghostInterval;
    public float fadeTime;


    private void Start()
    {
        player = FindObjectOfType<Player>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void ShowGhost()
    {
        Sequence s = DOTween.Sequence();

        for (int i = 0; i < ghostsParent.childCount; i++)
        {
            Transform currentGhost = ghostsParent.GetChild(i);
            s.AppendCallback(() => currentGhost.position = player.transform.position);
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().flipX = player.spriteRenderer.flipX);
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().sprite = player.spriteHolder.GetComponent<SpriteRenderer>().sprite);
            s.Append(currentGhost.GetComponent<SpriteRenderer>().material.DOColor(trailColor, 0));
            s.AppendCallback(() => FadeSprite(currentGhost));
            s.AppendInterval(ghostInterval);
        }
    }

    public void FadeSprite(Transform current)
    {
        current.GetComponent<SpriteRenderer>().material.DOKill();
        current.GetComponent<SpriteRenderer>().material.DOColor(fadeColor, fadeTime);
    }
}
