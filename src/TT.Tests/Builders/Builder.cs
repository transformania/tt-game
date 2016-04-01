using System;
using System.Linq.Expressions;
using System.Reflection;
using TT.Domain;
using TT.Domain.Entities;
using TT.Tests.Utilities;

namespace TT.Tests.Builders
{
    public class Builder<TEntity, TKey> where TEntity : Entity<TKey>
    {
        protected TEntity Instance { get; set; }

        public static TEntity Create()
        {
            var constructors = typeof(TEntity).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            if (constructors == null || constructors.Length == 0)
                throw new Exception(string.Format("Could't find a protected default constructor on type {0}", typeof(TEntity).FullName));

            return (TEntity)constructors[0].Invoke(null);
        }

        public Builder<TEntity, TKey> With<U>(Expression<Func<TEntity, U>> propertyFunction, U value)
        {
            var propertyInfo = ReflectionHelper.GetPropertyInfo(propertyFunction.Body);
            ReflectionHelper.SetProtectedProperty(propertyInfo, Instance, value);

            return this;
        }

        public TEntity Build()
        {
            return Instance;
        }

        public TEntity BuildAndSave()
        {
            if (DomainRegistry.Repository == null)
                return Instance;

            var context = ((DomainRepository)DomainRegistry.Repository).Context;

            context.Add(Instance);
            context.Commit();

            return Instance;
        }
    }
}