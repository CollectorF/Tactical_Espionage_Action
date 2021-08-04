using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Guard", menuName = "Characters/Guard")]
public class GuardStarterInfo : ScriptableObject
{
    public float waitTime;
    public float speedWhenPatrolling = 2f;
    public float speedWhenSuspicious = 3f;
}
