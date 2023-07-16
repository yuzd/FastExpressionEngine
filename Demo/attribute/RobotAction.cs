using Autofac.Annotation;

namespace Demo.attribute;

/// <summary>
/// 自定义注解
/// </summary>
[Component]
public class RobotAction : Attribute
{

    /// <summary>
    /// 默认注册到容器为IRobotAction类型
    /// </summary>
    [AliasFor(typeof(Component), "Services")]
    public Type[] Services { get; set; } = new[] { typeof(IRobotAction) };
    
    public RobotAction(string expression)
    {
        Expression = expression;
    }

    /// <summary>
    /// 容器中拿此类的时候执行的方法
    /// </summary>
    [AliasFor(typeof(Component), "InitMethod")]

    public string InitMethod { get; set; } = nameof(BaseRobotAction.Init);
    
    /// <summary>
    /// 表达式
    /// </summary>
    public string Expression { get; set; }
}