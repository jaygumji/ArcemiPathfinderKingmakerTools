using System;
using System.Reflection;

namespace Arcemi.Models
{
    public class SaveFileTypeMetadata
    {
        public SaveFileTypeMetadata(Type type)
        {
            Type = type;
            Attribute = type.GetCustomAttribute<SaveFileTypeAttribute>();
            Constructor = Type.GetConstructor(new[] { typeof(ModelDataAccessor) });
            //var modelParamExpr = Expression.Parameter(typeof(ModelDataAccessor));
            //var activatorExpr = Expression.New(constructor, modelParamExpr);
            //var lambda = Expression.Lambda<Func<ModelDataAccessor, Model>>(
        }

        public Type Type { get; }
        public SaveFileTypeAttribute Attribute { get; }
        public ConstructorInfo Constructor { get; }

        public Model CreateInstance(ModelDataAccessor m)
        {
            return (Model)Constructor.Invoke(new object[] { m });
        }
    }
}