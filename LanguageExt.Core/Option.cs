﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.ComponentModel;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace LanguageExt
{
    /// <summary>
    /// Option T can be in two states:
    ///     1. Some(x) -- which means there is a value stored inside
    ///     2. None    -- which means there's no value stored inside
    /// To extract the value you must use the 'match' function.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(OptionalTypeConverter))]
    public struct Option<T> : 
        IOptional, 
        IComparable<Option<T>>, 
        IComparable<T>, 
        IEquatable<Option<T>>, 
        IEquatable<T>,
        IAppendable<Option<T>>,
        ISubtractable<Option<T>>,
        IProductable<Option<T>>,
        IDivisible<Option<T>>
    {
        readonly T value;

        private Option(T value, bool isSome)
        {
            this.IsSome = isSome;
            this.value = value;
        }

        private Option(T value) 
            : this (value,value != null)
            {}

        /// <summary>
        /// Option Some(x) constructor
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Some(value) as Option T</returns>
        public static Option<T> Some(T value) => 
            new Option<T>(value);

        /// <summary>
        /// Option None of T
        /// </summary>
        public static readonly Option<T> None = 
            new Option<T>();

        /// <summary>
        /// true if the Option is in a Some(x) state
        /// </summary>
        public bool IsSome { get; }

        /// <summary>
        /// true if the Option is in a None state
        /// </summary>
        public bool IsNone => 
            !IsSome;

        internal T Value =>
            IsSome
                ? value
                : raise<T>(new OptionIsNoneException());

        public static implicit operator Option<T>(T value) =>
            value == null
                ? None
                : Some(value);

        public static implicit operator Option<T>(OptionNone none) => 
            None;

        internal static U CheckNullReturn<U>(U value, string location) =>
            value == null
                ? raise<U>(new ResultIsNullException($"'{location}' result is null.  Not allowed."))
                : value;

        internal static U CheckNullNoneReturn<U>(U value) =>
            CheckNullReturn(value, "None");

        internal static U CheckNullSomeReturn<U>(U value) =>
            CheckNullReturn(value, "Some");

        public R Match<R>(Func<T, R> Some, Func<R> None) =>
            IsSome
                ? CheckNullReturn(Some(Value), "Some")
                : CheckNullReturn(None(), "None");

        public R MatchUnsafe<R>(Func<T, R> Some, Func<R> None) =>
            IsSome
                ? Some(Value)
                : None();

        /// <summary>
        /// Match the two states of the Option
        /// The Some can return a Task R and the None an R.  The result is wrapped in a Task R
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A promise to return an R</returns>
        public async Task<R> MatchAsync<R>(Func<T, Task<R>> Some, Func<R> None) =>
            IsSome
                ? CheckNullReturn(await Some(Value), "Some")
                : CheckNullReturn(None(),"None");

        /// <summary>
        /// Match the two states of the Option
        /// The Some and None can return a Task R and the None an R
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A promise to return an R</returns>
        public async Task<R> MatchAsync<R>(Func<T, Task<R>> Some, Func<Task<R>> None) =>
            await (IsSome
                ? Some(Value)
                : None());

        /// <summary>
        /// Match the two states of the Option
        /// The Some can return an IObservable R and the None an R.  The result is wrapped in an IObservable R
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A promise to return an stream of Rs</returns>
        public IObservable<R> MatchObservable<R>(Func<T, IObservable<R>> Some, Func<R> None) =>
            IsSome
                ? Some(Value).Select(CheckNullSomeReturn)
                : Observable.Return(CheckNullReturn(None(),"None"));

        /// <summary>
        /// Match the two states of the Option
        /// The Some and None can return an IObservable R
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A promise to return an stream of Rs</returns>
        public IObservable<R> MatchObservable<R>(Func<T, IObservable<R>> Some, Func<IObservable<R>> None) =>
            IsSome
                ? Some(Value).Select(CheckNullSomeReturn)
                : None().Select(CheckNullNoneReturn);

        /// <summary>
        /// Untyped check
        /// </summary>
        /// <remarks>May also return null</remarks>
        /// <returns>R</returns>
        public R MatchUntyped<R>(Func<object, R> Some, Func<R> None) =>
            IsSome
                ? Some(Value)
                : None();

        /// <summary>
        /// Match the two states of the Option T
        /// </summary>
        /// <param name="Some">Some match</param>
        /// <param name="None">None match</param>
        /// <returns></returns>
        public Unit Match(Action<T> Some, Action None)
        {
            if (IsSome)
            {
                Some(Value);
            }
            else
            {
                None();
            }
            return Unit.Default;
        }

        /// <summary>
        /// Invokes the someHandler if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public Unit IfSome(Action<T> someHandler)
        {
            if (IsSome)
            {
                someHandler(value);
            }
            return unit;
        }

        /// <summary>
        /// Invokes the someHandler if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public Unit IfSome(Func<T,Unit> someHandler)
        {
            if (IsSome)
            {
                someHandler(value);
            }
            return unit;
        }

        public T IfNone(Func<T> None) =>
            Match(identity, None);

        public T IfNone(T noneValue) =>
            Match(identity, () => noneValue);

        public T IfNoneUnsafe(Func<T> None) =>
            MatchUnsafe(identity, None);

        public T IfNoneUnsafe(T noneValue) =>
            MatchUnsafe(identity, () => noneValue);

        [Obsolete("'Failure' has been deprecated.  Please use 'IfNone' instead")]
        public T Failure(Func<T> None) => 
            Match(identity, None);

        [Obsolete("'Failure' has been deprecated.  Please use 'IfNone' instead")]
        public T Failure(T noneValue) => 
            Match(identity, () => noneValue);

        public SomeUnitContext<T> Some(Action<T> someHandler) =>
            new SomeUnitContext<T>(this, someHandler);

        public SomeContext<T, R> Some<R>(Func<T, R> someHandler) =>
            new SomeContext<T, R>(this,someHandler);

        public override string ToString() =>
            IsSome
                ? Value == null 
                    ? "Some(null)"
                    : $"Some({Value})"
                : "None";

        public override int GetHashCode() =>
            IsSome && Value != null
                ? Value.GetHashCode()
                : 0;

        public override bool Equals(object obj) =>
            obj is Option<T>
                ? map(this, (Option<T>)obj, (lhs, rhs) =>
                      lhs.IsNone && rhs.IsNone
                          ? true
                          : lhs.IsNone || rhs.IsNone
                              ? false
                              : lhs.Value.Equals(rhs.Value))
                : IsSome
                    ? Value.Equals(obj)
                    : false;

        public Lst<T> ToList() =>
            toList(AsEnumerable());

        public T[] ToArray() =>
            toArray(AsEnumerable());

        public IEnumerable<T> AsEnumerable()
        {
            if (IsSome)
            {
                yield return Value;
            }
        }

        public Either<L, T> ToEither<L>(L defaultLeftValue) =>
            IsSome
                ? Right<L, T>(Value)
                : Left<L, T>(defaultLeftValue);

        public Either<L, T> ToEither<L>(Func<L> Left) =>
            IsSome
                ? Right<L, T>(Value)
                : Left<L, T>(Left());

        public EitherUnsafe<L, T> ToEitherUnsafe<L>(L defaultLeftValue) =>
            IsSome
                ? RightUnsafe<L, T>(Value)
                : LeftUnsafe<L, T>(defaultLeftValue);

        public EitherUnsafe<L, T> ToEitherUnsafe<L>(Func<L> Left) =>
            IsSome
                ? RightUnsafe<L, T>(Value)
                : LeftUnsafe<L, T>(Left());

        public TryOption<T> ToTryOption<L>(L defaultLeftValue)
        {
            var self = this;
            return () => self;
        }

        public static bool operator ==(Option<T> lhs, Option<T> rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Option<T> lhs, Option<T> rhs) =>
            !lhs.Equals(rhs);

        public static Option<T> operator |(Option<T> lhs, Option<T> rhs) =>
            lhs.IsSome
                ? lhs
                : rhs;

        public static bool operator true(Option<T> value) =>
            value.IsSome;

        public static bool operator false(Option<T> value) =>
            value.IsNone;

        public Type GetUnderlyingType() =>
            typeof(T);

        public int CompareTo(Option<T> other) =>
            IsNone && other.IsNone
                ? 0
                : IsSome && other.IsSome
                    ? Comparer<T>.Default.Compare(Value, other.Value)
                    : IsSome
                        ? -1
                        : 1;

        public int CompareTo(T other) =>
            IsNone
                ? -1
                : Comparer<T>.Default.Compare(Value, other);

        public bool Equals(T other) =>
            IsNone
                ? false
                : EqualityComparer<T>.Default.Equals(Value, other);

        public bool Equals(Option<T> other) =>
            IsNone && other.IsNone
                ? true
                : IsSome && other.IsSome
                    ? EqualityComparer<T>.Default.Equals(Value, other.Value)
                    : false;

        /// <summary>
        /// Append the Some(x) of one option to the Some(y) of another.
        /// For numeric values the behaviour is to sum the Somes (lhs + rhs)
        /// For string values the behaviour is to concatenate the strings
        /// For Lst/Stck/Que values the behaviour is to concatenate the lists
        /// For Map or Set values the behaviour is to merge the sets
        /// Otherwise if the T type derives from IAppendable then the behaviour
        /// is to call lhs.Append(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        public static Option<T> operator + (Option<T> lhs, Option<T> rhs) =>
            lhs.Append(rhs);

        /// <summary>
        /// Append the Some(x) of one option to the Some(y) of another.
        /// For numeric values the behaviour is to sum the Somes (lhs + rhs)
        /// For string values the behaviour is to concatenate the strings
        /// For Lst/Stck/Que values the behaviour is to concatenate the lists
        /// For Map or Set values the behaviour is to merge the sets
        /// Otherwise if the T type derives from IAppendable then the behaviour
        /// is to call lhs.Append(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        public Option<T> Append(Option<T> rhs)
        {
            if (IsNone && rhs.IsNone) return this;  // None  + None  = None
            if (rhs.IsNone) return this;            // Value + None  = Value
            if (this.IsNone) return rhs;            // None  + Value = Value
            return Optional(TypeDesc.Append(Value, rhs.Value, TypeDesc<T>.Default));
        }

        /// <summary>
        /// Subtract the Some(x) of one option from the Some(y) of another.  If either of the
        /// options are None then the result is None
        /// For numeric values the behaviour is to find the difference between the Somes (lhs - rhs)
        /// For Lst values the behaviour is to remove items in the rhs from the lhs
        /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
        /// Otherwise if the T type derives from ISubtractable then the behaviour
        /// is to call lhs.Subtract(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        public static Option<T> operator -(Option<T> lhs, Option<T> rhs) =>
            lhs.Subtract(rhs);

        /// <summary>
        /// Subtract the Some(x) of one option from the Some(y) of another.
        /// For numeric values the behaviour is to find the difference between the Somes (lhs - rhs)
        /// For Lst values the behaviour is to remove items in the rhs from the lhs
        /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
        /// Otherwise if the T type derives from ISubtractable then the behaviour
        /// is to call lhs.Subtract(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        public Option<T> Subtract(Option<T> rhs)
        {
            var self = IsNone
                ? TypeDesc<T>.Default.HasZero
                    ? Some(TypeDesc<T>.Default.Zero<T>())
                    : this
                : this;
            if (self.IsNone) return this;  // zero - rhs = undefined (when HasZero == false)
            if (rhs.IsNone) return this;   // lhs - zero = lhs
            return Optional(TypeDesc.Subtract(self.Value, rhs.Value, TypeDesc<T>.Default));
        }

        /// <summary>
        /// Find the product of the Somes.
        /// For numeric values the behaviour is to multiply the Somes (lhs * rhs)
        /// For Lst values the behaviour is to multiply all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the T type derives from IProductable then the behaviour
        /// is to call lhs.Product(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        public static Option<T> operator *(Option<T> lhs, Option<T> rhs) =>
            lhs.Product(rhs);

        /// <summary>
        /// Find the product of the Somes.
        /// For numeric values the behaviour is to multiply the Somes (lhs * rhs)
        /// For Lst values the behaviour is to multiply all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the T type derives from IProductable then the behaviour
        /// is to call lhs.Product(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        public Option<T> Product(Option<T> rhs)
        {
            if (IsNone) return this;     // zero * rhs = zero
            if (rhs.IsNone) return rhs;  // lhs * zero = zero
            return Optional(TypeDesc.Product(Value, rhs.Value, TypeDesc<T>.Default));
        }

        /// <summary>
        /// Divide the Somes.
        /// For numeric values the behaviour is to divide the Somes (lhs / rhs)
        /// For Lst values the behaviour is to divide all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the T type derives from IDivisible then the behaviour
        /// is to call lhs.Divide(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        public static Option<T> operator /(Option<T> lhs, Option<T> rhs) =>
            lhs.Divide(rhs);

        /// <summary>
        /// Divide the Somes.
        /// For numeric values the behaviour is to divide the Somes (lhs / rhs)
        /// For Lst values the behaviour is to divide all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the T type derives from IDivisible then the behaviour
        /// is to call lhs.Divide(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        public Option<T> Divide(Option<T> rhs)
        {
            if (IsNone) return this;     // zero / rhs  = zero
            if (rhs.IsNone) return rhs;  // lhs  / zero = undefined: zero
            return TypeDesc.Divide(Value, rhs.Value, TypeDesc<T>.Default);
        }
    }

    public struct SomeContext<T, R>
    {
        readonly Option<T> option;
        readonly Func<T, R> someHandler;

        internal SomeContext(Option<T> option, Func<T, R> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        public R None(Func<R> noneHandler) =>
            match(option, someHandler, noneHandler);

        public R None(R noneValue) =>
            match(option, someHandler, () => noneValue);
    }

    public struct SomeUnitContext<T>
    {
        readonly Option<T> option;
        readonly Action<T> someHandler;

        internal SomeUnitContext(Option<T> option, Action<T> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        public Unit None(Action noneHandler) =>
            match(option, someHandler, noneHandler);
    }

    public struct OptionNone
    {
        public static OptionNone Default = new OptionNone();
    }

    internal static class OptionCast
    {
        public static Option<T> Cast<T>(T value) =>
            value == null
                ? Option<T>.None
                : Option<T>.Some(value);


        public static Option<T> Cast<T>(Nullable<T> value) where T : struct =>
            value == null
                ? Option<T>.None
                : Option<T>.Some(value.Value);
    }
}

public static class __OptionExt
{
    public static T? ToNullable<T>(this Option<T> self) where T : struct =>
        self.IsNone
            ? (T?)null
            : new Nullable<T>(self.Value);

    /// <summary>
    /// Apply an Optional value to an Optional function
    /// </summary>
    /// <param name="opt">Optional function</param>
    /// <param name="arg">Optional argument</param>
    /// <returns>Returns the result of applying the optional argument to the optional function</returns>
    public static Option<R> Apply<T, R>(this Option<Func<T, R>> opt, Option<T> arg) => 
        opt.IsSome && arg.IsSome
            ? Optional(opt.Value(arg.Value))
            : None;

    /// <summary>
    /// Apply an Optional value to an Optional function of arity 2
    /// </summary>
    /// <param name="opt">Optional function</param>
    /// <param name="arg">Optional argument</param>
    /// <returns>Returns the result of applying the optional argument to the optional function:
    /// an optonal function of arity 1</returns>
    public static Option<Func<T2, R>> Apply<T1, T2, R>(this Option<Func<T1, T2, R>> opt, Option<T1> arg) =>
        opt.IsSome && arg.IsSome
            ? Optional(par(opt.Value, arg.Value))
            : None;

    /// <summary>
    /// Apply Optional values to an Optional function of arity 2
    /// </summary>
    /// <param name="opt">Optional function</param>
    /// <param name="arg1">Optional argument</param>
    /// <param name="arg2">Optional argument</param>
    /// <returns>Returns the result of applying the optional arguments to the optional function</returns>
    public static Option<R> Apply<T1, T2, R>(this Option<Func<T1, T2, R>> opt, Option<T1> arg1, Option<T2> arg2) =>
        opt.IsSome && arg1.IsSome && arg2.IsSome
            ? Optional(opt.Value(arg1.Value, arg2.Value))
            : None;

    public static Unit Iter<T>(this Option<T> self, Action<T> action) =>
        self.IfSome(action);

    public static int Count<T>(this Option<T> self) =>
        self.IsSome
            ? 1
            : 0;

    public static bool ForAll<T>(this Option<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
            : true;

    public static bool ForAll<T>(this Option<T> self, Func<T, bool> Some, Func<bool> None) =>
        self.IsSome
            ? Some(self.Value)
            : None();

    public static bool Exists<T>(this Option<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
            : false;

    public static bool Exists<T>(this Option<T> self, Func<T, bool> Some, Func<bool> None) =>
        self.IsSome
            ? Some(self.Value)
            : None();

    public static S Fold<S, T>(this Option<T> self, S state, Func<S, T, S> folder) =>
        self.IsSome
            ? folder(state, self.Value)
            : state;

    public static S Fold<S, T>(this Option<T> self, S state, Func<S, T, S> Some, Func<S, S> None) =>
        self.IsSome
            ? Some(state, self.Value)
            : None(state);

    public static Option<R> Map<T, R>(this Option<T> self, Func<T, R> mapper) =>
        self.IsSome
            ? OptionCast.Cast(mapper(self.Value))
            : None;

    public static Option<R> Map<T, R>(this Option<T> self, Func<T, R> Some, Func<R> None) =>
        self.IsSome
            ? OptionCast.Cast(Some(self.Value))
            : None();

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    public static Option<Func<T2, R>> Map<T1, T2, R>(this Option<T1> opt, Func<T1, T2, R> func) =>
        opt.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    public static Option<Func<T2, Func<T3, R>>> Map<T1, T2, T3, R>(this Option<T1> opt, Func<T1, T2, T3, R> func) =>
        opt.Map(curry(func));

    public static Option<T> Filter<T>(this Option<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
                ? self
                : None
            : self;

    public static Option<T> Filter<T>(this Option<T> self, Func<T, bool> Some, Func<bool> None) =>
        self.IsSome
            ? Some(self.Value)
                ? self
                : Option<T>.None
            : None()
                ? self
                : Option<T>.None;

    public static Option<R> Bind<T, R>(this Option<T> self, Func<T, Option<R>> binder) =>
        self.IsSome
            ? binder(self.Value)
            : None;

    public static Option<R> Bind<T, R>(this Option<T> self, Func<T, Option<R>> Some, Func<Option<R>> None) =>
        self.IsSome
            ? Some(self.Value)
            : None();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Option<U> Select<T, U>(this Option<T> self, Func<T, U> map) =>
        self.Map(map);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Option<T> Where<T>(this Option<T> self, Func<T, bool> pred) =>
        self.Filter(pred)
            ? self
            : None;

    public static int Sum(this Option<int> self) =>
        self.IsSome
            ? self.Value
            : 0;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Option<V> SelectMany<T, U, V>(this Option<T> self,
        Func<T, Option<U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return None;
        var resU = bind(self.Value);
        if (resU.IsNone) return None;

        var res = Optional(project(self.Value, resU.Value));
        if (resU.Value is ILinqDisposable) (resU.Value as ILinqDisposable).Dispose();
        if (self.Value is ILinqDisposable) (self.Value as ILinqDisposable).Dispose();
        return res;
    }

    /// <summary>
    /// Match the two states of the Option&lt;Task&lt;T&gt;&gt;
    /// 
    ///     If Some then the result of the Task is passed to Some and returned as a Task R.
    ///     If None then the result of None() is returned as a Task R
    ///     
    /// </summary>
    /// <typeparam name="R">Return type</typeparam>
    /// <param name="Some">Some handler</param>
    /// <param name="None">None handler</param>
    /// <returns>A promise to return an R</returns>
    public static async Task<R> MatchAsync<T, R>(this Option<Task<T>> self, Func<T, R> Some, Func<R> None) =>
        self.IsSome
            ? Some(await self.Value)
            : None();

    /// <summary>
    /// Match the two states of the Option&lt;IObservable&lt;T&gt;&gt;
    /// 
    ///     If Some then the observable stream is mapped with Some (until the subscription ends)
    ///     If None the a single value observable is returned with the None result in
    /// 
    /// </summary>
    /// <typeparam name="R">Return type</typeparam>
    /// <param name="Some">Some handler</param>
    /// <param name="None">None handler</param>
    /// <returns>A stream of Rs</returns>
    public static IObservable<R> MatchObservable<T, R>(this Option<IObservable<T>> self, Func<T, R> Some, Func<R> None) =>
        self.IsSome
            ? self.Value.Select(Some).Select(Option<R>.CheckNullSomeReturn)
            : Observable.Return(Option<R>.CheckNullReturn(None(), "None"));

    /// <summary>
    /// Match the two states of the IObservable&lt;Option&lt;T&gt;&gt;
    /// 
    ///     Matches a stream of options
    /// 
    /// </summary>
    /// <typeparam name="R">Return type</typeparam>
    /// <param name="Some">Some handler</param>
    /// <param name="None">None handler</param>
    /// <returns>A stream of Rs</returns>
    public static IObservable<R> MatchObservable<T, R>(this IObservable<Option<T>> self, Func<T, R> Some, Func<R> None) =>
        self.Select(opt => match(opt, Some, None));
}
