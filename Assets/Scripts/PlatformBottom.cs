using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformBottom : MonoBehaviour
{
    private Collider2D bottomCollider;
    public Collider2D topCollider;

    private void Start()
    {
        bottomCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        topCollider.isTrigger = true;
    }
}
