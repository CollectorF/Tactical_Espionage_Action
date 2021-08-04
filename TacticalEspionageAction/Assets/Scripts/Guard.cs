using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

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
    private GameObject statePod;
    [SerializeField]
    private GuardStarterInfo starterInfo;

    public GuardState state;
    public TMP_Text statePodText;
    private int currentIndex;
    private int direction = -1;
    private Vision vision;
    internal Coroutine guardCoroutine;
    private Camera playerCamera;
    private float waitTime;
    private float speedWhenPatrolling;
    private float speedWhenSuspicious;

    public delegate void PlayerCaughtUp();
    public event PlayerCaughtUp TouchPlayer;
    public delegate void PlayerSpotted();
    public event PlayerSpotted SpotPlayer;
    public delegate void PlayerLost();
    public event PlayerLost LoosePlayer;

    private void Awake()
    {
        playerCamera = Camera.main;

        vision = GetComponentInChildren<Vision>();
        vision.SpotPlayer += VisionSpotPlayer;
        vision.SeePlayer += VisionSeePlayer;
        statePodText = statePod.GetComponentInChildren<TMP_Text>();

        waitTime = starterInfo.waitTime;
        speedWhenPatrolling = starterInfo.speedWhenPatrolling;
        speedWhenSuspicious = starterInfo.speedWhenSuspicious;

        state = new GuardState()
        {
            HasReachedNextPoint = false,
            Status = GuardStatus.Nothing
        };

        var injectTree = new BehaviorTreeBuilder(gameObject)
            .Selector()

                .Sequence("Start patrolling")
                    .Condition("If we do nothing", () =>
                    {
                        return state.Status == GuardStatus.Nothing;
                    })
                    .Do("Start patrolling", () =>
                    {
                        StartPatrolling();
                        state.HasReachedNextPoint = true;
                        return TaskStatus.Success;
                    })
                    .SetVisualStateHint(null, Color.clear)
                    .SendTextMsg("Send text message", "Starting patrolling")
                    .SetGuardStatus("Set status 'Patrolling'", GuardStatus.Patrolling, speedWhenPatrolling)
                .End()

                .Sequence("Move to next point")
                    .Condition("If we are patrolling", () =>
                    {
                        return state.Status == GuardStatus.Patrolling;
                    })
                    .Condition("If we reached next point", () =>
                    {
                        return state.HasReachedNextPoint == true;
                    })
                    .WaitTime(waitTime)
                    .Do("Move to next point", () =>
                    {
                        MoveToNextPoint();
                        return TaskStatus.Success;
                    })
                .End()

                .Sequence("Chase player")
                    .Condition("If player was spotted", () =>
                    {
                        return state.Status == GuardStatus.Suspicious;
                    })
                    .SetVisualStateHint("!", Color.red)
                    .Do("Move to last player point", () =>
                    {
                        agent.speed = speedWhenSuspicious;
                        MoveToPosition(state.LastPlayerPosition);
                        return TaskStatus.Success;
                    })
                        .Sequence("Continue patrolling")
                            .Condition("If we reached last player point", () =>
                            {
                                return state.HasReachedNextPoint == true;
                            })
                            .SetVisualStateHint("?", Color.yellow)
                            .WaitTime(waitTime)
                            .Do("Look around", () =>
                            {
                                LookAround(1f);
                                return TaskStatus.Success;
                            })
                            .WaitTime(waitTime)
                            .Do("Look around", () =>
                            {
                                LookAround(-1f);
                                return TaskStatus.Success;
                            })
                            .WaitTime(waitTime)
                            .SetVisualStateHint(null, Color.clear)
                            .Do("Look around", () =>
                            {
                                LoosePlayer?.Invoke();
                                return TaskStatus.Success;
                            })
                            .SendTextMsg("Send text message", "Returning to patrolling")
                            .SetGuardStatus("Set status 'Patrolling'", GuardStatus.Patrolling, speedWhenPatrolling)
                            .Do("Move to next point", () =>
                            {
                                MoveToNextPoint();
                                return TaskStatus.Success;
                            })
                        .End()
                .End()
            .End();

        guardBehaviorTree = new BehaviorTreeBuilder(gameObject)
            .Splice(injectTree.Build())
            .Build();
    }

    private void Update()
    {
        guardBehaviorTree.Tick();
        statePod.transform.eulerAngles = new Vector3(20f, playerCamera.transform.rotation.y - 90f, 0);
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
        SpotPlayer?.Invoke();
    }

    private void LookAround(float delta)
    {
        var currentPosition = agent.transform.position;
        agent.SetDestination(new Vector3(currentPosition.x + delta, currentPosition.y, currentPosition.z + delta));
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            TouchPlayer?.Invoke();
        }
    }
}
