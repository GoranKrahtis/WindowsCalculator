using WindowsCalculator.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindowsCalculator
{
    public class MathEvaluator : IDisposable
    {
        public const string AnswerVariable = "answer";

        //instance scope to optimize reuse
        private Stack<string> _symbolStack;
        private Queue<IExpression> _expressionQueue;
        private Dictionary<string, IExpression> _expressionCache;
        private StringBuilder _buffer;
        private Stack<double> _calculationStack;
        private Stack<double> _parameters;
        private uint _nestedGroupDepth;
        private StringReader _expressionReader;
        private VariableDictionary _variables;
        private char _currentChar;

        public MathEvaluator()
        {
            _variables = new VariableDictionary(this);
            _expressionCache = new Dictionary<string, IExpression>(StringComparer.OrdinalIgnoreCase);
            _symbolStack = new Stack<string>();
            _expressionQueue = new Queue<IExpression>();
            _buffer = new StringBuilder();
            _calculationStack = new Stack<double>();
            _parameters = new Stack<double>(2);
            _nestedGroupDepth = 0;
        }


        /// <summary>
        /// Gets the variables collections.
        /// </summary>
        /// <value>The variables for <see cref="MathEvaluator"/>.</value>
        public VariableDictionary Variables
        {
            get { return _variables; }
        }


        /// <summary>Gets the answer from the last evaluation.</summary>
        /// <value>The answer variable value.</value>
        /// <seealso cref="Variables"/>
        public double Answer
        {
            get { return _variables[AnswerVariable]; }
        }

        /// <summary>Evaluates the specified expression.</summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>The result of the evaluated expression.</returns>
        /// <exception cref="ArgumentNullException">When expression is null or empty.</exception>
        /// <exception cref="ParseException">When there is an error parsing the expression.</exception>
        public double Evaluate(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                throw new ArgumentNullException("expression");
            try
            {
                _expressionReader = new StringReader(expression);
                _symbolStack.Clear();
                _nestedGroupDepth = 0;
                _expressionQueue.Clear();

                ParseExpressionToQueue();

                double result = CalculateFromQueue();

                _variables[AnswerVariable] = result;
                return result;
            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }            
        }

        private void ParseExpressionToQueue()
        {
            char lastChar = '\0';
            _currentChar = '\0';

            do
            {
                // last non white space char
                if (!char.IsWhiteSpace(_currentChar))
                    lastChar = _currentChar;

                _currentChar = (char)_expressionReader.Read();

                if (char.IsWhiteSpace(_currentChar))
                    continue;

                if (TryNumber(lastChar))
                    continue;

                if (TryString())
                    continue;

                if (TryStartGroup())
                    continue;

                if (TryComma())
                    continue;

                if (TryOperator())
                    continue;

                if (TryEndGroup())
                    continue;
                throw new ParseException(Resources.InvalidCharacterEncountered + _currentChar);
            }
            while (_expressionReader.Peek() != -1);

            ProcessSymbolStack();
        }
               
        private bool TryString()
        {
            if (!char.IsLetter(_currentChar))
                return false;

            _buffer.Length = 0;
            _buffer.Append(_currentChar);

            char p = (char)_expressionReader.Peek();
            while (char.IsLetter(p) || char.IsNumber(p))
            {
                _buffer.Append((char)_expressionReader.Read());
                p = (char)_expressionReader.Peek();
            }

            if (_variables.ContainsKey(_buffer.ToString()))
            {
                double value = _variables[_buffer.ToString()];
                NumberExpression expression = new NumberExpression(value);
                _expressionQueue.Enqueue(expression);

                return true;
            }

            throw new ParseException(Resources.InvalidVariableEncountered + _buffer);
        }

        private bool TryStartGroup()
        {
            if (_currentChar != '(')
                return false;

            if (PeekNextNonWhitespaceChar() == ',')
            {
                throw new ParseException(Resources.InvalidCharacterEncountered + ",");
            }

            _symbolStack.Push(_currentChar.ToString());
            _nestedGroupDepth++;
            return true;
        }

        private bool TryComma()
        {
            if (_currentChar != ',')
                return false;

            char nextChar = PeekNextNonWhitespaceChar();
            if (nextChar == ',')
            {
                throw new ParseException(Resources.InvalidCharacterEncountered + _currentChar);
            }

            return true;
        }

        private char PeekNextNonWhitespaceChar()
        {
            int next = _expressionReader.Peek();
            while (next != -1 && char.IsWhiteSpace((char)next))
            {
                _expressionReader.Read();
                next = _expressionReader.Peek();
            }
            return (char)next;
        }


        private bool TryEndGroup()
        {
            if (_currentChar != ')')
                return false;

            bool hasStart = false;

            while (_symbolStack.Count > 0)
            {
                string p = _symbolStack.Pop();
                if (p == "(")
                {
                    hasStart = true;

                    if (_symbolStack.Count == 0)
                        break;

                    string n = _symbolStack.Peek();

                    _nestedGroupDepth--;

                    break;
                }

                IExpression e = GetExpressionFromSymbol(p);
                _expressionQueue.Enqueue(e);
            }

            if (!hasStart)
                throw new ParseException(Resources.UnbalancedParentheses);

            return true;
        }

        private bool TryOperator()
        {
            if (!OperatorExpression.IsSymbol(_currentChar))
                return false;

            bool repeat;
            string s = _currentChar.ToString();

            do
            {
                string p = _symbolStack.Count == 0 ? string.Empty : _symbolStack.Peek();
                repeat = false;
                if (_symbolStack.Count == 0)
                    _symbolStack.Push(s);
                else if (p == "(")
                    _symbolStack.Push(s);
                else if (Precedence(s) > Precedence(p))
                    _symbolStack.Push(s);
                else
                {
                    IExpression e = GetExpressionFromSymbol(_symbolStack.Pop());
                    _expressionQueue.Enqueue(e);
                    repeat = true;
                }
            } while (repeat);

            return true;
        }

        private bool TryNumber(char lastChar)
        {
            bool isNumber = NumberExpression.IsNumber(_currentChar);
            // only negative when last char is group start or symbol
            bool isNegative = NumberExpression.IsNegativeSign(_currentChar) &&
                              (lastChar == '\0' || OperatorExpression.IsSymbol(lastChar));

            if (!isNumber && !isNegative)
                return false;

            _buffer.Length = 0;
            _buffer.Append(_currentChar);

            char p = (char)_expressionReader.Peek();
            while (NumberExpression.IsNumber(p))
            {
                _currentChar = (char)_expressionReader.Read();
                _buffer.Append(_currentChar);
                p = (char)_expressionReader.Peek();
            }

            if (!(double.TryParse(_buffer.ToString(), out double value)))
                throw new ParseException(Resources.InvalidNumberFormat + _buffer);

            NumberExpression expression = new NumberExpression(value);
            _expressionQueue.Enqueue(expression);

            return true;
        }

        private void ProcessSymbolStack()
        {
            while (_symbolStack.Count > 0)
            {
                string p = _symbolStack.Pop();
                if (p.Length == 1 && p == "(")
                    throw new ParseException(Resources.UnbalancedParentheses);

                IExpression e = GetExpressionFromSymbol(p);
                _expressionQueue.Enqueue(e);
            }
        }

        private IExpression GetExpressionFromSymbol(string p)
        {
            IExpression e;
            if (OperatorExpression.IsSymbol(Convert.ToChar(p)))
            {
                e = new OperatorExpression(p);
                _expressionCache.Add(p, e);
            }
            else
                throw new ParseException(Resources.InvalidSymbolOnStack + p);

            return e;
        }

        private static int Precedence(string c)
        {
            if (c.Length == 1 && (c[0] == '*' || c[0] == '/' || c[0] == '%'))
                return 2;

            return 1;
        }

        private double CalculateFromQueue()
        {
            double result;
            _calculationStack.Clear();

            foreach (IExpression expression in _expressionQueue)
            {
                if (_calculationStack.Count < expression.ArgumentCount)
                    throw new ParseException(Resources.NotEnoughNumbers + expression);

                _parameters.Clear();
                for (int i = 0; i < expression.ArgumentCount; i++)
                    _parameters.Push(_calculationStack.Pop());

                _calculationStack.Push(expression.Evaluate.Invoke(_parameters.ToArray()));
            }

            result = _calculationStack.Pop();

            if (_calculationStack.Any())
            {
                throw new ParseException(string.Format("{0}Items '{1}' were remaining on calculation stack.", Resources.InvalidSymbolOnStack, string.Join(", ", _calculationStack)));
            }

            return result;
        }

        #region IDisposable Members

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and  managed resources
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; 
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_expressionReader != null)
                {
                    _expressionReader.Dispose();
                    _expressionReader = null;
                }
            }
        }

        #endregion
    }
}
