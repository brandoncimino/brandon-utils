using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /**
     * <inheritdoc cref="IFailableFunc{TValue,TExcuse}"/>
     */
    [PublicAPI]
    public interface IFailableFunc<out TValue> : IFailableFunc<TValue, Exception> { }
}