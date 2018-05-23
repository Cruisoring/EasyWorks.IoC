using System;

namespace EasyWorks.IoC
{
    public interface IServiceProvider<TService> where TService : class
    {
        /// <summary>
        /// Get the service instance upon request
        /// </summary>
        /// <returns>Instance of TService</returns>
        TService GetService();

        /// <summary>
        /// Type of the service.
        /// </summary>
        Type ServiceType { get; }
    }
}