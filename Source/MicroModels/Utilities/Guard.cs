﻿using System;
using System.Diagnostics;

namespace MicroModels.Utilities
{
    [DebuggerNonUserCode]
    internal static class Guard
    {
        public static void ArgumentNotNull(object argument, string parameterName)
        {
            if (argument == null) 
                throw new ArgumentNullException(parameterName);
        }

        public static void ArgumentIsOfType(object argument, Type type, string parameterName)
        {
            if (argument == null || !type.IsAssignableFrom(argument.GetType()))
                throw new ArgumentException(parameterName);
        }

        public static void ArgumentNotNullOrEmpty(string argument, string parameterName)
        {
            ArgumentNotNull(argument, parameterName);
            if (argument.Trim().Length == 0)
            {
                throw new ArgumentException(string.Format("The parameter '{0}' cannot be empty.", parameterName), parameterName);
            }
        }
    }
}


