using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone {
    /// <summary>
    /// Extension methods that operate on <see cref="Action"/>s and <see cref="Func{TResult}"/>s.
    /// </summary>
    [PublicAPI]
    public static class LambdaExtensions {
        #region Action with Tuple args

        public static void Invoke<T1, T2>([NotNull] this                     Action<T1, T2>                     action, (T1 arg1, T2 arg2)                                              args) => action.Invoke(args.arg1, args.arg2);
        public static void Invoke<T1, T2, T3>([NotNull] this                 Action<T1, T2, T3>                 action, (T1 arg1, T2 arg2, T3 arg3)                                     args) => action.Invoke(args.arg1, args.arg2, args.arg3);
        public static void Invoke<T1, T2, T3, T4>([NotNull] this             Action<T1, T2, T3, T4>             action, (T1 arg1, T2 arg2, T3 arg3, T4 arg4)                            args) => action.Invoke(args.arg1, args.arg2, args.arg3, args.arg4);
        public static void Invoke<T1, T2, T3, T4, T5>([NotNull] this         Action<T1, T2, T3, T4, T5>         action, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)                   args) => action.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5);
        public static void Invoke<T1, T2, T3, T4, T5, T6>([NotNull] this     Action<T1, T2, T3, T4, T5, T6>     action, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)          args) => action.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5, args.arg6);
        public static void Invoke<T1, T2, T3, T4, T5, T6, T7>([NotNull] this Action<T1, T2, T3, T4, T5, T6, T7> action, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) args) => action.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5, args.arg6, args.arg7);

        #endregion

        #region Func with Tuple args

        [Pure] public static TResult Invoke<T1, T2, TResult>([NotNull] this                     Func<T1, T2, TResult>                     func, (T1 arg1, T2 arg2)                                              args) => func.Invoke(args.arg1, args.arg2);
        [Pure] public static TResult Invoke<T1, T2, T3, TResult>([NotNull] this                 Func<T1, T2, T3, TResult>                 func, (T1 arg1, T2 arg2, T3 arg3)                                     args) => func.Invoke(args.arg1, args.arg2, args.arg3);
        [Pure] public static TResult Invoke<T1, T2, T3, T4, TResult>([NotNull] this             Func<T1, T2, T3, T4, TResult>             func, (T1 arg1, T2 arg2, T3 arg3, T4 arg4)                            args) => func.Invoke(args.arg1, args.arg2, args.arg3, args.arg4);
        [Pure] public static TResult Invoke<T1, T2, T3, T4, T5, TResult>([NotNull] this         Func<T1, T2, T3, T4, T5, TResult>         func, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)                   args) => func.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5);
        [Pure] public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>([NotNull] this     Func<T1, T2, T3, T4, T5, T6, TResult>     func, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)          args) => func.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5, args.arg6);
        [Pure] public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>([NotNull] this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) args) => func.Invoke(args.arg1, args.arg2, args.arg3, args.arg4, args.arg5, args.arg6, args.arg7);

        #endregion
    }
}