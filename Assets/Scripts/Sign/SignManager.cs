using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignManager : MonoBehaviour
{
    public static Sign currentSign;

    public static void InteractWithCurrentSign()
    {
        if (!currentSign)
        {
            return;
        }
        currentSign.Interact();
    }
}
