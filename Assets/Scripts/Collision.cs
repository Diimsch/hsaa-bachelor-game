using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    private Controller controller;

    private void Awake()
    {
        controller = GetComponent<Controller>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
