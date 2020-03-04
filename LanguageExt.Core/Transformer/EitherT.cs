﻿using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class EitherTExtensions
    {
        public static Either<L, Arr<B>> Traverse<L, A, B>(this Arr<Either<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return Either<L, Arr<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }
            return new Arr<B>(res);
        }
        
        public static Either<L, Either<L, B>> Traverse<L, A, B>(this Either<L, Either<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsLeft)
            {
                return Either<L, Either<L, B>>.Left((L)ma);
            }
            else
            {
                var mb = (Either<L, A>)ma;
                if (mb.IsLeft)
                {
                    return Either<L, Either<L, B>>.Left((L)mb);
                }
                else
                {
                    return Either<L, Either<L, B>>.Right(f((A)mb));
                }
            }
        }
        
        public static Either<L, EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Either<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsLeft)
            {
                return Either<L, EitherUnsafe<L, B>>.Left((L)ma);
            }
            else
            {
                var mb = (Either<L, A>)ma;
                if (mb.IsLeft)
                {
                    return Either<L, EitherUnsafe<L, B>>.Left((L)mb);
                }
                else
                {
                    return Either<L, EitherUnsafe<L, B>>.Right(f((A)mb));
                }
            }
        }
        
        public static Either<L, HashSet<B>> Traverse<L, A, B>(this HashSet<Either<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return Either<L, HashSet<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }
            return new HashSet<B>(res);
        }
                
        public static Either<L, Identity<B>> Traverse<L, A, B>(this Identity<Either<L, A>> ma, Func<A, B> f)
        {
            if (ma.Value.IsLeft)
            {
                return Either<L, Identity<B>>.Left((L)ma.Value);
            }
            else
            {
                return Either<L, Identity<B>>.Right(new Identity<B>(f((A)ma.Value)));
            }
        }
        
        public static Either<L, Lst<B>> Traverse<L, A, B>(this Lst<Either<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return Either<L, Lst<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }
            return new Lst<B>(res);
        }
        
        public static Either<L, Option<B>> Traverse<L, A, B>(this Option<Either<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsNone)
            {
                return Either<L, Option<B>>.Right(Option<B>.None);
            }
            else
            {
                var mb = (Either<L, A>)ma;
                if (mb.IsLeft)
                {
                    return Either<L, Option<B>>.Left((L)mb);
                }
                else
                {
                    return Either<L, Option<B>>.Right(f((A)mb));
                }
            }
        }        
        
        public static Either<L, OptionUnsafe<B>> Traverse<L, A, B>(this OptionUnsafe<Either<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsNone)
            {
                return Either<L, OptionUnsafe<B>>.Right(OptionUnsafe<B>.None);
            }
            else
            {
                var mb = (Either<L, A>)ma;
                if (mb.IsLeft)
                {
                    return Either<L, OptionUnsafe<B>>.Left((L)mb);
                }
                else
                {
                    return Either<L, OptionUnsafe<B>>.Right(f((A)mb));
                }
            }
        }        
        
        public static Either<L, Que<B>> Traverse<L, A, B>(this Que<Either<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return Either<L, Que<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }
            return new Que<B>(res);
        }
        
        public static Either<L, Seq<B>> Traverse<L, A, B>(this Seq<Either<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return Either<L, Seq<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }
            return new Seq<B>(res);
        }
        
        public static Either<L, IEnumerable<B>> Traverse<L, A, B>(this IEnumerable<Either<L, A>> ma, Func<A, B> f)
        {
            var res = new List<B>();
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return Either<L, IEnumerable<B>>.Left((L)x);
                }
                else
                {
                    res.Add(f((A)x));                    
                }
            }
            return Seq.FromArray<B>(res.ToArray());
        }
        
        public static Either<L, Set<B>> Traverse<L, A, B>(this Set<Either<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return Either<L, Set<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }
            return new Set<B>(res);
        }
        
        public static Either<L, Stck<B>> Traverse<L, A, B>(this Stck<Either<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return Either<L, Stck<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }
            return new Stck<B>(res);
        }
        
        public static Either<L, Try<B>> Traverse<L, A, B>(this Try<Either<L, A>> ma, Func<A, B> f)
        {
            var tres = ma.Try();
            
            if (tres.IsBottom)
            {
                return Either<L, Try<B>>.Bottom;
            }
            else if (tres.IsFaulted)
            {
                return default(MEither<L, Try<B>>).Fail(tres.Exception);
            }
            else if (tres.Value.IsLeft)
            {
                return Either<L, Try<B>>.Left((L)tres.Value);
            }
            else
            {
                return Either<L, Try<B>>.Right(Try(f((A)tres.Value)));
            }
        }
        
        public static Either<L, TryOption<B>> Traverse<L, A, B>(this TryOption<Either<L, A>> ma, Func<A, B> f)
        {
            var tres = ma.Try();
            
            if (tres.IsBottom)
            {
                return Either<L, TryOption<B>>.Bottom;
            }
            else if (tres.IsFaulted)
            {
                return default(MEither<L, TryOption<B>>).Fail(tres.Exception);
            }
            else if (tres.Value.IsNone)
            {
                return Either<L, TryOption<B>>.Right(TryOption<B>(None));
            }
            else if (tres.Value.Value.IsLeft)
            {
                return Either<L, TryOption<B>>.Left((L)tres.Value.Value);
            }
            else
            {
                return Either<L, TryOption<B>>.Right(TryOption(f((A)tres.Value.Value)));
            }
        }
        
        public static Either<Fail, Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Either<Fail, A>> ma, Func<A, B> f)
        {
            if (ma.IsFail && ma.FailValue.IsEmpty)
            {
                return Either<Fail, Validation<Fail, B>>.Bottom;
            }
            if (ma.IsFail)
            {
                return Either<Fail, Validation<Fail, B>>.Left(ma.FailValue.Head());
            }
            else
            {
                var mb = ma.SuccessValue;
                if (mb.IsLeft)
                {
                    return Either<Fail, Validation<Fail, B>>.Left((Fail)mb);
                }
                else
                {
                    return Either<Fail, Validation<Fail, B>>.Right(f((A)mb));
                }
            }
        }
        
        public static Either<Fail, Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Either<Fail, A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            if (ma.IsFail)
            {
                return Either<Fail, Validation<MonoidFail, Fail, B>>.Left(ma.FailValue);
            }
            else
            {
                var mb = ma.SuccessValue;
                if (mb.IsLeft)
                {
                    return Either<Fail, Validation<MonoidFail, Fail, B>>.Left((Fail)mb);
                }
                else
                {
                    return Either<Fail, Validation<MonoidFail, Fail, B>>.Right(f((A)mb));
                }
            }
        }
    }
}
