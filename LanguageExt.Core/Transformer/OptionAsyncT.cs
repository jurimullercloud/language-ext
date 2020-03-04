using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.DataTypes.Serialisation;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class OptionAsyncTExtensions
    {
        //
        // Collections
        //

        public static OptionAsync<Arr<B>> Traverse<A, B>(this Arr<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Arr<B>>(Go(ma, f));
            async Task<(bool, Arr<B>)> Go(Arr<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data));
                return rb.Exists(d => !d.IsSome)
                     ? (false, default)
                     : (true, new Arr<B>(rb.Map(d => d.Value)));
            }
        }

        public static OptionAsync<HashSet<B>> Traverse<A, B>(this HashSet<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<HashSet<B>>(Go(ma, f));
            async Task<(bool, HashSet<B>)> Go(HashSet<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data));
                return rb.Exists(d => !d.IsSome)
                    ? (false, default)
                    : (true, new HashSet<B>(rb.Map(d => d.Value)));
            }
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static OptionAsync<IEnumerable<B>> Traverse<A, B>(this IEnumerable<OptionAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
        
        public static OptionAsync<IEnumerable<B>> TraverseSerial<A, B>(this IEnumerable<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<IEnumerable<B>>(Go(ma, f));
            async Task<(bool, IEnumerable<B>)> Go(IEnumerable<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = new List<B>();
                foreach (var a in ma)
                {
                    var mb = await a;
                    if (mb.IsNone) return (false, default);
                    rb.Add(f(mb.Value));
                }
                return (true, rb);
            };
        }
        
        public static OptionAsync<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<IEnumerable<B>>(Go(ma, f));
            async Task<(bool, IEnumerable<B>)> Go(IEnumerable<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data));
                return rb.Exists(d => !d.IsSome)
                    ? (false, default)
                    : (true, rb.Map(d => d.Value));
            }
        }

        public static OptionAsync<Lst<B>> Traverse<A, B>(this Lst<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Lst<B>>(Go(ma, f));
            async Task<(bool, Lst<B>)> Go(Lst<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data));
                return rb.Exists(d => !d.IsSome)
                    ? (false, default)
                    : (true, new Lst<B>(rb.Map(d => d.Value)));
            }
        }

        public static OptionAsync<Que<B>> Traverse<A, B>(this Que<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Que<B>>(Go(ma, f));
            async Task<(bool, Que<B>)> Go(Que<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data));
                return rb.Exists(d => !d.IsSome)
                    ? (false, default)
                    : (true, new Que<B>(rb.Map(d => d.Value)));
            }
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static OptionAsync<Seq<B>> Traverse<A, B>(this Seq<OptionAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
        
        public static OptionAsync<Seq<B>> TraverseSerial<A, B>(this Seq<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Seq<B>>(Go(ma, f));
            async Task<(bool, Seq<B>)> Go(Seq<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = new B[ma.Count];
                var ix = 0;
                foreach (var a in ma)
                {
                    var mb = await a;
                    if (mb.IsNone) return (false, default);
                    rb[ix] = f(mb.Value);
                    ix++;
                }
                return (true, Seq.FromArray<B>(rb));
            };
        }

        public static OptionAsync<Seq<B>> TraverseParallel<A, B>(this Seq<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Seq<B>>(Go(ma, f));
            async Task<(bool, Seq<B>)> Go(Seq<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data));
                return rb.Exists(d => !d.IsSome)
                    ? (false, default)
                    : (true, Seq.FromArray<B>(rb.Map(d => d.Value).ToArray()));
            }
        }

        public static OptionAsync<Set<B>> Traverse<A, B>(this Set<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Set<B>>(Go(ma, f));
            async Task<(bool, Set<B>)> Go(Set<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data));
                return rb.Exists(d => !d.IsSome)
                    ? (false, default)
                    : (true, new Set<B>(rb.Map(d => d.Value)));
            }
        }

        public static OptionAsync<Stck<B>> Traverse<A, B>(this Stck<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Stck<B>>(Go(ma, f));
            async Task<(bool, Stck<B>)> Go(Stck<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data));
                return rb.Exists(d => !d.IsSome)
                    ? (false, default)
                    : (true, new Stck<B>(rb.Map(d => d.Value)));
            }
        }
        
        //
        // Async types
        //

        public static OptionAsync<EitherAsync<L, B>> Traverse<L, A, B>(this EitherAsync<L, OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<EitherAsync<L, B>>(Go(ma, f));
            async Task<(bool, EitherAsync<L, B>)> Go(EitherAsync<L, OptionAsync<A>> ma, Func<A, B> f)
            {
                var da = await ma.Data;
                if (da.State == EitherStatus.IsBottom) return (false, default);
                if (da.State == EitherStatus.IsLeft) return (false, default);
                var (isSome, value) = await da.Right.Data;
                if (!isSome) return (false, default);
                return (true, EitherAsync<L, B>.Right(f(value)));
            }
        }

        public static OptionAsync<OptionAsync<B>> Traverse<A, B>(this OptionAsync<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<OptionAsync<B>>(Go(ma, f));
            async Task<(bool, OptionAsync<B>)> Go(OptionAsync<OptionAsync<A>> ma, Func<A, B> f)
            {
                var (isSomeA, valueA) = await ma.Data;
                if (!isSomeA) return (false, default);
                var (isSomeB, valueB) = await valueA.Data;
                if (!isSomeB) return (false, default);
                return (true, OptionAsync<B>.Some(f(valueB)));
            }
        }
        
        public static OptionAsync<TryAsync<B>> Traverse<A, B>(this TryAsync<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<TryAsync<B>>(Go(ma, f));
            async Task<(bool, TryAsync<B>)> Go(TryAsync<OptionAsync<A>> ma, Func<A, B> f)
            {
                var resultA = await ma.Try();
                if (resultA.IsBottom) return (false, default);
                if (resultA.IsFaulted) return (false, default);
                var (isSome, value) = await resultA.Value.Data;
                if (!isSome) return (false, default);
                return (true, TryAsync<B>(f(value)));
            }
        }
        
        public static OptionAsync<TryOptionAsync<B>> Traverse<A, B>(this TryOptionAsync<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<TryOptionAsync<B>>(Go(ma, f));
            async Task<(bool, TryOptionAsync<B>)> Go(TryOptionAsync<OptionAsync<A>> ma, Func<A, B> f)
            {
                var resultA = await ma.Try();
                if (resultA.IsBottom) return (false, default);
                if (resultA.IsNone) return (false, default);
                if (resultA.IsFaulted) return (false, default);
                var (isSome, value) = await resultA.Value.Value.Data;
                if (!isSome) return (false, default);
                return (true, TryOptionAsync<B>(f(value)));
            }
        }

        public static OptionAsync<Task<B>> Traverse<A, B>(this Task<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Task<B>>(Go(ma, f));
            async Task<(bool, Task<B>)> Go(Task<OptionAsync<A>> ma, Func<A, B> f)
            {
                var result = await ma;
                var (isSome, value) = await result.Data;
                if (!isSome) return (false, default);
                return (true, f(value).AsTask());
            }
        }

        //
        // Sync types
        // 
        
        public static OptionAsync<Either<L, B>> Traverse<L, A, B>(this Either<L, OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Either<L, B>>(Go(ma, f));
            async Task<(bool, Either<L, B>)> Go(Either<L, OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return (false, default);
                if(ma.IsLeft) return (false, default);
                var (isSome, value) = await ma.RightValue.Data;
                if(!isSome) return (false, default);
                return (true, f(value));
            }
        }

        public static OptionAsync<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<EitherUnsafe<L, B>>(Go(ma, f));
            async Task<(bool, EitherUnsafe<L, B>)> Go(EitherUnsafe<L, OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return (false, default);
                if(ma.IsLeft) return (false, default);
                var (isSome, value) = await ma.RightValue.Data;
                if(!isSome) return (false, default);
                return (true, f(value));
            }
        }

        public static OptionAsync<Identity<B>> Traverse<A, B>(this Identity<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Identity<B>>(Go(ma, f));
            async Task<(bool, Identity<B>)> Go(Identity<OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return (false, default);
                var (isSome, value) = await ma.Value.Data;
                if(!isSome) return (false, default);
                return (true, new Identity<B>(f(value)));
            }
        }

        public static OptionAsync<Option<B>> Traverse<A, B>(this Option<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Option<B>>(Go(ma, f));
            async Task<(bool, Option<B>)> Go(Option<OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return (false, default);
                var (isSome, value) = await ma.Value.Data;
                if(!isSome) return (false, default);
                return (true, Option<B>.Some(f(value)));
            }
        }
        
        public static OptionAsync<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<OptionUnsafe<B>>(Go(ma, f));
            async Task<(bool, OptionUnsafe<B>)> Go(OptionUnsafe<OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return (false, default);
                var (isSome, value) = await ma.Value.Data;
                if(!isSome) return (false, default);
                return (true, OptionUnsafe<B>.Some(f(value)));
            }
        }
        
        public static OptionAsync<Try<B>> Traverse<A, B>(this Try<OptionAsync<A>> ma, Func<A, B> f)
        {
            try
            {
                return new OptionAsync<Try<B>>(Go(ma, f));
                async Task<(bool, Try<B>)> Go(Try<OptionAsync<A>> ma, Func<A, B> f)
                {
                    var ra = ma.Try();
                    if(ra.IsBottom) return (false, default);
                    if(ra.IsFaulted) return (false, default);
                    var (isSome, value) = await ra.Value.Data;
                    if(!isSome) return (false, default);
                    return (true, Try<B>(f(value)));
                }
            }
            catch (Exception e)
            {
                return Try<B>(e);
            }
        }
        
        public static OptionAsync<TryOption<B>> Traverse<A, B>(this TryOption<OptionAsync<A>> ma, Func<A, B> f)
        {
            try
            {
                return new OptionAsync<TryOption<B>>(Go(ma, f));
                async Task<(bool, TryOption<B>)> Go(TryOption<OptionAsync<A>> ma, Func<A, B> f)
                {
                    var ra = ma.Try();
                    if (ra.IsBottom) return (false, default);
                    if (ra.IsFaultedOrNone) return (false, default);
                    var (isSome, value) = await ra.Value.Value.Data;
                    if (!isSome) return (false, default);
                    return (true, TryOption<B>(f(value)));
                }
            }
            catch (Exception e)
            {
                return TryOption<B>(e);
            }
        }
        
        public static OptionAsync<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Validation<Fail, B>>(Go(ma, f));
            async Task<(bool, Validation<Fail, B>)> Go(Validation<Fail, OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return (false, default);
                var (isSome, value) = await ma.SuccessValue.Data;
                if(!isSome) return (false, default);
                return (true, f(value));
            }
        }
        
        public static OptionAsync<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, OptionAsync<A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            return new OptionAsync<Validation<MonoidFail, Fail, B>>(Go(ma, f));
            async Task<(bool, Validation<MonoidFail, Fail, B>)> Go(Validation<MonoidFail, Fail, OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return (false, default);
                var (isSome, value) = await ma.SuccessValue.Data;
                if(!isSome) return (false, default);
                return (true, f(value));
            }
        }
    }
}
