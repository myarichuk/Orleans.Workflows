using System;
using System.Collections.Generic;
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

        public WorkflowActivityBuilder<TActivity> Input<TValue>(Expression<Func<ActivityContext, TValue>> propertySelector, TValue paramValue)
        {
            var setter = ExpressionHelper.CreateSetter(propertySelector, paramValue);
            if(_workflowBuilder._context.InputMapping.TryGetValue(_currentActivity, out var inputs))
            {
                inputs.Add(setter);
            }
            else
            {
                _workflowBuilder._context.InputMapping.Add(_currentActivity, new List<Expression<Action<ActivityContext>>> { setter });
            }
            return this;
        }

        public WorkflowActivityBuilder<TActivity> Output<TValue>(Expression<Func<ActivityContext, TValue>> outputPropertySelector, Expression<Func<ActivityContext, TValue>> outputSetter)
        {
            var entityParameterExpression = (ParameterExpression)((MemberExpression)outputPropertySelector.Body).Expression;
            var setter = Expression.Lambda<Action<ActivityContext>>(Expression.Assign(outputSetter.Body, outputPropertySelector), entityParameterExpression);
            
            if (_workflowBuilder._context.OutputMapping.TryGetValue(_currentActivity, out var outputs))
            {
                outputs.Add(setter);
            }
            else
            {
               _workflowBuilder._context.OutputMapping.Add(_currentActivity, new List<Expression<Action<ActivityContext>>> { setter });
            }
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
