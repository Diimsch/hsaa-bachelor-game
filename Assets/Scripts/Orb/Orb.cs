using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    private void OnDestroy()
    {
        OrbRespawner respawner = GetComponentInParent<OrbRespawner>();
        if (!respawner)
        {
            return;
        }
        respawner.Respawn();
    }
}
