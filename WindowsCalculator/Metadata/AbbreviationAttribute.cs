using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsCalculator.Metadata
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class AbbreviationAttribute : Attribute
    {
        public AbbreviationAttribute(string text)
        {
            _text = text;
        }

        private string _text;

        public string Text
        {
            get { return _text; }
        }
        public override string ToString()
        {
            return _text;
        }
    }
}
