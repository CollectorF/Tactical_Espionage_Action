using CleverCrow.Fluid.BTs.Trees;
using UnityEngine;

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

    public static BehaviorTreeBuilder SetVisualStateHint(this BehaviorTreeBuilder builder, string hintText, Color hintColor)
    {
        return builder.AddNode(new SetVisualStateHint
        {
            TextToSet = hintText,
            ColorToSet = hintColor,
        });
    }
}