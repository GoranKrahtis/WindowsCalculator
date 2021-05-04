using MathExpressions.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressions
{
    public abstract class ExpressionBase : IExpression
    {
        public abstract int ArgumentCount { get; }
        private MathEvaluate _evaluateDelegate;

        public virtual MathEvaluate Evaluate
        {
            get { return _evaluateDelegate; }
            set { _evaluateDelegate = value; }
        }

        protected void Validate(double[] numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException("numbers");
            if (numbers.Length != ArgumentCount)
                throw new ArgumentException(Resources.InvalidLengthOfArray);
        }
    }
}
