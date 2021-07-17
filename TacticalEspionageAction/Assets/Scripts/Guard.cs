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
    [SerializeField]
    private float waitTime;

    private GuardState state;
    private int currentIndex;
    private int direction = -1;

    private void Awake()
    {
        state = new GuardState()
        {
            HasReachedNextPoint = false,
            Status = GuardStatus.Nothing
        };

        _tree = new BehaviorTreeBuilder(gameObject)
            .Selector()
                .Sequence("Start patrolling")
                    .Condition("If we do nothing", () => {
                        return state.Status == GuardStatus.Nothing;
                    })
                    .Do("Start patrolling", () => {
                        StartPatrolling();
                        state.HasReachedNextPoint = true;
                        //MoveToNextPoint();
                        return TaskStatus.Success;
                    })
                    .Do("Set guard status", () => {
                        state.Status = GuardStatus.Patrolling;
                        return TaskStatus.Success;
                    })
                .End()

                .Sequence("Move to next point")
                    .Condition("If we are patrolling", () => {
                        return state.Status == GuardStatus.Patrolling;
                    })
                    .Condition("If we reaced next point", () => {
                        return state.HasReachedNextPoint == true;
                    })
                    .WaitTime(waitTime)
                    .Do("Move to next point", () => {
                        MoveToNextPoint();
                        return TaskStatus.Success;
                    })
                .End()
            .End()
            .Build();
    }

    private void Update()
    {
        _tree.Tick();
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
            for (int j = 1; j < path.corners.Length; j++)
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

    public void MoveToNextPoint()
    {
        StartCoroutine(MoveToNextPointCoroutine());
    }

    private IEnumerator MoveToNextPointCoroutine()
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
        if (currentIndex == patrollingPoints.Length - 1)
        {
            direction = -1;
        }
        else if (currentIndex == 0)
        {
            direction = 1;
        }
        currentIndex += direction;
        state.HasReachedNextPoint = true;
    }
}
