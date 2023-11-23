using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketCheck : MonoBehaviour
{
    public bool ust = false, alt = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ust")
        {
            ust = true;
        }
        if (other.tag == "Alt")
        {
            alt = true;
        }
    }
}
