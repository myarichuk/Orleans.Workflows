using System;
using System.Linq.Expressions;

namespace Orleans.Workflows
{
    public static class ExpressionHelper
    {
        public static Expression<Action<TEntity>> CreateSetter<TEntity, TValue>(
            Expression<Func<TEntity, TValue>> propertyGetExpression,
            TValue value)
        {
            var entityParameterExpression = (ParameterExpression)((MemberExpression)propertyGetExpression.Body).Expression;

            return Expression.Lambda<Action<TEntity>>(
                Expression.Assign(propertyGetExpression.Body, Expression.Constant(value)),
                entityParameterExpression);
        }
    }
}
