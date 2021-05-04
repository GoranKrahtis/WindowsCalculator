using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace MathExpressions
{
    public class NumberExpression : ExpressionBase
    {
        public NumberExpression(double value)
        {
            _value = value;
            base.Evaluate = delegate
            {
                return Value;
            };
        }

        public override int ArgumentCount
        {
            get { return 0; }
        }

        private double _value;

        public double Value
        {
            get { return _value; }
        }

        public static bool IsNumber(char c)
        {
            NumberFormatInfo f = CultureInfo.CurrentUICulture.NumberFormat;
            return char.IsDigit(c) || f.NumberDecimalSeparator.IndexOf(c) >= 0;
        }
                
        public static bool IsNegativeSign(char c)
        {
            NumberFormatInfo f = CultureInfo.CurrentUICulture.NumberFormat;
            return f.NegativeSign.IndexOf(c) >= 0;
        }
        public override string ToString()
        {
            return _value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
