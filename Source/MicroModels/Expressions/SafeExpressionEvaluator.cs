using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using MicroModels.Utilities;

namespace MicroModels.Expressions
{
    internal class SafeExpressionEvaluator
    {
        private readonly Expression _lambdaExpression;
        private readonly Dictionary<string, object> _parameters;

        public SafeExpressionEvaluator(Expression lambdaExpression, Dictionary<string, object> parameters)
        {
            _lambdaExpression = lambdaExpression;
            _parameters = parameters;
        }

        public object Evaluate()
        {
            var result = EvaluateExpresion(_lambdaExpression);
            return result == Statement.Terminate
                       ? null
                       : result;
        }

        private object EvaluateExpresion(Expression expression)
        {
            if (expression == null) return null;

            var firstParameterType = expression.GetType();
            while (firstParameterType.IsGenericType) firstParameterType = firstParameterType.BaseType;

            var method = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.Name == "EvaluateExpression")
                .FirstOrDefault(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == firstParameterType);

            if (method == null)
            {
                throw new NotSupportedException(string.Format("Expressions of type {0} are not supported.", expression.GetType().Name));
            }

            return method.Invoke(this, new[] { expression });
        }

        private object EvaluateExpression(ConstantExpression constantExpression)
        {
            return constantExpression.Value;
        }

        private object EvaluateExpression(BinaryExpression binaryExpression)
        {
            var left = EvaluateExpresion(binaryExpression.Left);
            if (left == Statement.Terminate)
            {
                return Statement.Terminate;
            }

            var right = EvaluateExpresion(binaryExpression.Right);
            if (right == Statement.Terminate)
            {
                return Statement.Terminate;
            }

            return binaryExpression.Method.Invoke(null, new[] { left, right });
        }

        private object EvaluateExpression(LambdaExpression lambdaExpression)
        {
            var method = typeof(LambdaExtensions).GetMethod("Eval", BindingFlags.Static | BindingFlags.Public);

            var funcType = lambdaExpression.GetType().GetGenericArguments()[0];
            var funcArguments = funcType.GetGenericArguments();
            var funcInvokerType = typeof(FuncInvoker<,>).MakeGenericType(funcArguments);
            var funcInvoker = Activator.CreateInstance(funcInvokerType, lambdaExpression);

            return Delegate.CreateDelegate(funcType, funcInvoker, "Invoke");
        }

        private object EvaluateExpression(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Object == null && !methodCallExpression.Method.IsStatic)
            {
                return Statement.Terminate;
            }

            var arguments = new List<object>();
            foreach (var argument in methodCallExpression.Arguments)
            {
                var argumentValue = EvaluateExpresion(argument);
                arguments.Add(argumentValue);
            }

            if (!methodCallExpression.Method.IsStatic)
            {
                var left = EvaluateExpresion(methodCallExpression.Object);
                if (left == null || left == Statement.Terminate)
                {
                    return Statement.Terminate;
                }
                return methodCallExpression.Method.Invoke(left, arguments.ToArray());
            }
            if (arguments.Count > 0)
            {
                if ((arguments[0] == null || arguments[0] == Statement.Terminate) && methodCallExpression.Method.GetCustomAttributes(typeof(ExtensionAttribute), false).Length > 0)
                {
                    return Statement.Terminate;
                }
            }
            return methodCallExpression.Method.Invoke(null, arguments.ToArray());
        }

        private object EvaluateExpression(MemberExpression memberExpression)
        {
            var leftHandSide = EvaluateExpresion(memberExpression.Expression);
            if (leftHandSide == null || leftHandSide == Statement.Terminate)
            {
                return Statement.Terminate;
            }

            if (memberExpression.Member is FieldInfo)
            {
                return ((FieldInfo)memberExpression.Member).GetValue(leftHandSide);
            }
            if (memberExpression.Member is PropertyInfo)
            {
                return ((PropertyInfo)memberExpression.Member).GetValue(leftHandSide, null);
            }
            throw new NotSupportedException(string.Format("Expressions using members of type {0} are not supported.", memberExpression.Member.GetType().Name));
        }

        private object EvaluateExpression(ParameterExpression parameterExpression)
        {
            var parameterValue = _parameters[parameterExpression.Name];
            if (parameterValue == null)
            {
                return Statement.Terminate;
            }
            return parameterValue;
        }
    }
}