using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbRespawner : MonoBehaviour
{
    public GameObject orb;
    public Player player;

    public void Respawn()
    {
        StartCoroutine(TriggerRespawn());
    }

    IEnumerator TriggerRespawn()
    {
        for (int i = 5; i > 0; --i)
        {
            if (player.isGrounded)
            {
                break;
            }
            yield return new WaitForSeconds(1);
        }

        Instantiate(orb, transform);
        yield return null;
    }
}
