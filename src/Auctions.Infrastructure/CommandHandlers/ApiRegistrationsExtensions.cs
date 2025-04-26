using System.Linq.Expressions;
using System.Reflection;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Services;

// ReSharper disable InconsistentNaming

namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;
/// <summary>
/// Api registrations logic. Potentially some of the lambda compiled logic could be made into Roslyn generated code.
/// Note the use of decorators in order to be able to share the logic between the different types of command handlers.
/// </summary>
public static class ApiRegistrationsExtensions
{
    public static IServiceCollection RegisterAttributesForType<T>(this IServiceCollection services) where T : IEntity =>
        RegisterAttributesForType(services, typeof(T));

    private static IServiceCollection RegisterAttributesForType(this IServiceCollection services, Type t)
    {
        foreach (var method in
                 from m in t.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public |
                                        BindingFlags.NonPublic)
                 let attr = m.GetCustomAttribute<CommandHandlerAttribute>()
                 where attr != null
                 select m)
        {
            if (RegisterCreateCommandHandlers(services, t, method))
            {
                continue;
            }

            if (RegisterUpdateCommandHandlers(services, t, method))
            {
                continue;
            }

            throw new Exception("Could not interpret commandhandler for method")
                { Data = { { "Method", method.Name } } };
        }

        return services;
    }

    private static bool RegisterCreateCommandHandlers(IServiceCollection services, Type t, MethodInfo method)
    {
        if (!method.IsStatic)
        {
            return false;
        }

        if (RegisterOnlyServiceProviderCreateCommandHandler(services, t, method))
        {
            return true;
        }

        if (RegisterServiceCreateCommandHandler(services, t, method))
        {
            return true;
        }

        return false;
    }

    static bool RegisterOnlyServiceProviderCreateCommandHandler(IServiceCollection services, Type t, MethodInfo method)
    {
        var parameters = method.GetParameters();
        if (method.ReturnType == t
            && parameters.Length == 2
            && parameters[1].ParameterType == typeof(IServiceProvider))
            // select (method, parameters[0].ParameterType, method.ReturnType))
        {
            var returnType = method.ReturnType;
            var commandType = parameters[0].ParameterType;
            var regType = typeof(ICommandHandler<,>).MakeGenericType(commandType, returnType);
            Func<IServiceProvider, object> serviceFactory =
                CreateFuncCreateOnlyServiceProviderFactory(t, method, commandType);
            services.AddScoped(regType, svc =>serviceFactory(svc));
            return true;
        }

        return false;
    }

    /// <summary>
    /// This method turns the methodInfo into the following lambda method that returns an instance of FuncCreateCommandHandler
    /// new FuncCreateCommandHandler &lt;TEntity,TCommand&gt; ((entity, cmd, svcProvider) =>
    /// `EntityType`.`MethodInfo`(cmd, svcProvider), outerSvcProvider)
    /// </summary>
    static Func<IServiceProvider, object> CreateFuncCreateOnlyServiceProviderFactory(Type t, MethodInfo methodInfo,
        Type commandType)
    {
        var implType = typeof(FuncCreateCommandHandler<,>).MakeGenericType(t, commandType);
        //<TCommand, IServiceProvider, T>
        var funcType = typeof(Func<,,>).MakeGenericType(commandType, typeof(IServiceProvider), t);
        var parameter_Svc = Expression.Parameter(typeof(IServiceProvider), "svc");
        // (cmd, svcProvider) => `EntityType`.`MethodInfo`(cmd, svcProvider)
        Func<IServiceProvider, object> lambda = Expression.Lambda<Func<IServiceProvider, object>>(
                    Expression.New( // new
                    implType.GetConstructors()[0], // FuncCreateCommandHandler<TEntity,TCommand>
                    Expression.Constant(Delegate.CreateDelegate(funcType, methodInfo)),
                    parameter_Svc,
            parameter_Svc)).Compile();
        return lambda;
    }

    static bool RegisterServiceCreateCommandHandler(IServiceCollection services, Type t, MethodInfo method)
    {
        var parameters = method.GetParameters();
        var restParameters = parameters.Skip(1).ToList();
        switch (parameters.Length)
        {
            case >= 1
                when restParameters.TrueForAll(p => p.ParameterType != typeof(IServiceProvider)):
            {
                var commandType = parameters[0].ParameterType;
                var serviceParameters = restParameters.Select(p => p.ParameterType).ToArray();
                var returnType = method.ReturnType;
                var regType = typeof(ICommandHandler<,>).MakeGenericType(commandType, returnType);
                Func<IServiceProvider, object> serviceFactory =
                    CreateFuncCreateServiceFactory(t, method, commandType, serviceParameters);
                services.AddScoped(regType, svc => serviceFactory(svc));
                return true;
            }
            default:
                return false;
        }
    }

    /// <summary>
    /// This method turns the methodInfo into the following lambda method that returns an instance of FuncCreateCommandHandler
    /// new FuncCreateCommandHandler &lt;TEntity,TCommand&gt; ((entity, cmd, svcProvider) =>
    ///     `EntityType`.`MethodInfo`(cmd, svcProvider.GetRequiredService  &lt; TService &gt;() ..), outerSvcProvider)
    /// </summary>
    static Func<IServiceProvider, object> CreateFuncCreateServiceFactory(Type t, MethodInfo methodInfo,
        Type commandType, Type[] serviceParameters)
    {
        var implType = typeof(FuncCreateCommandHandler<,>).MakeGenericType(t, commandType);
        //<TCommand, IServiceProvider, T>
        var parameter_Svc = Expression.Parameter(typeof(IServiceProvider), "svc");
        var parameter_Cmd = Expression.Parameter(commandType, "cmd");
        var parameters = from p in serviceParameters
            from m in typeof(ServiceProviderServiceExtensions).GetMethods()
            where m.Name == nameof(ServiceProviderServiceExtensions.GetRequiredService)
                  //&& m.IsGenericMethod
                  && m.IsGenericMethodDefinition
            let getRequiredService = m.MakeGenericMethod(p)
            select (Expression)Expression.Call(getRequiredService, parameter_Svc);
        // (cmd, svc) => `EntityType`.`MethodInfo`(cmd, svcProvider.GetRequiredService<TService>() ...)
        Func<IServiceProvider, object> lambda = Expression.Lambda<Func<IServiceProvider, object>>(
            Expression.New( // new
                implType.GetConstructors()[0], // FuncCreateCommandHandler<T,TCommand>
                Expression.Lambda( // (cmd, svcProvider) =>
                    Expression.Call(methodInfo, new Expression[] { parameter_Cmd }.Union(parameters).ToArray()),
                    parameter_Cmd, parameter_Svc),
                parameter_Svc
            ),
            parameter_Svc).Compile();
        return lambda;
    }


    private static bool RegisterUpdateCommandHandlers(IServiceCollection services, Type t, MethodInfo method)
    {
        if (RegisterFuncMutateServices(services, t, method))
        {
            return true;
        }

        if (RegisterFuncMutateOnlyServiceProvider(services, t, method))
        {
            return true;
        }

        return false;
    }

    static bool RegisterFuncMutateServices(IServiceCollection services, Type t, MethodInfo method)
    {
        var parameters = method.GetParameters();
        var restParameters = parameters.Skip(1).ToList();
        switch (parameters.Length)
        {
            case >= 1
                when restParameters.TrueForAll(p => p.ParameterType != typeof(IServiceProvider)):
            {
                var commandType = parameters[0].ParameterType;
                var serviceParameters = restParameters
                    .Select(p => p.ParameterType).ToArray();
                var returnType = method.ReturnType == typeof(void) ? typeof(Unit) : method.ReturnType;
                var handlerTCommand = typeof(ICommandHandler<,>).MakeGenericType(commandType, returnType);
                Func<IServiceProvider, object> serviceFactory =
                    CreateFuncMutateServicesFactory(t, method, commandType, serviceParameters, returnType);
                services.AddScoped(handlerTCommand,svc => serviceFactory(svc));
                return true;
            }
            default:
                return false;
        }
    }


    /// <summary>
    /// This method turns the methodInfo into the following lambda method that returns an instance of FuncMutateCommandHandler
    /// new FuncMutateCommandHandler &lt;TEntity,TCommand, TReturnType&gt; ((entity, cmd, svcProvider) => entity.`MethodInfo`(cmd,
    /// svcProvider.GetRequiredService &lt;IService1&gt; (),
    /// svcProvider.GetRequiredService &lt;IService2&gt; ()
    /// ), outerSvcProvider)
    /// </summary>
    static Func<IServiceProvider, object> CreateFuncMutateServicesFactory(Type t, MethodInfo methodInfo,
        Type commandType, Type[] serviceParameters, Type returnType)
    {
        var funcMutateCommandHandlerTT =
            (returnType.IsAssignableTo(typeof(IResult)), returnType == typeof(Unit)) switch
            {
                (true, _) => typeof(FuncResultMutateCommandHandler<,,>).MakeGenericType(t, commandType, returnType),
                (_, true) => typeof(FuncMutateCommandHandler<,,>).MakeGenericType(t, commandType, typeof(object)),
                _ => typeof(FuncMutateCommandHandler<,,>).MakeGenericType(t, commandType, returnType)
            };
        var parameter_Entity = Expression.Parameter(t, "entity");
        var parameter_Cmd = Expression.Parameter(commandType, "cmd");
        var parameter_Svc = Expression.Parameter(typeof(IServiceProvider), "svc");

        var parameters = from p in serviceParameters
            from m in typeof(ServiceProviderServiceExtensions).GetMethods()
            where m.Name == nameof(ServiceProviderServiceExtensions.GetRequiredService)
                  //&& m.IsGenericMethod
                  && m.IsGenericMethodDefinition
            let getRequiredService = m.MakeGenericMethod(p)
            select (Expression)Expression.Call(getRequiredService, parameter_Svc);
        // (entity, cmd, svcProvider) => entity.`MethodInfo`(cmd, svcProvider)
        Func<IServiceProvider, object> lambda = Expression.Lambda<Func<IServiceProvider, object>>(
                Expression.New( // new
                    funcMutateCommandHandlerTT
                        .GetConstructors()[0], // FuncMutateCommandHandler<TEntity,TCommand,TReturnType>
                    Expression.Lambda( // (entity, cmd, svcProvider) =>
                        // entity.`MethodInfo`(cmd, svc.GetRequiredService<T>() ... ) , i.e. entity.HandleTheCommand(cmd,svc)
                        Expression.Call(parameter_Entity, methodInfo,
                            new Expression[] { parameter_Cmd }.Union(parameters).ToArray()),
                        parameter_Entity, parameter_Cmd, parameter_Svc),
                    parameter_Svc
                ), parameter_Svc)
            .Compile();
        return lambda;
    }

    static bool RegisterFuncMutateOnlyServiceProvider(IServiceCollection services, Type t, MethodInfo method)
    {
        var parameters = method.GetParameters();
        switch (parameters.Length)
        {
            case 2 when parameters[1].ParameterType == typeof(IServiceProvider):
            {
                var commandType = parameters[0].ParameterType;
                var returnType = method.ReturnType == typeof(void) ? typeof(Unit) : method.ReturnType;
                var handlerTCommand = typeof(ICommandHandler<,>).MakeGenericType(commandType, returnType);
                Func<IServiceProvider, object> serviceFactory =
                    CreateFuncMutateOnlyServiceProviderFactory(t, method, commandType, returnType);
                services.AddScoped(handlerTCommand, svc => serviceFactory(svc));
                return true;
            }
            default:
                return false;
        }
    }

    /// <summary>
    /// This method turns the methodInfo into the following lambda method that returns an instance of FuncMutateCommandHandler
    /// new FuncMutateCommandHandler &lt;TEntity,TCommand,TReturnType&gt; ((entity, cmd, svcProvider) =>
    /// entity.`MethodInfo`(cmd, svcProvider), outerSvdProvider)
    /// </summary>
    static Func<IServiceProvider, object> CreateFuncMutateOnlyServiceProviderFactory(Type t, MethodInfo methodInfo,
        Type commandType, Type returnType)
    {
        var funcMutateCommandHandlerTT = returnType == typeof(Unit)
            ? typeof(FuncMutateCommandHandler<,,>).MakeGenericType(t, commandType, typeof(object))
            : typeof(FuncMutateCommandHandler<,,>).MakeGenericType(t, commandType, returnType);
        var parameter_Entity = Expression.Parameter(t, "entity");
        var parameter_Cmd = Expression.Parameter(commandType, "cmd");
        var parameter_Svc = Expression.Parameter(typeof(IServiceProvider), "svc");
        // (entity, cmd, svcProvider) => entity.`MethodInfo`(cmd, svcProvider)
        Func<IServiceProvider, object> lambda = Expression.Lambda<Func<IServiceProvider, object>>(
                Expression.New( // new
                    funcMutateCommandHandlerTT
                        .GetConstructors()[0], // FuncMutateCommandHandler<TEntity,TCommand,TReturnType>
                    Expression.Lambda( // (entity, cmd, svcProvider) =>
                        // entity.`MethodInfo`(cmd, svcProvider) , i.e. entity.HandleTheCommand(cmd,svcProvider)
                        Expression.Call(parameter_Entity, methodInfo, parameter_Cmd, parameter_Svc),
                        parameter_Entity, parameter_Cmd, parameter_Svc),
                    parameter_Svc
                ), parameter_Svc)
            .Compile();
        return lambda;
    }
}