using System;

using JetBrains.Annotations;


namespace BrandonUtils.Standalone.Hierarchic {
    /// <summary>
    /// An object which cannot exist without an object of type <see cref="TGuardian"/>.
    /// </summary>
    /// <typeparam name="TGuardian"></typeparam>
    /// <typeparam name="TDependant"></typeparam>
    public abstract class Dependant<TGuardian, TDependant>
        where TGuardian : Guardian<TGuardian, TDependant>
        where TDependant : Dependant<TGuardian, TDependant> {

        [NotNull]
        [UsedImplicitly]
        public readonly TGuardian Guardian;

        [UsedImplicitly]
        protected Dependant([NotNull] TGuardian guardian) {
            Guardian = guardian;
            guardian.Adopt(this as TDependant ?? throw new InvalidOperationException());
        }
    }
}