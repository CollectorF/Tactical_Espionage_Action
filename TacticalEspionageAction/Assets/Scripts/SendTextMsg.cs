using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using UnityEngine;
using UnityEngine.AI;

public class SendTextMsg : ActionBase
{
    public string Msg;

    protected override TaskStatus OnUpdate()
    {
        Debug.Log(Msg);
        return TaskStatus.Success;
    }
}
