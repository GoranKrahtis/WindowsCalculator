using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace WindowsCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _display;
        private readonly DisplayControl DisplayControl = new DisplayControl();
        private bool isShowingError = false;
        public string Display
        {
            get { return _display; }
            set
            {
                if (string.Equals(value, _display))
                    return;
                _display = value;
                OnPropertyChanged("Display");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void toggleErrorDisplay()
        {
            if (isShowingError)
            {
                errorDisplay.Opacity = 0.0;
                isShowingError = false;
            }
            else
            {
                errorDisplay.Opacity = 1.0;
                isShowingError = true;
            }

        }

        private void EvaluateExpression()
        {
            try
            {
                DisplayControl.Evaluate();
                Display = DisplayControl.Text;
            }
            catch (ParseException e)
            {
                Console.WriteLine("Invalid expression: {0}", e);
                toggleErrorDisplay();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonPressed(string NewValue, bool IsNewTerm = false, string Operation = "")
        {
            if (isShowingError)
                toggleErrorDisplay();

            DisplayControl.Update(NewValue: NewValue, IsNewTerm: IsNewTerm, Operation: Operation);
            Display = DisplayControl.Text;
        }

        private void CE_Click(object sender, RoutedEventArgs e)
        {
            Display = "0";
            DisplayControl.Clear();
        }

        private void C_Click(object sender, RoutedEventArgs e)
        {
            Display = "0";
            DisplayControl.Clear();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "BACKSPACE");
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "+", IsNewTerm: true);
        }

        private void Mult_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "*");
        }

        private void Div_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "/");
        }


        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "-", IsNewTerm: true);
        }

        private void Equal_Click(object sender, RoutedEventArgs e)
        {
            EvaluateExpression();
        }

        private void Zero_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "0");
        }

        private void One_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "1");
        }

        private void Two_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "2");
        }

        private void Three_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "3");
        }

        private void Four_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "4");
        }

        private void Five_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "5");
        }

        private void Six_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "6");
        }

        private void Seven_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "7");
        }

        private void Eight_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "8");
        }

        private void Nine_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "9");
        }

        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: ".");
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //keybindings
            switch (e.Key)
            {
                // Basic Digits
                case Key.D1:
                    One_Click(sender, e);
                    break;
                case Key.D2:
                    Two_Click(sender, e);
                    break;
                case Key.D3:
                    Three_Click(sender, e);
                    break;
                case Key.D4:
                    Four_Click(sender, e);
                    break;
                case Key.D5:
                    Five_Click(sender, e);
                    break;
                case Key.D6:
                    Six_Click(sender, e);
                    break;
                case Key.D7:
                    Seven_Click(sender, e);
                    break;
                case Key.D8:
                    Eight_Click(sender, e);
                    break;
                case Key.D9:
                    Nine_Click(sender, e);
                    break;
                case Key.D0:
                    Zero_Click(sender, e);
                    break;
                case Key.Decimal:
                    Decimal_Click(sender, e);
                    break;

                // Numpad digits
                case Key.NumPad1:
                    One_Click(sender, e);
                    break;
                case Key.NumPad2:
                    Two_Click(sender, e);
                    break;
                case Key.NumPad3:
                    Three_Click(sender, e);
                    break;
                case Key.NumPad4:
                    Four_Click(sender, e);
                    break;
                case Key.NumPad5:
                    Five_Click(sender, e);
                    break;
                case Key.NumPad6:
                    Six_Click(sender, e);
                    break;
                case Key.NumPad7:
                    Seven_Click(sender, e);
                    break;
                case Key.NumPad8:
                    Eight_Click(sender, e);
                    break;
                case Key.NumPad9:
                    Nine_Click(sender, e);
                    break;
                case Key.NumPad0:
                    Zero_Click(sender, e);
                    break;

                // Operations
                case Key.OemPlus:
                    Plus_Click(sender, e);
                    break;
                case Key.OemMinus:
                    Minus_Click(sender, e);
                    break;
                case Key.Multiply:
                    Mult_Click(sender, e);
                    break;
                case Key.Divide:
                    Div_Click(sender, e);
                    break;
                case Key.OemBackslash:
                    Div_Click(sender, e);
                    break;

                // Evaluation
                case Key.Enter:
                    Equal_Click(sender, e);
                    break;
                case Key.System:
                    Equal_Click(sender, e);
                    break;

                // Backspace
                case Key.Back:
                    Back_Click(sender, e);
                    break;
            }
        }

        private void Mod_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(NewValue: "%");
        }
    }
}
