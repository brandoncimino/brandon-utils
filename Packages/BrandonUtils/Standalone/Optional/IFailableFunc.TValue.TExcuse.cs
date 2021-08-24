using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// Represents something <b>with a return value</b> that <i>might</i> have failed.
    /// <p/>
    /// To represent something <b>without</b> a return value that might have failed, use <see cref="IFailable{TExcuse}"/>.
    /// </summary>
    /// <remarks>
    /// <ul>
    /// <li>This interface combines <see cref="IOptional{T}"/> with <see cref="IFailable{TExcuse}"/>.</li>
    /// <li>An <see cref="IFailableFunc{TValue,TExcuse}"/> should generally be used when there is a meaningful return value (i.e. a <see cref="Func{TResult}"/>).
    ///     When the failable code does not have a return value, i.e. <see cref="Action"/>, <see cref="IFailable{TExcuse}"/> should be used instead.</li>
    /// </ul>
    /// TODO: Is there a fancy-schmancy way to enforce these kinds of contractual obligations? If we had default interfaces methods, that would be ideal, but those aren't supported by Unity yet...
    /// <p><b>Notes to Implementers</b></p>
    /// <see cref="IOptional{TValue}.Value"/> and <see cref="IFailable{TExcuse}.Excuse"/> should be <b>mutually exclusive</b>.
    /// <ul>
    /// <li><see cref="IOptional{TValue}.HasValue"/> and <see cref="IFailable{TExcuse}.Failed"/> should <b>never</b> be equal.</li>
    /// <li>If <see cref="IOptional{T}.HasValue"/> is true, <see cref="IFailable{TExcuse}.Excuse"/> should throw an exception.</li>
    /// <li>If <see cref="IFailable{TExcuse}.Failed"/> is true, <see cref="IOptional{TValue}.Value"/> should throw an exception.</li>
    /// </ul>
    /// </remarks>
    /// <typeparam name="TValue">the <see cref="IOptional{T}.Value"/>, if this succeeded</typeparam>
    /// <typeparam name="TExcuse">information about the failure, if this <see cref="IFailable{TExcuse}.Failed"/></typeparam>
    [PublicAPI]
    public interface IFailableFunc<out TValue, out TExcuse> : IOptional<TValue>, IFailable<TExcuse> where TExcuse : Exception { }
}