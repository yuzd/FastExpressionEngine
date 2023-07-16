namespace Demo.models;

public class VxRobotVm
{
    /// <summary>
    /// 发送内容
    /// </summary>
    public string Context { get; set; }
    
    /// <summary>
    /// 群id
    /// </summary>
    public string From { get; set; }
    
    /// <summary>
    /// 发送微信id
    /// </summary>
    public string To { get; set; }
}
