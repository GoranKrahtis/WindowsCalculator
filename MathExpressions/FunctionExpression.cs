using System;
using System.Collections.Generic;
using System.Reflection;
using MathExpressions.Properties;
using System.Globalization;
using System.Linq;

namespace MathExpressions
{
    public class FunctionExpression : ExpressionBase
    {
        
        private static readonly string[] oneArgumentMathFunctions = new string[]
        {
            "abs", "acos", "asin", "atan", "ceiling", "cos", "cosh", "exp",
            "floor", "log", "log10", "sin", "sinh", "sqrt", "tan", "tanh"
        };
                
        private static readonly string[] twoArgumentMathFunctions = new string[]
        {
            "max", "min", "pow", "round"
        };
                
        public FunctionExpression(string function) : this(function, true)
        {
        }

        internal FunctionExpression(string function, bool validate)
        {
            function = function.ToLowerInvariant();

            if (validate && !IsFunction(function))
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Resources.InvalidFunctionName, _function),
                    "function");

            _function = function;
            base.Evaluate = new MathEvaluate(Execute);
        }

        private string _function;

        public string Function
        {
            get { return _function; }
        }

        public double Execute(double[] numbers)
        {
            Validate(numbers);

            Type[] desiredMethodSignatureArgs = { typeof(double) };

            if (IsTwoArgumentFunction(_function))
            {
                desiredMethodSignatureArgs = new[] { typeof(double), typeof(double) };
            }

            string function = char.ToUpperInvariant(_function[0]) + _function.Substring(1);
            MethodInfo method = typeof(Math).GetMethod(
                function,
                BindingFlags.Static | BindingFlags.Public,
                null,
                desiredMethodSignatureArgs,
                null);

            if (method == null)
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture,
                        Resources.InvalidFunctionName, _function));

            object[] parameters = new object[numbers.Length];
            Array.Copy(numbers, parameters, numbers.Length);
            return (double)method.Invoke(null, parameters);
        }
        
        public override int ArgumentCount
        {
            get
            {
                int rval = 1;

                if (IsTwoArgumentFunction(_function))
                {
                    rval = 2;
                }

                return rval;
            }
        }
                
        public static bool IsFunction(string function)
        {
            return IsOneArgumentFunction(function) || IsTwoArgumentFunction(function);
        }

        private static bool IsTwoArgumentFunction(string function)
        {
            bool isTwoArgumentFunction = Array.BinarySearch(
                twoArgumentMathFunctions, function,
                StringComparer.OrdinalIgnoreCase) >= 0;
            return isTwoArgumentFunction;
        }

        private static bool IsOneArgumentFunction(string function)
        {
            bool isOneArgumentFunction = Array.BinarySearch(
                oneArgumentMathFunctions, function,
                StringComparer.OrdinalIgnoreCase) >= 0;
            return isOneArgumentFunction;
        }

        public override string ToString()
        {
            return _function;
        }

        public static string[] GetFunctionNames()
        {
            return oneArgumentMathFunctions.Concat(twoArgumentMathFunctions).ToArray();
        }
    }
}