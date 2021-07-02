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
            Debug.Log("moiun0");
            s.AppendCallback(() => currentGhost.position = player.transform.position);
            Debug.Log("moin1");
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().sprite = player.spriteRenderer.sprite);
            Debug.Log("moin2");
            s.Append(currentGhost.GetComponent<SpriteRenderer>().material.DOColor(trailColor, 0));
            Debug.Log("moin3");
            s.AppendCallback(() => FadeSprite(currentGhost));
            Debug.Log("moin4");
            s.AppendInterval(ghostInterval);
            Debug.Log("moin5");
        }
    }

    public void FadeSprite(Transform current)
    {
        current.GetComponent<SpriteRenderer>().material.DOKill();
        current.GetComponent<SpriteRenderer>().material.DOColor(fadeColor, fadeTime);
    }
}
