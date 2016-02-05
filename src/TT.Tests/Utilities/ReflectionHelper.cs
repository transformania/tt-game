using System;
using System.Linq.Expressions;
using System.Reflection;

namespace TT.Tests.Utilities
{
    public class ReflectionHelper
    {
        public static void SetProtectedProperty<TObject, TValue>(PropertyInfo propertyInfo, TObject instance, TValue value)
        {
            if (propertyInfo.GetSetMethod() != null)
                throw new Exception("Property '" + propertyInfo.Name + "' is not protected on " + instance);

            if (!propertyInfo.CanWrite)
                throw new Exception("Property '" + propertyInfo.Name + "' does not have a mutator on " + instance);

            propertyInfo.SetValue(instance, value, null);
        }

        public static PropertyInfo GetPropertyInfo(Expression body)
        {
            if (body is UnaryExpression)
                body = (body as UnaryExpression).Operand;

            var me = (MemberExpression)body;
            return (PropertyInfo)me.Member;
        }
    }
}