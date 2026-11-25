#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Arcemi.Models
{
    public static class ModelFactory
    {
        private static readonly ConcurrentDictionary<Type, object> Factories = new();

        static ModelFactory()
        {
            RegisterFactory(ItemModel.Create);
        }

        public static void RegisterFactory<T>(Func<ModelDataAccessor, T> factory)
        {
            if (!Factories.TryAdd(typeof(T), factory)) {
                throw new ArgumentException("A factory is already registered for type " + typeof(T).FullName);
            }
        }

        public static Func<ModelDataAccessor, T> Get<T>() =>
            (Func<ModelDataAccessor, T>)Factories.GetOrAdd(typeof(T), _ => CreateFactory<T>());

        private static Func<ModelDataAccessor, T> CreateFactory<T>()
        {
            var constructor = typeof(T).GetConstructor(new[] { typeof(ModelDataAccessor) });
            if (constructor is null) {
                throw new ArgumentException("No factory registered for type " + typeof(T).FullName);
            }
            var accessor = Expression.Parameter(typeof(ModelDataAccessor));
            var call = Expression.New(constructor, accessor);
            return Expression.Lambda<Func<ModelDataAccessor, T>>(call, accessor).Compile();
        }
    }
}
