using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MicroModels.Expressions;
using System.Reflection;

namespace MicroModels.Utilities
{
    internal static class LambdaExtensions
    {
        public static TValue Eval<TSource, TValue>(this TSource source, Expression<Func<TSource, TValue>> value)
        {
            var parameters = new Dictionary<string, object> { { value.Parameters.First().Name, source } };
            var evaluator = new SafeExpressionEvaluator(value.Body, parameters);
            var result = evaluator.Evaluate();
            if (result == null) return default(TValue);
            return (TValue)result;
        }

        public static MemberExpression GetOutermostMember(this LambdaExpression lambda)
        {
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return memberExpression;
        }
    }
}
