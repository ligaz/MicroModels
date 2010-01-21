//Source: http://blogs.msdn.com/mattwar/archive/2007/08/01/linq-building-an-iqueryable-provider-part-iii.aspx

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MicroModels.Expressions
{
    internal static class Evaluator
    {
        public static Expression<Func<T>> EvaluateClosures<T>(Expression<Func<T>> expression)
        {
            Func<Expression, bool> canEvaluateClosure =
                exp => exp.NodeType == ExpressionType.Constant || IsClosureMemberExpression(exp);

            return (Expression<Func<T>>) PartialEval(expression, canEvaluateClosure);
        }

        private static bool IsClosureMemberExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = (MemberExpression) expression;
                var itemType = memberExpression.Expression.Type;

                return Attribute.IsDefined(itemType, typeof(CompilerGeneratedAttribute)) && itemType.IsNestedPrivate;
            }

            return false;
        }

        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
        }


        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression)
        {
            return PartialEval(expression, CanBeEvaluatedLocally);
        }


        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        private class Nominator : ExpressionVisitor
        {
            private List<Expression> candidates;

            private bool cannotBeEvaluated;
            private readonly Func<Expression, bool> fnCanBeEvaluated;


            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.fnCanBeEvaluated = fnCanBeEvaluated;
            }


            internal List<Expression> Nominate(Expression expression)
            {
                this.candidates = new List<Expression>();
                this.Visit(expression);
                return this.candidates;
            }


            protected override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = this.cannotBeEvaluated;

                    this.cannotBeEvaluated = false;

                    base.Visit(expression);

                    if (!this.cannotBeEvaluated)
                    {
                        if (this.fnCanBeEvaluated(expression))
                        {
                            this.candidates.Add(expression);
                        }

                        else
                        {
                            this.cannotBeEvaluated = true;
                        }
                    }

                    this.cannotBeEvaluated |= saveCannotBeEvaluated;
                }

                return expression;
            }
        }

        /// <summary>
        /// Evaluates & replaces sub-trees when first candidate is reached (top-down)
        /// </summary>
        private class SubtreeEvaluator : ExpressionVisitor
        {
            private readonly List<Expression> candidates;

            internal SubtreeEvaluator(List<Expression> candidates)
            {
                this.candidates = candidates;
            }

            internal Expression Eval(Expression exp)
            {
                return this.Visit(exp);
            }

            protected override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }

                if (this.candidates.Contains(exp))
                {
                    return Evaluate(exp);
                }

                return base.Visit(exp);
            }


            private static Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                {
                    return e;
                }

                LambdaExpression lambda = Expression.Lambda(e);
                Delegate fn = lambda.Compile();

                return Expression.Constant(fn.DynamicInvoke(null), e.Type);
            }
        }
    }
}