
using System.Reflection;
using Demo.attribute;
using Demo.models;

namespace Demo;
public abstract class BaseRobotAction : IRobotAction
{
    internal void Init()
    {
        RobotAction = GetType().GetCustomAttribute<RobotAction>()!;
    }
    public abstract Task Do(HttpContext context,VxRobotVm model);
    public RobotAction RobotAction { get; set; }

    public async Task DoAction(HttpContext context,VxRobotVm? model)
    {
        await Do(context,model!);
    }

    public RobotAction getRobotActionAttr()
    {
        return RobotAction;
    }
}