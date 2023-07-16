using Demo.attribute;
using Demo.models;

namespace Demo.actions;

[RobotAction("Context.StartsWith(\"Command1\") AND From == \"123\"")]
public class Command1 : BaseRobotAction
{
    public override Task Do(HttpContext context, VxRobotVm model)
    {
        Console.WriteLine($"{model.From} : {model.Context}");
        return Task.CompletedTask;
    }
}