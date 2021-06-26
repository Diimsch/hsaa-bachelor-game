using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTop : MonoBehaviour
{
    private Collider2D _collider;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _collider.isTrigger = false;
    }
    
    
}
