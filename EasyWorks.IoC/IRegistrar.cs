using System;
using System.Reflection;

namespace EasyWorks.IoC
{
    public interface IRegistrar
    {
        IRegistrar Register<TService>(Func<TService> factory, Predicate<TService> reusePredicate = null)
            where TService : class;

        IRegistrar UnRegister<TService>();
        IRegistrar RegisterType(Type serviceType, bool reuse = true, params Assembly[] assemblies);

        IRegistrar Register<TService>(Func<TService> factory, bool reuse = true)
            where TService : class;

        IRegistrar Register<TService>(TService instance)
            where TService : class;

        IRegistrar Register<TService, TImplementation>()
            where TService : class
            where TImplementation : TService, new();

        IRegistrar Register<TService>(Predicate<TService> reusePredicate = null)
            where TService : class;

        IRegistrar Register<TService, TImplementation>(Predicate<TService> reusePredicate = null)
            where TService : class
            where TImplementation : class, TService;

        Func<object> GetFactory(Type type, params Assembly[] assemblies);

        Func<TService> GetFactory<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
    }
}
