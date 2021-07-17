using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("GOTCHA!!!!");
        }
    }
}
