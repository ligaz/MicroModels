using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MicroModels.Dependencies.ExpressionAnalysis
{
    /// <summary>
    /// Given a LINQ expression, traverses the expression and produces a flat list of all of the 
    /// expressions. A list of expression types can be provided which the expression flattener will stop 
    /// at rather than traversing any further.
    /// </summary>
    public sealed class ExpressionFlattener
    {
        private readonly List<Expression> _expressions = new List<Expression>();
        private readonly ExpressionType[] _stopAt;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionFlattener"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="stopAt">Expression types to stop processing at.</param>
        public ExpressionFlattener(Expression expression, params ExpressionType[] stopAt)
        {
            _stopAt = stopAt;
            TraverseExpression(expression);
        }

        /// <summary>
        /// Gets the member expressions that have been found.
        /// </summary>
        /// <value>The member expressions.</value>
        public IEnumerable<Expression> Expressions
        {
            get { return _expressions; }
        }

        /// <summary>
        /// Traverses the expressions.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        private void TraverseExpressions(IEnumerable expressions)
        {
            if (expressions != null)
            {
                foreach (Expression expression in expressions)
                {
                    TraverseExpression(expression);
                }
            }
        }

        /// <summary>
        /// Traverses the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        private void TraverseExpression(Expression expression)
        {
            if (expression != null)
            {
                if (_stopAt.Contains(expression.NodeType))
                {
                    // We have found an expression we are interested in, so keep it and do not traverse it
                    // any further.
                    _expressions.Add(expression);
                }
                else
                {
                    // We do not want to keep this expression, so traverse it and look for things we do
                    // want to keep.
                    if (expression is BinaryExpression)
                    {
                        TraverseBinaryExpression((BinaryExpression) expression);
                    }
                    else if (expression is ConditionalExpression)
                    {
                        TraverseConditionalExpression((ConditionalExpression) expression);
                    }
                    else if (expression is ConstantExpression)
                    {
                        TraverseConstantExpression((ConstantExpression) expression);
                    }
                    else if (expression is InvocationExpression)
                    {
                        TraverseInvocationExpression((InvocationExpression) expression);
                    }
                    else if (expression is LambdaExpression)
                    {
                        TraverseLambdaExpression((LambdaExpression) expression);
                    }
                    else if (expression is ListInitExpression)
                    {
                        TraverseListInitExpression((ListInitExpression) expression);
                    }
                    else if (expression is MemberExpression)
                    {
                        TraverseMemberExpression((MemberExpression) expression);
                    }
                    else if (expression is MemberInitExpression)
                    {
                        TraverseMemberInitExpression((MemberInitExpression) expression);
                    }
                    else if (expression is MethodCallExpression)
                    {
                        TraverseMethodCallExpression((MethodCallExpression) expression);
                    }
                    else if (expression is NewArrayExpression)
                    {
                        TraverseNewArrayExpression((NewArrayExpression) expression);
                    }
                    else if (expression is NewExpression)
                    {
                        TraverseNewExpression((NewExpression) expression);
                    }
                    else if (expression is ParameterExpression)
                    {
                        TraverseParameterExpression((ParameterExpression) expression);
                    }
                    else if (expression is TypeBinaryExpression)
                    {
                        TraverseTypeBinaryExpression((TypeBinaryExpression) expression);
                    }
                    else if (expression is UnaryExpression)
                    {
                        TraverseUnaryExpression((UnaryExpression) expression);
                    }
                }
            }
        }

        /// <summary>
        /// Traverses the binary expression.
        /// </summary>
        /// <param name="binaryExpression">The binary expression.</param>
        private void TraverseBinaryExpression(BinaryExpression binaryExpression)
        {
            TraverseExpression(binaryExpression.Conversion);
            TraverseExpression(binaryExpression.Left);
            TraverseExpression(binaryExpression.Right);
        }

        /// <summary>
        /// Traverses the conditional expression.
        /// </summary>
        /// <param name="conditionalExpression">The conditional expression.</param>
        private void TraverseConditionalExpression(ConditionalExpression conditionalExpression)
        {
            TraverseExpression(conditionalExpression.IfFalse);
            TraverseExpression(conditionalExpression.IfTrue);
            TraverseExpression(conditionalExpression.Test);
        }

        /// <summary>
        /// Traverses the constant expression.
        /// </summary>
        /// <param name="constantExpression">The constant expression.</param>
        private void TraverseConstantExpression(ConstantExpression constantExpression)
        {
        }

        /// <summary>
        /// Traverses the invocation expression.
        /// </summary>
        /// <param name="invocationExpression">The invocation expression.</param>
        private void TraverseInvocationExpression(InvocationExpression invocationExpression)
        {
            TraverseExpressions(invocationExpression.Arguments);
            TraverseExpression(invocationExpression.Expression);
        }

        /// <summary>
        /// Traverses the lambda expression.
        /// </summary>
        /// <param name="lambdaExpression">The lambda expression.</param>
        private void TraverseLambdaExpression(LambdaExpression lambdaExpression)
        {
            TraverseExpressions(lambdaExpression.Parameters);
            TraverseExpression(lambdaExpression.Body);
        }

        /// <summary>
        /// Traverses the list init expression.
        /// </summary>
        /// <param name="listInitExpression">The list init expression.</param>
        private void TraverseListInitExpression(ListInitExpression listInitExpression)
        {
            TraverseExpressions(listInitExpression.Initializers.SelectMany(i => i.Arguments.Cast<Expression>()));
            TraverseExpression(listInitExpression.NewExpression);
        }

        /// <summary>
        /// Traverses the member expression.
        /// </summary>
        /// <param name="memberExpression">The member expression.</param>
        private void TraverseMemberExpression(MemberExpression memberExpression)
        {
            TraverseExpression(memberExpression.Expression);
        }

        /// <summary>
        /// Traverses the member init expression.
        /// </summary>
        /// <param name="memberInitExpression">The member init expression.</param>
        private void TraverseMemberInitExpression(MemberInitExpression memberInitExpression)
        {
            TraverseExpressions(memberInitExpression.Bindings.Where(b => b.BindingType == MemberBindingType.Assignment).Select(b => ((MemberAssignment) b).Expression));
            TraverseExpressions(memberInitExpression.Bindings.Where(b => b.BindingType == MemberBindingType.ListBinding).Select(b => ((MemberListBinding) b).Initializers.SelectMany(i => i.Arguments.Cast<Expression>())));
            TraverseExpressions(memberInitExpression.Bindings.Where(b => b.BindingType == MemberBindingType.MemberBinding).Select(b => ((MemberMemberBinding) b).Bindings.Cast<Expression>()));
            TraverseExpression(memberInitExpression.NewExpression);
        }

        /// <summary>
        /// Traverses the method call expression.
        /// </summary>
        /// <param name="methodCallExpression">The method call expression.</param>
        private void TraverseMethodCallExpression(MethodCallExpression methodCallExpression)
        {
            TraverseExpressions(methodCallExpression.Arguments);
            TraverseExpression(methodCallExpression.Object);
        }

        /// <summary>
        /// Traverses the new array expression.
        /// </summary>
        /// <param name="newArrayExpression">The new array expression.</param>
        private void TraverseNewArrayExpression(NewArrayExpression newArrayExpression)
        {
            TraverseExpressions(newArrayExpression.Expressions);
        }

        /// <summary>
        /// Traverses the new expression.
        /// </summary>
        /// <param name="newExpression">The new expression.</param>
        private void TraverseNewExpression(NewExpression newExpression)
        {
            TraverseExpressions(newExpression.Arguments);
        }

        /// <summary>
        /// Traverses the parameter expression.
        /// </summary>
        /// <param name="parameterExpression">The parameter expression.</param>
        private void TraverseParameterExpression(ParameterExpression parameterExpression)
        {
            
        }

        /// <summary>
        /// Traverses the type binary expression.
        /// </summary>
        /// <param name="typeBinaryExpression">The type binary expression.</param>
        private void TraverseTypeBinaryExpression(TypeBinaryExpression typeBinaryExpression)
        {
            TraverseExpression(typeBinaryExpression.Expression);
        }

        /// <summary>
        /// Traverses the unary expression.
        /// </summary>
        /// <param name="unaryExpression">The unary expression.</param>
        private void TraverseUnaryExpression(UnaryExpression unaryExpression)
        {
            TraverseExpression(unaryExpression.Operand);
        }
    }
}