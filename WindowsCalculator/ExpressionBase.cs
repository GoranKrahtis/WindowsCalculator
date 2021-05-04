using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsCalculator.Properties;

namespace WindowsCalculator
{
    public abstract class ExpressionBase : IExpression
    {
        /// Gets the number of arguments this expression uses.
        /// The argument count.
        public abstract int ArgumentCount { get; }
        private MathEvaluate _evaluateDelegate;

        public virtual MathEvaluate Evaluate
        {
            /// Gets or sets the evaluate delegate.
            /// The evaluate delegate.
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
