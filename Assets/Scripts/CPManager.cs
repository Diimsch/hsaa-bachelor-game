using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CPManager : MonoBehaviour
{
    public Player player;
    public GameObject CoinHolder;
    public GameObject CheckpointHolder;
    public TMP_Text cp;

    private int maxCP;
    // Start is called before the first frame update
    void Start()
    {
        maxCP = CoinHolder.transform.childCount + CheckpointHolder.transform.childCount * 5;
        UpdateCP(0);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCP(player.cp);
    }

    private void UpdateCP(int current)
    {
        cp.text = String.Format("{0} / {1} CP", current, maxCP);
    }
}
