using EasyIoC.Implementations;
using System;

namespace EasyWorks.IoC
{
    public class Dependency
    {
        public const string ConsumeIndicator = "->";
        public readonly Type SupplierType;
        public readonly Type ConsumerType;

        public Dependency(Type consumerType, Type supplierType)
        {
            ConsumerType = consumerType ?? throw new ArgumentNullException(nameof(consumerType));
            SupplierType = supplierType ?? throw new ArgumentNullException(nameof(supplierType));
        }

        public TCustomer Induce<TCustomer>(Container container)
        {
            return (TCustomer)container.Resolve(ConsumerType);
        }

        public TCustomer Induce<TCustomer>()
        {
            return Induce<TCustomer>(Container.Default);
        }

        public TSupplier Suggest<TSupplier>(Container container)
        {
            return (TSupplier)container.Resolve(SupplierType);
        }

        public TSupplier Suggest<TSupplier>()
        {
            return Suggest<TSupplier>(Container.Default);
        }

        public override string ToString()
        {
            return $"{ConsumerType.FullName}{ConsumeIndicator}{SupplierType.FullName}";
        }
    }
}
