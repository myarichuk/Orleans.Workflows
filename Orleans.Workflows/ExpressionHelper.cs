using System;
using System.Linq.Expressions;

namespace Orleans.Workflows
{
    public static class ExpressionHelper
    {
        public static Expression<Action<TActivity, ActivityContext>> CreateWorkflowSetter<TActivity, TValue>(
           Expression<Func<TActivity, TValue>> propertyGetExpression, Expression<Func<ActivityContext, object>> valueExtractor)
                where TActivity : WorkflowActivity
        {
            if (propertyGetExpression.Body is MemberExpression memberExpression && valueExtractor is LambdaExpression le && le.Body is MethodCallExpression mce)
            {
                var entityParameterExpression = (ParameterExpression)memberExpression.Expression;
                var contextParameterExpression = (ParameterExpression)mce.Object;

                var lambda = Expression.Lambda<Action<TActivity, ActivityContext>>(
                    Expression.Assign(propertyGetExpression.Body, Expression.Convert(valueExtractor.Body, typeof(TValue))),
                    entityParameterExpression, contextParameterExpression);

                return lambda;
            }
            else
            {
                throw new NotSupportedException("Only member expressions are supported in selectors and indexer reference calls are supported on value extractors.");
            }
        }

        public static Expression<Action<TActivity>> CreateWorkflowSetter<TActivity, TValue>(
            Expression<Func<TActivity, TValue>> propertyGetExpression, TValue value)
            where TActivity : WorkflowActivity
        {
            if (propertyGetExpression.Body is MemberExpression memberExpression)
            {
                var entityParameterExpression = (ParameterExpression)memberExpression.Expression;

                return Expression.Lambda<Action<TActivity>>(
                    Expression.Assign(propertyGetExpression.Body, Expression.Constant(value)),
                    entityParameterExpression);
            }
            else
            {
                throw new NotSupportedException("Only member expressions are supported in selectors.");
            }
        }
    }
}
