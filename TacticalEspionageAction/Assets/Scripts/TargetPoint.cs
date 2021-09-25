using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    public delegate void PlayerOnPoint();
    public event PlayerOnPoint OnPlayerOnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            OnPlayerOnPoint?.Invoke();
        }
    }
}
