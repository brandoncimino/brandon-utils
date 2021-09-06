using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Hierarchic {
    /// <summary>
    /// An object which cannot exist without an object of type <see cref="TGuardian"/>.
    /// </summary>
    /// <typeparam name="TGuardian"></typeparam>
    /// <typeparam name="TDependant"></typeparam>
    [Obsolete("meh")]
    public abstract class Dependant<TGuardian, TDependant> : IDependant<TGuardian, TDependant>
        where TGuardian : IGuardian<TGuardian, TDependant>
        where TDependant : class, IDependant<TGuardian, TDependant> {
        [NotNull] public TGuardian Guardian { get; }

        protected Dependant([NotNull] TGuardian guardian) {
            Guardian = guardian;

            guardian.Adopt(this as TDependant ?? throw new InvalidOperationException());
        }
    }
}