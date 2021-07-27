using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using UnityEngine;
using UnityEngine.AI;

public class SetGuardStatus : ActionBase
{
    private NavMeshAgent agent;
    private Guard guard;
    public GuardStatus Status;
    public float Speed;

    protected override void OnInit()
    {
        agent = Owner.GetComponent<NavMeshAgent>();
        guard = Owner.GetComponent<Guard>();
    }
    protected override TaskStatus OnUpdate()
    {
        agent.speed = Speed;
        guard.state.Status = Status;
        return TaskStatus.Success;
    }
}
