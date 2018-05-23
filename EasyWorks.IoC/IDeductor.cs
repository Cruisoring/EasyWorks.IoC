using System;
using System.Reflection;

namespace EasyWorks.IoC
{
    /// <summary>
    /// Factory holders of any registered services.
    /// </summary>
    public interface IDeductor
    {
        TService Resolve<TService>(params Assembly[] assemblies) where TService : class;
        object Resolve(Type serviceType, params Assembly[] assemblies);
        void Clear();

        int Bind<TCause, TResult>(Assembly assembly, Func<Type, String> keyFromType = null)
            where TCause : class
            where TResult : class;

        int Bind<TCause, TResult>(Assembly[] assemblies, Func<Type, String> keyFromType = null)
            where TCause : class
            where TResult : class;

        TResult Induce<TCause, TResult>(TCause instance)
            where TResult : class;

        TResult Induce<TResult>(Type causeType)
            where TResult : class;

        TCause Suggest<TCause, TResult>(TResult instance)
            where TCause : class;

        TCause Suggest<TCause>(Type resultType)
            where TCause : class;
    }
}
