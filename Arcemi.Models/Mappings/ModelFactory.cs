#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Arcemi.Models
{
    public static class ModelFactory
    {
        private static readonly Dictionary<Type, Func<ModelDataAccessor, object>> UntypedFactories;
        private static readonly Dictionary<Type, object> Factories;

        static ModelFactory()
        {
            UntypedFactories = new Dictionary<Type, Func<ModelDataAccessor, object>>();
            Factories = new Dictionary<Type, object>();
            RegisterFactory(ItemModel.Create);
        }
        public static void RegisterFactory<T>(Func<ModelDataAccessor, T> factory)
        {
            UntypedFactories.Add(typeof(T), m => factory(m));
            Factories.Add(typeof(T), factory);
        }
        public static Func<ModelDataAccessor, T> Get<T>()
        {
            if (!Factories.TryGetValue(typeof(T), out var factory)) {
                var constructor = typeof(T).GetConstructor(new[] { typeof(ModelDataAccessor) });
                if (constructor is null) {
                    throw new ArgumentException("No factory registered for type " + typeof(T).FullName);
                }
                var accessorPara = Expression.Parameter(typeof(ModelDataAccessor));
                var constructorCallExpr = Expression.New(constructor, accessorPara);
                var lambdaExpr = Expression.Lambda<Func<ModelDataAccessor, T>>(constructorCallExpr, accessorPara);
                factory = lambdaExpr.Compile();
                Factories.Add(typeof(T), factory);
            }
            return (Func<ModelDataAccessor, T>)factory;
        }
    }
}