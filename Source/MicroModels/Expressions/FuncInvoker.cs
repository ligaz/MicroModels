using System;
using System.Linq.Expressions;
using MicroModels.Utilities;

namespace MicroModels.Expressions
{
    internal class FuncInvoker<TArg0, TReturn>
    {
        private readonly Expression<Func<TArg0, TReturn>> _func;

        public FuncInvoker(Expression<Func<TArg0, TReturn>> func)
        {
            _func = func;
        }

        public TReturn Invoke(TArg0 arg)
        {
            return arg.Eval(_func);
        }
    }
}
