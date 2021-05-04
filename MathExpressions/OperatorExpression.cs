using MathExpressions.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace MathExpressions
{
    public class OperatorExpression : ExpressionBase
    {
        private static readonly char[] operatorSymbols = new char[] { '+', '-', '*', '/', '%', '^' };


        private MathOperators _mathOperator;
        public MathOperators MathOperator
        {
            get { return _mathOperator; }
        }

        public double Add(double[] numbers)
        {
            Validate(numbers);
            double result = 0;
            foreach (double n in numbers)
                result += n;

            return result;
        }

        public double Subtract(double[] numbers)
        {
            Validate(numbers);
            double? result = null;
            foreach (double n in numbers)
                if (result.HasValue)
                    result -= n;
                else
                    result = n;

            return result ?? 0;
        }

        public double Multiple(double[] numbers)
        {
            Validate(numbers);
            double? result = null;
            foreach (double n in numbers)
                if (result.HasValue)
                    result *= n;
                else
                    result = n;

            return result ?? 0;
        }

        public double Divide(double[] numbers)
        {
            Validate(numbers);
            double? result = null;
            foreach (double n in numbers)
                if (result.HasValue)
                    result /= n;
                else
                    result = n;

            return result ?? 0;
        }
        
        public double Modulo(double[] numbers)
        {
            Validate(numbers);
            double? result = null;
            foreach (double n in numbers)
                if (result.HasValue)
                    result %= n;
                else
                    result = n;

            return result ?? 0;
        }

        public double Power(double[] numbers)
        {
            Validate(numbers);
            return Math.Pow(numbers[0], numbers[1]);
        }

        public static bool IsSymbol(char c)
        {
            return Array.Exists(operatorSymbols, delegate (char s) { return s == c; });
        }

        public override int ArgumentCount
        {
            get { return 2; }
        }

        public OperatorExpression(string @operator)
        {
            if (string.IsNullOrEmpty(@operator))
                throw new ArgumentNullException("operator");

            switch (@operator)
            {
                case "+":
                    base.Evaluate = new MathEvaluate(Add);
                    _mathOperator = MathOperators.Add;
                    break;
                case "-":
                    base.Evaluate = new MathEvaluate(Subtract);
                    _mathOperator = MathOperators.Subtract;
                    break;
                case "*":
                    base.Evaluate = new MathEvaluate(Multiple);
                    _mathOperator = MathOperators.Multiple;
                    break;
                case "/":
                    base.Evaluate = new MathEvaluate(Divide);
                    _mathOperator = MathOperators.Divide;
                    break;
                case "%":
                    base.Evaluate = new MathEvaluate(Modulo);
                    _mathOperator = MathOperators.Modulo;
                    break;
                case "^":
                    base.Evaluate = new MathEvaluate(Power);
                    _mathOperator = MathOperators.Power;
                    break;

                default:
                    throw new ArgumentException(Resources.InvalidOperator + @operator, "operator");
            }
        }

        public override string ToString()
        {
            return _mathOperator.ToString();
        }
    }
}
