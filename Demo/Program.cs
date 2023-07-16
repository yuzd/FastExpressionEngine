using System.Text.Json;
using Autofac;
using Autofac.Annotation;
using Autofac.Extensions.DependencyInjection;
using Demo;
using Demo.models;
using FastExpressionEngine;

var builder = WebApplication.CreateBuilder();

// 设置使用autofac作为DI容器
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
// 设置DI容器 添加注解功能
builder.Host.ConfigureContainer<ContainerBuilder>((c, containerBuilder) =>
{
    // 在这里 在这里 在这里
    containerBuilder.RegisterModule(new AutofacAnnotationModule().SetDefaultAutofacScopeToSingleInstance());
    containerBuilder.RegisterInstance(new ExpressionEngine());
});

builder.WebHost.UseUrls("http://0.0.0.0:5555");

var app = builder.Build();
app.MapPost("/robot", async (context) =>
{
    var result = new ResultJsonInfo<string>
    {
        Status = 1
    };
    try
    {
        // 读取post body
        var robotMsg = await ReadBodyAsync<VxRobotVm>(context.Request.Body);
        // 从容器中拿到表达式引擎
        var engine = context.RequestServices.GetAutofacRoot().Resolve<ExpressionEngine>();
        // 从容器中拿到注册为robotAction的所有实例
        var actions = context.RequestServices.GetAutofacRoot().Resolve<IEnumerable<IRobotAction>>();
        foreach (var action in actions)
        {
            var robotActionAttr = action.getRobotActionAttr();
            if (!engine.Execute(robotActionAttr.Expression, robotMsg).IsSuccess) continue;
            // 找到满足条件的action
            await action.DoAction(context,robotMsg);
            break;
        }
    }
    finally
    {
        await context.Response.WriteAsJsonAsync(result);
    }
});

Console.WriteLine(AppContext.BaseDirectory);
app.Run();
 static async Task<T?> ReadBodyAsync<T>( Stream bodyStream)
{
    try
    {
        var body = "";
        using (StreamReader stream = new StreamReader(bodyStream))
        {
            body = await stream.ReadToEndAsync();
        }
        return  JsonSerializer.Deserialize<T>(body);
    }
    catch (Exception e)
    {
        return default;
    }
}
