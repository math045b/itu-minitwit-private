using System.Reflection;
using ArxOne.MrAdvice.Advice;
using Serilog;
using Serilog.Context;

namespace Api.Services;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
public class LogReturnValueAttribute : Attribute, IMethodAsyncAdvice
{
    public async Task Advise(MethodAsyncAdviceContext context)
    {
        var sourceContext = context.TargetMethod.DeclaringType?.FullName ?? "UnknownSource";
        using (LogContext.PushProperty("SourceContext", sourceContext))
        {
            await context.ProceedAsync();
            
            var returnValue = context.ReturnValue;
            Log.Information("Method \"{MethodName}\" returned {@ReturnValue}",
                context.TargetMethod.Name, returnValue);

        }
    }
}

[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public class LogMethodParametersAttribute : Attribute, IMethodAdvice
{
    public void Advise(MethodAdviceContext context)
    {
        // Retrieve method information
        var method = context.TargetMethod as MethodInfo;
        if (method == null)
        {
            context.Proceed();
            return;
        }

        // Get parameter names and values
        var parameters = method.GetParameters();
        var arguments = context.Arguments;

        var parameterInfo = parameters
            .Select((p, i) => $"{p.Name} = {arguments[i] ?? "null"}")
            .ToArray();

        var sourceContext = context.TargetMethod.DeclaringType?.FullName ?? "UnknownSource";
        using (LogContext.PushProperty("SourceContext", sourceContext))
        {
            Log.Information("Method \"{MethodName}\" called with {@parameterInfo}",
                context.TargetMethod.Name, parameterInfo);

        }
        
        // Proceed with the original method execution
        context.Proceed();
    }
}