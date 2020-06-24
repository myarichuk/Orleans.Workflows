﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using QuickGraph;

namespace Orleans.Workflows
{
    [Serializable]
    public class EdgeWithPredicate : IEdge<WorkflowActivity>
    {
        public WorkflowActivity Source { get; set; }
        public WorkflowActivity Target { get; set; }

        public Expression<Func<ActivityContext, bool>> Predicate { get; set; } = _ => true;
    }

    public class WorkflowDefinition
    {
        public class IOMappingContext
        {
            public Dictionary<WorkflowActivity, List<Expression<Action<ActivityContext>>>> InputMapping { get; } = new Dictionary<WorkflowActivity, List<Expression<Action<ActivityContext>>>>();
            public Dictionary<WorkflowActivity, List<Expression<Action<ActivityContext>>>> OutputMapping { get; } = new Dictionary<WorkflowActivity, List<Expression<Action<ActivityContext>>>>();
        }

        protected readonly AdjacencyGraph<WorkflowActivity, EdgeWithPredicate> _flow;

        public IOMappingContext Context { get; }

        public WorkflowActivity FirstActivity { get; }

        public IGraph<WorkflowActivity, EdgeWithPredicate> Flow => _flow;

        public WorkflowDefinition(AdjacencyGraph<WorkflowActivity, EdgeWithPredicate> flow, WorkflowActivity first, IOMappingContext context)
        {
            _flow = flow;
            FirstActivity = first;
            Context = context;
        }
    }
}