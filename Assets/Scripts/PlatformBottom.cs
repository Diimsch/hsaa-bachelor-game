using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformBottom : MonoBehaviour
{
    public Collider2D topCollider;

    private void OnTriggerEnter2D(Collider2D other)
    {
        topCollider.isTrigger = true;
    }
}
