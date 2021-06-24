using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTop : MonoBehaviour
{
    private Collider2D collider;

    private void Start()
    {
        collider = GetComponent<Collider2D>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        collider.isTrigger = false;
    }
    
    
}
