using CleverCrow.Fluid.BTs.Trees;

public static class BehaviorTreeBuilderExtensions
{
    public static BehaviorTreeBuilder SetGuardStatus(this BehaviorTreeBuilder builder, string name, GuardStatus status, float speed)
    {
        return builder.AddNode(new SetGuardStatus
        {
            Status = status,
            Speed = speed,
            Name = name,
        });
    }

    public static BehaviorTreeBuilder SendTextMsg(this BehaviorTreeBuilder builder, string name, string msg)
    {
        return builder.AddNode(new SendTextMsg
        {
            Msg = msg,
            Name = name,
        });
    }
}