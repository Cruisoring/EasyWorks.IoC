using System;

namespace EasyWorks.IoC
{
    /// <summary>
    /// Holder of service information to provide resolved instance.
    /// </summary>
    public abstract class ServiceProvider
    {
        /// <summary>
        /// Type of the service.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Constructor to keep the Type info.
        /// </summary>
        /// <param name="serviceType"></param>
        protected ServiceProvider(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            ServiceType = serviceType;
        }

        /// <summary>
        /// Supply the resolved instance as an object.
        /// </summary>
        /// <returns></returns>
        public abstract object Instance { get; }
    }

    /// <summary>
    /// Singleton ServiceProvider implementation.
    /// </summary>
    /// <typeparam name="TService">Type of the service to be provided.</typeparam>
    public class SingletonProvider<TService> : ServiceProvider,
        IServiceProvider<TService> where TService : class
    {
        private TService _instance;

        public SingletonProvider(TService instance)
            : base(typeof(TService))
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        /// <summary>
        /// Retrieve the singleton instance.
        /// </summary>
        /// <returns>The singleton instance under request.</returns>
        public TService GetService() => _instance;

        public override object Instance => _instance;

        public override string ToString()
        {
            return $"Singleton {ServiceType.Name} -> {_instance.GetType().Name}";
        }

    }

    /// <summary>
    /// ServiceProvider with factory implementation.
    /// </summary>
    /// <typeparam name="TService">Type of the service to be provided.</typeparam>
    public class FactoryProvider<TService> : ServiceProvider,
        IServiceProvider<TService> where TService : class
    {
        private readonly Func<TService> _factory;
        private TService _instance;
        private readonly Predicate<TService> _useInstance;

        /// <summary>
        /// Construct a FactoryProvider with the Factory and optional serviceType and instance predicate.
        /// </summary>
        /// <param name="factory">The factory to create TService instance upon request</param>
        /// <param name="serviceType">Service Type identity of this Factory ServiceProvider</param>
        /// <param name="useInstance">Used to evaluate if <value>_instance</value> could be reused.
        /// With default null value, always create new instance.</param>
        public FactoryProvider(Func<TService> factory, Type serviceType = null, Predicate<TService> useInstance = null)
            : base(serviceType ?? typeof(TService))
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _instance = null;
            _useInstance = useInstance;
        }

        /// <summary>
        /// Get the service instance upon request
        /// </summary>
        /// <returns>The service instance either created by the factory, or the cached one that can still be reused</returns>
        public TService GetService()
        {
            //Create new instance when 1) By default when _useInstance is null; 
            //2) _instance is not set; 3) _instance cannot be reused
            if (_useInstance == null || _instance == null || !_useInstance.Invoke(_instance))
            {
                _instance = _factory.Invoke();
            }
            return _instance;
        }

        public override object Instance => GetService();

        public override string ToString()
        {
            return $"{ServiceType.Name} -> {(_instance == null ? "?" : _instance.GetType().Name)}";
        }

    }
}
