using Demo.attribute;
using Demo.models;

namespace Demo;

public interface IRobotAction
{
    Task DoAction(HttpContext context,VxRobotVm? model);

    RobotAction getRobotActionAttr();
}