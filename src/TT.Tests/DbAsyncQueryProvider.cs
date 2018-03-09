using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace TT.Tests
{
    /// <summary>
    /// Wrapper class for <see cref="IQueryable{T}"/> to supply async opperations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncQueryable<T> : IQueryable<T>, IDbAsyncEnumerable<T>
    {
        public Expression Expression => queryable.Expression;
        public Type ElementType => queryable.ElementType;
        public IQueryProvider Provider => dbAsyncQueryProvider;

        private IDbAsyncQueryProvider dbAsyncQueryProvider;
        private IDbAsyncEnumerator<T> dbAsyncEnumerator;
        private IQueryable<T> queryable;

        public AsyncQueryable(IQueryable<T> queryable)
        {
            this.queryable = queryable;
            this.dbAsyncQueryProvider = new DbAsyncQueryProvider<T>(queryable.Provider);
            this.dbAsyncEnumerator = new DbAsyncEnumerator<T>(queryable.GetEnumerator());
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return dbAsyncEnumerator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queryable.GetEnumerator();
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class DbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal DbAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                if (expression is MethodCallExpression m)
                {
                    // Fix for a possible Automapper bug.
                    var resultType = m.Method.ReturnType;
                    var tElement = resultType.GetGenericArguments()[0];
                    var queryType = typeof(DbAsyncEnumerable<>).MakeGenericType(tElement, tElement);
                    return (IQueryable)Activator.CreateInstance(queryType, expression);
                }

                return new DbAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new DbAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute(expression));
            }

            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute<TResult>(expression));
            }
        }

        private class DbAsyncEnumerable<TEntity> : EnumerableQuery<TEntity>, IDbAsyncEnumerable<TEntity>, IQueryable<TEntity>
        {
            public DbAsyncEnumerable(IEnumerable<TEntity> enumerable)
                : base(enumerable)
            { }

            public DbAsyncEnumerable(Expression expression)
                : base(expression)
            { }

            public IDbAsyncEnumerator<TEntity> GetAsyncEnumerator()
            {
                return new DbAsyncEnumerator<TEntity>(this.AsEnumerable().GetEnumerator());
            }

            IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
            {
                return GetAsyncEnumerator();
            }

            IQueryProvider IQueryable.Provider
            {
                get { return new DbAsyncQueryProvider<TEntity>(this); }
            }
        }

        private class DbAsyncEnumerator<TEntity> : IDbAsyncEnumerator<TEntity>
        {
            private readonly IEnumerator<TEntity> _inner;

            public DbAsyncEnumerator(IEnumerator<TEntity> inner)
            {
                _inner = inner;
            }

            public void Dispose()
            {
                _inner.Dispose();
            }

            public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(_inner.MoveNext());
            }

            public TEntity Current
            {
                get { return _inner.Current; }
            }

            object IDbAsyncEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
