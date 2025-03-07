using System.Diagnostics;
using System.Reflection;
using ArxOne.MrAdvice.Advice;
using Serilog;
using Serilog.Context;

namespace Api.Services;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
public class LogReturnValueAsyncAttribute : Attribute, IMethodAsyncAdvice
{
    public async Task Advise(MethodAsyncAdviceContext context)
    {
        var sourceContext = context.TargetMethod.DeclaringType?.FullName ?? "UnknownSource";
        using (LogContext.PushProperty("SourceContext", sourceContext))
        {
            await context.ProceedAsync();
            
            var returnValue = context.ReturnValue;
            Log.Information("Method \"{MethodName}\" returned: {@ReturnValue}",
                context.TargetMethod.Name, returnValue);

        }
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
public class LogReturnValueAttribute : Attribute, IMethodAdvice
{
    public void Advise(MethodAdviceContext context)
    {
        var sourceContext = context.TargetMethod.DeclaringType?.FullName ?? "UnknownSource";
        using (LogContext.PushProperty("SourceContext", sourceContext))
        {
            context.Proceed(); // Proceed with the original method execution

            var returnValue = context.ReturnValue;
            Log.Information("Method \"{MethodName}\" returned: {@ReturnValue}",
                context.TargetMethod.Name, returnValue);
        }
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class LogMethodParametersAttribute : Attribute, IMethodAdvice
{
    public void Advise(MethodAdviceContext context)
    {
        
        // Get the current method info
        MethodBase method = context.TargetMethod;
        var methodName = $"{method.Name}";
        var methodClass = method.DeclaringType?.FullName; 
        
        // Get method arguments
        var args = method.GetParameters()
            .Select((p, i) => $"{p.Name}: {context.Arguments[i]}")
            .ToArray();
        string argsString = string.Join(", ", args);
        
        var sourceContext = context.TargetMethod.DeclaringType?.FullName ?? "UnknownSource";
        using (LogContext.PushProperty("SourceContext", sourceContext))
        {
            Log.Information("Method Called: {MethodName}({Arguments}) in {MethodClass}", 
                methodName, argsString, methodClass);
        
        }
        
        // Proceed with the original method execution
        context.Proceed();
    }
}