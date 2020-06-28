using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Orleans.Workflows
{
    public class WorkflowActivityBuilder<TActivity> : IWorkflowDefinitionBuilder
        where TActivity : WorkflowActivity
    {
        private readonly WorkflowBuilder _workflowBuilder;
        private readonly TActivity _currentActivity;

        public WorkflowActivityBuilder(WorkflowBuilder workflowBuilder, TActivity activity)
        {
            _currentActivity = activity;
            _workflowBuilder = workflowBuilder;
        }

        public WorkflowActivityBuilder<TActivity> Input<TValue>(Expression<Func<TActivity, TValue>> propertySelector, Expression<Func<ActivityContext, object>> valueExtractor)
            where TValue : struct
        {
            //TODO: proper error handling, invoke can throw casting exception due to ActivityContext's nature
            var setter = ExpressionHelper.CreateWorkflowSetter(propertySelector, valueExtractor);

            _currentActivity.InputSettersWithContext.Add((activity, ctx) => setter.Compile().Invoke((TActivity)activity, ctx));
            return this;
        }

        public WorkflowActivityBuilder<TActivity> Input<TValue>(Expression<Func<TActivity, TValue>> propertySelector, TValue paramValue)
            where TValue : struct
        {
            var setter = ExpressionHelper.CreateWorkflowSetter(propertySelector, paramValue);

            setter.Compile().Invoke(_currentActivity);

            _currentActivity.InputSetters.Add(activity => setter.Compile().Invoke((TActivity)activity));
            return this;
        }

        public WorkflowActivityBuilder<TActivity> Output<TValue>(Expression<Func<TActivity, TValue>> outputPropertySelector, Expression<Func<ActivityContext, object>> outputSetter)
        {
            //var entityParameterExpression = (ParameterExpression)((MemberExpression)outputPropertySelector.Body).Expression;
            //var setter = Expression.Lambda<Action<ActivityContext>>(Expression.Assign(outputSetter.Body, outputPropertySelector), entityParameterExpression);
      
            return this;
        }

        public WorkflowActivityBuilder<TNextActivity> Then<TNextActivity>(Action<TNextActivity> configure = null)
            where TNextActivity : WorkflowActivity, new()
        {
            var nextActivity = new TNextActivity();
            configure?.Invoke(nextActivity);

            _workflowBuilder._flow.AddVertex(nextActivity);
            
            _ = _workflowBuilder._flow.AddEdge(new EdgeWithPredicate
            {
                Predicate = _ => true,
                Source = _currentActivity,
                Target = nextActivity
            });

            return new WorkflowActivityBuilder<TNextActivity>(_workflowBuilder, nextActivity);
        }

        public WorkflowDefinition Build() => _workflowBuilder.Build();
    }
}
