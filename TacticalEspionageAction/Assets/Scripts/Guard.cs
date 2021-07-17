using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum GuardStatus
{
    Nothing,
    Patrolling,
    Suspicious
}

public struct GuardState
{
    public GuardStatus Status;
    public bool HasReachedNextPoint;
}


public class Guard : MonoBehaviour
{
    [SerializeField]
    private BehaviorTree _tree;
    [SerializeField]
    private Vector3[] patrollingPoints;
    [SerializeField]
    private NavMeshAgent agent;

    private GuardState state;
    private int currentIndex;

    private void Awake()
    {
        state = new GuardState()
        {
            HasReachedNextPoint = false,
            Status = GuardStatus.Nothing
        };

        _tree = new BehaviorTreeBuilder(gameObject)
            .Sequence()
                .Condition("Custom Condition", () => {
                    return true;
                })
                .Do("Custom Action", () => {
                    return TaskStatus.Success;
                })
            .End()
            .Build();
    }


    void Update()
    {
        
    }

    public void StartPatrolling()
    {
        NavMeshPath path = new NavMeshPath();
        float closestDistance = float.PositiveInfinity;
        int closestPointIndex = 0;
        for (int i = 0; i < patrollingPoints.Length; i++)
        {
            float distance = 0;
            agent.CalculatePath(patrollingPoints[i], path);
            for (int j = 0; j < path.corners.Length; j++)
            {
                distance += Vector3.Distance(path.corners[j - 1], path.corners[j]);
            }
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPointIndex = i;
            }
        }
        currentIndex = closestPointIndex;
    }

    private IEnumerator MoveToNextPoint()
    {
        state.HasReachedNextPoint = false;
        agent.SetDestination(patrollingPoints[currentIndex]);
        while (agent.pathPending)
        {
            yield return null;
        }
        while (!Mathf.Approximately(agent.remainingDistance, 0))
        {
            yield return null;
        }
        state.HasReachedNextPoint = true;
    }
}
