using System;
using System.Linq.Expressions;

namespace OLinqProvider
{
    public static class ExpressionExtensions
    {
        public static string GetCollectionName(this Expression expression)
        {
            return GetCollectionName(expression as MethodCallExpression);
        }

        public static string GetCollectionName(MethodCallExpression methodCallExpression)
        {
            var constantExpression = methodCallExpression.Arguments[0] as ConstantExpression;

            if (constantExpression != null)
            {
                return GetCollectionName(constantExpression);
            }

            return GetCollectionName(methodCallExpression.Arguments[0] as MethodCallExpression);
        }

        public static string GetCollectionName(ConstantExpression constantExpression)
        {
            return (constantExpression.Value as IOQuery).CollectionName;
        }
        public static int YearCompare(this DateTime date)
        {
            return date.Year;
        }
        public static int MonthCompare(this DateTime date)
        {
            return date.Month;
        }
        public static int DayCompare(this DateTime date)
        {
            return date.Day;
        }
        public static int MinuteCompare(this DateTime date)
        {
            return date.Minute;
        }
        public static int Length(this string input)
        {
            return input.Length;
        }
    }
}
