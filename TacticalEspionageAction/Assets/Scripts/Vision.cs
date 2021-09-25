using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    [SerializeField]
    private Transform eyesTransform;

    public delegate void PlayerInSight(GameObject target);
    public event PlayerInSight SeePlayer;
    public event PlayerInSight SpotPlayer;


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Ray ray = new Ray(eyesTransform.position, other.transform.position - eyesTransform.position);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.collider.gameObject == other.gameObject)
                {
                    SeePlayer?.Invoke(other.gameObject);
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Ray ray = new Ray(eyesTransform.position, other.transform.position - eyesTransform.position);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.collider.gameObject == other.gameObject)
                {
                    SpotPlayer?.Invoke(other.gameObject);
                }
            }
        }
    }
}
