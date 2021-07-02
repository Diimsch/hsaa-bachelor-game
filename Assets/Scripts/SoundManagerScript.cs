using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{

    public static AudioClip coinPickupSound, checkPointSound, dashSound, deathSound, jumpSound;
    static AudioSource audioSrc;

    // Start is called before the first frame update
    void Start()
    {
        coinPickupSound = Resources.Load<AudioClip>("CoinPickup");
        checkPointSound = Resources.Load<AudioClip>("Checkpoint");
        dashSound = Resources.Load<AudioClip>("Dash");
        deathSound = Resources.Load<AudioClip>("Death");
        jumpSound = Resources.Load<AudioClip>("Jump");


        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "CoinPickup":
                audioSrc.PlayOneShot(coinPickupSound);
                break;
            case "Checkpoint":
                audioSrc.PlayOneShot(checkPointSound);
                break;
            case "Dash":
                audioSrc.PlayOneShot(dashSound);
                break;
            case "Death":
                audioSrc.PlayOneShot(deathSound);
                break;
            case "Jump":
                audioSrc.PlayOneShot(jumpSound);
                break;
        }
    }
}
