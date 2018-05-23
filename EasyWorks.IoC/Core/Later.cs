using System;

namespace EasyWorks.IoC.Core
{
    /// <summary>
    /// Simplified Later initialization.
    /// </summary>
    /// <typeparam name="T">Specifies the type of element being initialized later when used.</typeparam>
    /// <remarks>Not thread safe.</remarks>
    internal class Later<T>
    {
        private Optional<T> _instance = Optional<T>.Empty;
        private readonly Func<T> _factory;

        /// <summary>
        /// Construct an instance with its factory for later initialization.
        /// </summary>
        /// <param name="func">Factory method to get the represented instance.</param>
        internal Later(Func<T> func)
        {
            this._factory = func ?? throw new ArgumentNullException(nameof(func));
        }

        /// <summary>
        /// Construct an instance with a solid instance, thus factory would never be called.
        /// </summary>
        /// <param name="instance">Instance cannot be null.</param>
        internal Later(T instance)
        {
            _instance = Optional<T>.Of(instance);
        }

        /// <summary>
        /// Gets the lazily initialized value of the current Later{T}.
        /// </summary>
        public T Value
        {
            get
            {
                if (!_instance.IsPresent)
                {
                    _instance = _factory.Invoke();
                }

                return _instance.Value;
            }
        }
    }
}
