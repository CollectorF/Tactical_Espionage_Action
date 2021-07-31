using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

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

public class SendTextMsg : ActionBase
{
    public string Msg;

    protected override TaskStatus OnUpdate()
    {
        Debug.Log(Msg);
        return TaskStatus.Success;
    }
}

public class SetVisualStateHint : ActionBase
{
    public Color ColorToSet;
    public string TextToSet;
    private Guard guard;

    protected override void OnInit()
    {
        guard = Owner.GetComponent<Guard>();
    }

    protected override TaskStatus OnUpdate()
    {
        guard.statePodText.text = TextToSet;
        guard.statePodText.color = ColorToSet;
        return TaskStatus.Success;
    }
}
