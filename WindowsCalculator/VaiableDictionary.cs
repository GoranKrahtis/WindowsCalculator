
using WindowsCalculator.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace WindowsCalculator
{
    [Serializable]
    public class VariableDictionary : Dictionary<string, double>
    {
        private MathEvaluator _evaluator;

        internal VariableDictionary(MathEvaluator evaluator)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            _evaluator = evaluator;
            base.Add(MathEvaluator.AnswerVariable, 0);
        }

        protected VariableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public new void Add(string name, double value)
        {
            Validate(name);
            base.Add(name, value);
        }

        private void Validate(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            for (int i = 0; i < name.Length; i++)
                if (!char.IsLetter(name[i]))
                    throw new ArgumentException(Resources.VariableNameContainsLetters, "name");
        }
                
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

    }
}
