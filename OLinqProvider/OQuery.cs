using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OLinqProvider
{
    public class OQuery<T>:IOrderedQueryable<T>, IOQuery
    {
        private readonly QueryProvider _provider;
        private readonly Expression _expression;
        public string CollectionName { get; private set; }

         public OQuery(QueryProvider provider, string collection)
        {
            CollectionName = collection;
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            _provider = provider;
            _expression = Expression.Constant(this);
        }
        public OQuery(QueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }
            _provider = provider;
            _expression = expression;
        }
         public Expression Expression
         {
             get { return _expression; }
         }

         public Type ElementType
         {
             get { return typeof(T); }
         }

         public IQueryProvider Provider
         {
             get { return _provider; }
         }

         public IEnumerator<T> GetEnumerator()
         {
             var execute = _provider.Execute<IEnumerable<T>>(Expression);
             return execute.GetEnumerator();
         }

         IEnumerator IEnumerable.GetEnumerator()
         {
             return GetEnumerator();
         }
    }
}
