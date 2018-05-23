using System;

namespace EasyWorks.IoC.Core
{
    /// <summary>
    /// A container object which may or may not contain a non-null value.
    /// If a value is present, IsPresent will return <value>True</value> and Value will return the value.
    /// <typeparam name="T"></typeparam>
    public struct Optional<T>
    {
        private static readonly Optional<T> _empty = new Optional<T>();
        /// <summary>
        /// Common instance for Empty.
        /// </summary>
        public static Optional<T> Empty => _empty;

        /// <summary>
        /// Returns an Optional instance with the specified present non-null value.
        /// </summary>
        /// <param name="value">value the value to be present, which must be non-null</param>
        /// <returns>An Optional with the value present</returns>
        public static Optional<T> Of(T value) => new Optional<T>(value);

        /// <summary>
        /// Returns an Optional instance  describing the specified value if non-null, 
        /// otherwise returns an empty
        /// </summary>
        /// <param name="value">value the possibly-null value to describe</param>
        /// <returns>An Optional  with a present value if the specified value is non-null, 
        /// otherwise an empty</returns>
        public static Optional<T> OfNullable(T value) => value == null ? Empty : Of(value);

        /// <summary>
        /// Indicate if the value is presented, <value>True</value> for yes, otherwise <value>False</value>.
        /// </summary>
        public bool IsPresent { get; }

        private T _value;
        /// <summary>
        /// Get the present value if it is not empty, or throw InvalidOperationException.
        /// </summary>
        public T Value => IsPresent ? _value : throw new InvalidOperationException();

        /// <summary>
        /// Private constructor only intended for the _empty instance.
        /// </summary>
        /// <param name="hasValue">Indicate if this Optional contains any value, shall only be <value>False</value></param>
        private Optional(bool hasValue = false)
        {
            this._value = default(T);
            IsPresent = false;
        }

        /// <summary>
        /// Constructs an instance with the value present.
        /// </summary>
        /// <param name="value">the non-null value to be present</param>
        public Optional(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            this._value = value;
            IsPresent = true;
        }

        /// <summary>
        /// Return the value if present, otherwise return <value>other</value>
        /// </summary>
        /// <param name="other">the value to be returned if there is no value present, may be null</param>
        /// <returns>return the value, if present, otherwise <value>other</value></returns>
        public T orElse(T other) => IsPresent ? Value : other;

        /// <summary>
        /// Explicit operator to get the value present.
        /// </summary>
        /// <param name="optional"></param>
        public static explicit operator T(Optional<T> optional)
        {
            return optional.Value;
        }

        /// <summary>
        /// Implicit operator to get the Optional instance with value present.
        /// </summary>
        /// <param name="value">the value to be present</param>
        public static implicit operator Optional<T>(T value)
        {
            return Optional<T>.OfNullable(value);
        }

        /// <summary>
        /// Indicates whether some other object is "equal to" this Optional. The 
        /// other object is considered equal if:
        ///     it is also an <code>Optional</code> and both instances have no value present or
        ///     the present values are "equal to" each other via {@code equals()}.
        /// </summary>
        /// <param name="obj">an object to be tested for equality</param>
        /// <returns><value>True</value> if the other object is "equal to" this object,
        /// otherwise<value>False</value></returns>
        public override bool Equals(object obj)
        {
            if (obj is Optional<T>)
                return this.Equals((Optional<T>)obj);
            else
                return false;
        }

        /// <summary>
        /// Indicates whether another Optional instance is "equal to" this Optional. The 
        /// other object is considered equal if both instances have no value present or
        ///     the present values are "equal to" each other via {@code equals()}.
        /// </summary>
        /// <param name="other">another Optional instance to be tested for equality</param>
        /// <returns><value>True</value> if the other object is "equal to" this object,
        /// otherwise<value>False</value></returns>
        public bool Equals(Optional<T> other)
        {
            if (IsPresent && other.IsPresent)
                return _value.Equals(other._value);
            else
                return IsPresent == other.IsPresent;
        }
    }
}
