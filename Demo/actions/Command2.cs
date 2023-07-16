using Demo.attribute;
using Demo.models;

namespace Demo.actions;

[RobotAction("Context.StartsWith(\"Command2\") OR From == \"234\"")]
public class Command2 : BaseRobotAction
{
    public override Task Do(HttpContext context, VxRobotVm model)
    {
        Console.WriteLine($"{model.From} : {model.Context}");
        return Task.CompletedTask;
    }
}