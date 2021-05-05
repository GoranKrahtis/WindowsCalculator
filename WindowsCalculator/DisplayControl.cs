using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindowsCalculator
{
    public class DisplayControl
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public string DisplayValue { get; set; }
        public string StoredValue { get; set; }
        private readonly MathEvaluator Eval = new MathEvaluator();
        private bool UserEnteredParentheses = false;

        public const string BackspaceWithNoCharactersMessage = "No characters to erase.";

        public DisplayControl()
        {
            Value = "0";
            Text = "0";
        }

        public void Update(string NewValue, bool IsNewTerm = false, string Operation = "")
        {
            // Remove 0 if it is only current value
            if (Value == "0")
                Value = "";
            switch (NewValue)
            {
                case "BACKSPACE":
                    Backspace();
                    return;
                default:
                    break;
            }

            // Determine new value
            string UpdatedValue;
            if (IsNewTerm == true)
                if (Operation == "")
                    UpdatedValue = "";
                else
                    UpdatedValue = TransformCurrentValue(Value: Value, Transform: Operation);
            else if (Operation != "")
                UpdatedValue = TransformCurrentValue(Value: Value, Transform: Operation);
            else
                UpdatedValue = Value + NewValue;

            // TODO: Check validity of Updated Value

            // Update display text
            UpdateText(NewValue: NewValue, UpdatedValue: UpdatedValue, Operation: Operation);
            // Update current value
            Value = UpdatedValue;
        }

        private string TransformCurrentValue(string Value, string Transform)
        {
            if (UserEnteredParentheses)
                return string.Format("{0}(", Transform);
            else
                return string.Format("{0}({1})", Transform, Value);
        }

        public string UpdateText(string NewValue, string UpdatedValue, string Operation = "")
        {
            string result = Text;
            // Remove 0 if it is only current value
            if (result == "0")
                result = "";

            if (Operation != "")
                if (UserEnteredParentheses)
                    result += UpdatedValue;
                else
                    result = ReplaceLastOccurrence(Source: result, Find: Value, Replace: UpdatedValue);
            else
                result += NewValue;

            Text = result;
            return result;
        }

        private static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }

        public void Clear()
        {
            Text = "0";
            Value = "0";
        }

        public void Evaluate()
        {
            string result = Eval.Evaluate(Text).ToString();
            Value = result;
            Text = result;
            UserEnteredParentheses = false;
        }

        public void StoreValue(string ValueToStore)
        {
            StoredValue = ValueToStore;
        }

        public void StorePositiveValue(string ValueToStore)
        {
            StoredValue = string.Format("{0}", Math.Abs(double.Parse(ValueToStore)));
        }

        public void StoreNegativeValue(string ValueToStore)
        {
            StoredValue = string.Format("{0}", -1 * Math.Abs(double.Parse(ValueToStore)));
        }

        public void ClearMemory()
        {
            StoredValue = "";
        }

        public void RecallMemory()
        {
            Update(NewValue: this.StoredValue);
        }

        public void Backspace()
        {
            try
            {
                Text = Text.Remove(Text.Length - 1);
                if(Text.Length<1)
                    Text = "";
            }
            catch (IndexOutOfRangeException e)
            {
                Text = e.Message;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Text = e.Message;
            }

            try
            {
                Value = Value.Remove(Value.Length - 1);
            }
            catch (IndexOutOfRangeException e)
            {
                Text = e.Message;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Text = e.Message;
            }
        }
    }
}
