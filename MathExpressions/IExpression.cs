using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressions
{
    public interface IExpression
    {
        int ArgumentCount { get; }
        /// <summary>Gets or sets the evaluate delegate.</summary>
        /// <value>The evaluate delegate.</value>
        MathEvaluate Evaluate { get; set; }
    }
}
