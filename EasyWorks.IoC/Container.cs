using DataCenter.Common;
using System;

namespace EasyWorks.IoC
{
    public class Container //: IContainer, IDisposable
    {


        protected readonly Repository<Type, ServiceProvider> Services = new Repository<Type, ServiceProvider>();
        protected readonly Repository<String, Dependency> Dependencies = new Repository<string, Dependency>();

        public void Clear()
        {
            Services.Clear();
            Dependencies.Clear();
        }

        /*/


        public int Bind<TCause, TResult>(Assembly assembly, Func<Type, String> keyFromType = null)
            where TCause : class
            where TResult : class
        {
            return Bind<TCause, TResult>(new[] { assembly }, keyFromType);
        }

        public int Bind<TCause, TResult>(Assembly[] assemblies, Func<Type, String> keyFromType = null)
            where TCause : class
            where TResult : class
        {
            keyFromType = keyFromType ?? firstWordOfTypeName;
            var causes = assemblies.SelectMany(a => solidAssignables(a, typeof(TCause)))
                .Select(ti => ti.AsType()).ToList();
            var results = assemblies.SelectMany(a => solidAssignables(a, typeof(TResult)))
                .Select(ti => ti.AsType()).ToList();

            causes.Concat(results).ToObservable().ForEachAsync(t =>
            {
                if (!Services.ContainsKey(t))
                    this.RegisterType(t);
            });

            var causeDict = causes.ToDictionary(keyFromType, t => t);
            var resultDict = results.ToDictionary(keyFromType, t => t);

            string causeKey = typeof(TCause).FullName;
            string resultKey = typeof(TResult).FullName;

            var deductionList = causeDict.Join(resultDict,
                    c => c.Key,
                    r => r.Key,
                    (c, r) => new Deduction(c.Value, r.Value))
                .ToList();
            ;

            int count = 0;

            foreach (var deduction in deductionList)
            {
                String deductionKey = deduction.ToString();
                if (Deductions.ContainsKey(deductionKey))
                    continue;

                count++;
                Deductions.Add(deductionKey, deduction);
                deductionKey = $"{deduction.CauseType.FullName}{Deduction.CuaseResultConnector}{resultKey}";
                if (!Deductions.ContainsKey(deductionKey))
                    Deductions.Add(deductionKey, deduction);
                deductionKey = $"{causeKey}{Deduction.CuaseResultConnector}{deduction.ResultType.FullName}";
                if (!Deductions.ContainsKey(deductionKey))
                    Deductions.Add(deductionKey, deduction);
            }
            return count;
        }

        public TResult Induce<TCause, TResult>(TCause instance)
            where TResult : class
        {
            Type causeType = instance.GetType();
            return Induce<TResult>(causeType);
        }

        public TResult Induce<TResult>(Type causeType)
            where TResult : class
        {
            String deductionKey = $"{causeType.FullName}{Deduction.CuaseResultConnector}{typeof(TResult).FullName}";
            return Deductions.ContainsKey(deductionKey) ? Deductions[deductionKey].Induce<TResult>() : null;
        }

        public TCause Suggest<TCause, TResult>(TResult instance)
            where TCause : class
        {
            return Suggest<TCause>(instance.GetType());
        }

        public TCause Suggest<TCause>(Type resultType)
            where TCause : class
        {
            String deductionKey = $"{typeof(TCause).FullName}{Deduction.CuaseResultConnector}{resultType.FullName}";
            return Deductions.ContainsKey(deductionKey) ? Deductions[deductionKey].Suggest<TCause>() : null;
        }

        public TService Resolve<TService>(params Assembly[] assemblies)
            where TService : class
        {
            Type serviceType = typeof(TService);
            TService service = (TService)Resolve(serviceType, assemblies);

            if (service == null && serviceType.GetTypeInfo().IsClass)
            {
                Func<TService> factory =
                    GetFactory<TService, TService>(ConstructorPreference.LeastResolveablePreferred);

                service = factory?.Invoke();

                //Should service be saved as singleton now?
            }

            return service;
        }

        public object Resolve(Type serviceType, params Assembly[] assemblies)
        {
            if (!Services.ContainsKey(serviceType) && assemblies.Length > 0)
                RegisterType(serviceType, true, assemblies);

            if (Services.ContainsKey(serviceType))
            {
                ServiceProvider serviceHolder = Services[serviceType];
                return serviceHolder.GetInstance();
            }

            //            if (assembly != null)
            //            {
            //                RegisterType(serviceType, assembly);
            ////                var types = solidAssignables(assembly, serviceType).ToList();
            ////                types = types.Count() == 1 ? types : types.Where(t => t.IsPublic).ToList();
            ////                if (types.Count() == 1)
            ////                {
            ////                    Func<object> factory = GetFactory(types[0].AsType(), assembly);
            ////                    return factory?.Invoke();
            ////                }
            ////                else if (types.Count() > 1)
            ////                {
            ////                    throw new InvalidOperationException("There are two many options: " + String.Join(",", types));
            ////                }
            //            }

            return null;
        }

        public IRegistrar UnRegister<TService>()
        {
            var serviceType = typeof(TService);
            if (Services.ContainsKey(serviceType))
                Services.Remove(serviceType);
            return this;
        }

        private void updateService(Type serviceType, ServiceProvider serviceHolder)
        {
            if (Services.ContainsKey(serviceType))
            {
                Services[serviceType] = serviceHolder;
                Debug.WriteLine($"{serviceType} is replaced with: {serviceHolder}");
            }
            else
            {
                Services.Add(serviceType, serviceHolder);
                Debug.WriteLine($"{serviceType} is added with: {serviceHolder}");
            }

        }

        public IRegistrar RegisterType(Type serviceType, bool reuse = SingletonePreferred, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                TypeInfo typeInfo = serviceType.GetTypeInfo();
                if (!typeInfo.IsClass || typeInfo.IsAbstract)
                    return this;

                Func<object> factory = GetFactory(serviceType);
                if (factory != null)
                {
                    ServiceProvider serviceProvider = new ServiceProvider<object>(factory, reuse);
                    updateService(serviceType, serviceProvider);
                }
            }
            else
            {
                foreach (var assembly in assemblies)
                {
                    var types = solidAssignables(assembly, serviceType).ToList();
                    types = types.Count() == 1 ? types : types.Where(t => t.IsPublic).ToList();
                    if (types.Count() == 1)
                    {
                        Func<object> factory = GetFactory(types[0].AsType(), ConstructorPreference.LeastResolveablePreferred, assemblies);
                        if (factory != null)
                        {
                            ServiceProvider serviceProvider = new ServiceProvider<object>(factory, reuse);
                            updateService(serviceType, serviceProvider);
                        }
                        return this;
                    }
                    else if (types.Count() > 1)
                    {
                        throw new InvalidOperationException("There are two many options: " + String.Join(",", types));
                    }
                }
            }

            return this;

        }

        public IRegistrar Register<TService>(Func<TService> factory, Predicate<TService> reusePredicate = null)
            where TService : class
        {
            ServiceProvider<TService> serviceProvider = new ServiceProvider<TService>(factory, reusePredicate);
            updateService(typeof(TService), serviceProvider);
            return this;
        }


        public IRegistrar Register<TService>(Func<TService> factory, bool reuse = SingletonePreferred)
            where TService : class
        {
            ServiceProvider<TService> serviceProvider = new ServiceProvider<TService>(factory, reuse);
            updateService(typeof(TService), serviceProvider);
            return this;
        }

        public IRegistrar Register<TService>(TService instance)
            where TService : class
        {
            ServiceProvider<TService> serviceProvider = new ServiceProvider<TService>(instance);
            updateService(typeof(TService), serviceProvider);
            return this;
        }

        public IRegistrar Register<TService, TImplementation>()
            where TService : class
            where TImplementation : TService, new()
        {
            Func<TService> factory = () => new TImplementation();
            return Register<TService>(factory, null);
        }

        public IRegistrar Register<TService>(ConstructorPreference strategy = ConstructorPreference.LeastResolveablePreferred,
            Predicate<TService> reusePredicate = null)
            where TService : class
        {
            return Register<TService, TService>(strategy, reusePredicate);
        }

        public IRegistrar Register<TService, TImplementation>(ConstructorPreference strategy = ConstructorPreference.LeastResolveablePreferred,
            Predicate<TService> reusePredicate = null)
            where TService : class
            where TImplementation : class, TService
        {
            Func<TService> factory = GetFactory<TService, TImplementation>(strategy);
            return Register<TService>(factory, reusePredicate);
        }

        public Func<object> GetFactory(Type type, ConstructorPreference strategy = ConstructorPreference.LeastResolveablePreferred, params Assembly[] assemblies)
        {
            //When TImplementation has been registered, use the serviceHolder to generate/get instance.
            if (Services.ContainsKey(type))
            {
                return () => Resolve(type);
            }

            TypeInfo typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsClass || typeInfo.IsAbstract)
                throw new ArgumentException("TImplementation cannot be abstract");

            List<ConstructorInfo> ctors = typeInfo.DeclaredConstructors
                .Where(c => c.IsPublic).ToList();

            if (ctors.Count > 1)
            {
                var parts = ctors.Where(c =>
                        c.GetParameters()
                            .Select(p => p.ParameterType) //ActualTypeOf(p.ParameterType))
                            .All(t => Services.ContainsKey(t)))
                    .ToList();

                if (parts.Count > 0)
                    ctors = parts;
                //else ??? How to handle it???
            }

            ConstructorInfo ctor = ctors.First();
            if (strategy != ConstructorPreference.FirstPreferred)
            {
                ctors = ctors.OrderBy(c => c.GetParameters().Length).ToList();
                ctor = strategy == ConstructorPreference.LeastResolveablePreferred ? ctors.First() : ctors.Last();
            }

            Func<object> factory = () =>
            {
                object[] parameters = ctor.GetParameters().Select(p =>
                    Resolve(p.ParameterType, assemblies)
                        //                    Services[p.ParameterType]//[ActualTypeOf(p.ParameterType)]
                        //                        .GetInstance()
                        ).ToArray();
                return ctor.Invoke(parameters);
            };

            return factory;
        }

        public Func<TService> GetFactory<TService, TImplementation>(ConstructorPreference strategy)
            where TService : class
            where TImplementation : class, TService
        {
            Type type = typeof(TImplementation);
            //When TImplementation has been registered, use the serviceHolder to generate/get instance.
            if (Services.ContainsKey(type))
            {
                return () => (TImplementation)Resolve(type);
            }

            TypeInfo typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsClass || typeInfo.IsAbstract)
                return null;

            List<ConstructorInfo> ctors = typeInfo.DeclaredConstructors
                .Where(c => c.IsPublic).Where(c =>
                    c.GetParameters()
                    .Select(p => p.ParameterType) //ActualTypeOf(p.ParameterType))
                    .All(t => Services.ContainsKey(t)))
                    .ToList();

            if (ctors.Count == 0)
                return null;

            ConstructorInfo ctor = ctors.First();
            if (strategy != ConstructorPreference.FirstPreferred)
            {
                ctors = ctors.OrderBy(c => c.GetParameters().Length).ToList();
                ctor = strategy == ConstructorPreference.LeastResolveablePreferred ? ctors.First() : ctors.Last();
            }

            Func<TService> factory = () =>
            {
                object[] parameters = ctor.GetParameters().Select(p =>
                    Services[p.ParameterType]//[ActualTypeOf(p.ParameterType)]
                    .GetInstance()).ToArray();
                return (TService)ctor.Invoke(parameters);
            };

            return factory;
        }
        //*/
    }
}
