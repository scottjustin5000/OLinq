using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OLinqProvider
{
    public abstract class QueryProvider:IQueryProvider
    {
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new OQuery<TElement>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = expression.Type;
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(OQuery<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public abstract TResult Execute<TResult>(Expression expression);

        public object Execute(Expression expression)
        {
            return Execute<object>(expression);
        }
    }
}
