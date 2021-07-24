using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using System;
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
    public Vector3 LastPlayerPosition;
}

public class Guard : MonoBehaviour
{
    [SerializeField]
    private BehaviorTree guardBehaviorTree;
    [SerializeField]
    private Vector3[] patrollingPoints;
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private float speedWhenPatrolling = 2f;
    [SerializeField]
    private float speedWhenSuspicious = 3f;

    private GuardState state;
    private int currentIndex;
    private int direction = -1;
    private Vision vision;
    internal Coroutine guardCoroutine;

    public delegate void PlayerCaughtUp();
    public event PlayerCaughtUp TouchPlayer;

    private void Awake()
    {
        vision = GetComponentInChildren<Vision>();
        vision.SpotPlayer += VisionSpotPlayer;
        vision.SeePlayer += VisionSeePlayer;

        state = new GuardState()
        {
            HasReachedNextPoint = false,
            Status = GuardStatus.Nothing
        };

        guardBehaviorTree = new BehaviorTreeBuilder(gameObject)
            .Selector()

                .Sequence("Start patrolling")
                    .Condition("If we do nothing", () => {
                        return state.Status == GuardStatus.Nothing;
                    })
                    .Do("Start patrolling", () => {
                        StartPatrolling();
                        state.HasReachedNextPoint = true;
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
                    .Condition("If we reached next point", () => {
                        return state.HasReachedNextPoint == true;
                    })
                    .WaitTime(waitTime)
                    .Do("Move to next point", () => {
                        agent.speed = speedWhenPatrolling;
                        MoveToNextPoint();
                        return TaskStatus.Success;
                    })
                .End()

                .Sequence("Chase player")
                    .Condition("If player was spotted", () =>
                    {
                        return state.Status == GuardStatus.Suspicious;
                    })
                    .Do("Move to last player point", () => {
                        agent.speed = speedWhenSuspicious;
                        MoveToPosition(state.LastPlayerPosition);
                        return TaskStatus.Success;
                    })
                        .Sequence("Continue patrolling")
                            .Condition("If we reached last player point", () =>
                            {
                                return state.HasReachedNextPoint == true;
                            })
                            .WaitTime(waitTime)
                            .Do("Look around", () => {
                                LookAround(1f);
                                return TaskStatus.Success;
                            })
                            .WaitTime(waitTime)
                            .Do("Look around", () => {
                                LookAround(-1f);
                                return TaskStatus.Success;
                            })
                            .WaitTime(waitTime)
                            .Do("Set guard status", () =>
                            {
                                Debug.Log("Returning to patrolling");
                                agent.speed = speedWhenPatrolling;
                                state.Status = GuardStatus.Patrolling;
                                return TaskStatus.Success;
                            })
                            .Do("Move to next point", () => {
                                MoveToNextPoint();
                                return TaskStatus.Success;
                            })
                        .End()
                .End()
            .End()
            .Build();
    }

    private void Update()
    {
        guardBehaviorTree.Tick();
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
        state.HasReachedNextPoint = false;
        if (guardCoroutine != null)
        {
            StopCoroutine(guardCoroutine);
        }
        guardCoroutine = StartCoroutine(MoveToNextPointCoroutine(patrollingPoints[currentIndex], ChooseNextPoint));
    }

    public void MoveToPosition(Vector3 nextPosition)
    {
        state.HasReachedNextPoint = false;
        if (guardCoroutine != null)
        {
            StopCoroutine(guardCoroutine);
        }
        guardCoroutine = StartCoroutine(MoveToNextPointCoroutine(nextPosition, null));
    }

    private IEnumerator MoveToNextPointCoroutine(Vector3 nextPosition, Action OnFinishedMovement)
    {
        agent.SetDestination(nextPosition);
        while (agent.pathPending)
        {
            yield return null;
        }
        while (!Mathf.Approximately(agent.remainingDistance, 0))
        {
            yield return null;
        }
        OnFinishedMovement?.Invoke();
        state.HasReachedNextPoint = true;
    }
    private void ChooseNextPoint()
    {
        if (currentIndex == patrollingPoints.Length - 1)
        {
            direction = -1;
        }
        else if (currentIndex == 0)
        {
            direction = 1;
        }
        currentIndex += direction;
    }
    private void VisionSeePlayer(GameObject target)
    {
        state.LastPlayerPosition = target.transform.position;
        state.Status = GuardStatus.Suspicious;
    }

    private void VisionSpotPlayer(GameObject target)
    {
        Debug.Log("I see you!");
    }

    private void LookAround(float delta)
    {
        var currentPosition = agent.transform.position;
        agent.SetDestination(new Vector3(currentPosition.x + delta, currentPosition.y, currentPosition.z + delta));
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("collision!");
        if (other.transform.CompareTag("Player"))
        {
            
            TouchPlayer?.Invoke();
        }
    }
}
