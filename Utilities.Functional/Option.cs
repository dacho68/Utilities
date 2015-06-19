﻿using System;
using System.Collections.Generic;

namespace Utilities.Functional
{
    public abstract class Option<T>
    {
        public static readonly Option<T> None = new None<T>();

        public abstract bool IsSome { get; }

        public abstract T GetUnsafe { get; }

        public abstract Option<R> Select<R>(Func<T, R> selector);

        public abstract Option<R> SelectMany<R>(Func<T, Option<R>> selector);

        public abstract Option<T> Where(Func<T, bool> predicate);

        public abstract void Iter(Action<T> action);

        public abstract R Match<R>(Func<T, R> some, Func<R> none);

        public abstract T Or(Func<T> selector);

        public abstract Option<T> Or(Func<Option<T>> selector);

        public abstract Option<R> OfType<R>();

        public abstract IEnumerable<T> ToEnumerable();

        public static Option<T> AsOption(T value)
        {
            return value != null ? new Some<T>(value) : None;
        }

        public static Option<T> Try(Func<T> selector)
        {
            try
            {
                return AsOption(selector.Invoke());
            }
            catch
            {
                return None;
            }
        }
    }
}
