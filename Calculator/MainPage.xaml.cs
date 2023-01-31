using System;
using System.Collections.Generic;
using System.Collections;
using Xamarin.Forms;

namespace Calculator
{
    public partial class MainPage : ContentPage
    {
        private string currentValues = ""; // Used to display the pressed numbers
        private string doublePlaceholder = ""; // Keeping track of double value until next operator is pressed

        private ArrayList equation = new ArrayList();
        private Stack<double> values = new Stack<double>();
        private Stack<char> operators = new Stack<char>();

        public MainPage()
        {
            InitializeComponent();
        }

        // Gets called whenever a number or '.' is pressed
        public void OnSelectNumber(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            currentValues += button.Text;
            resultText.Text = currentValues;

            // When an operator is pressed this will be pushed to the values stack
            doublePlaceholder += button.Text;
        }

        public void OnSelectOperator(object sender, EventArgs e)
        {
            // On operator, reset double placeholder
            if (doublePlaceholder.Length > 0)
            {
                equation.Add(doublePlaceholder);
                doublePlaceholder = "";
            }
            
            Button button = (Button)sender;
            equation.Add(button.Text);
            currentValues += button.Text;
            resultText.Text = currentValues;
        }

        // Resets text on calulator
        public void OnClear(object sender, EventArgs e)
        {
            resultText.Text = "";
            Reset();
        }

        // Performs calculations
        public void OnEquals(object sender, EventArgs e)
        {
            equation.Add(doublePlaceholder);

            // If bitshift, do not execute normal calculations
            if (currentValues.Contains("<") || currentValues.Contains(">"))
            {
                OnBitShiftCalculation();
                return;
            }

            for (int i = 0; i < equation.Count; i++)
            {
                if (equation[i].Equals(""))
                {
                    continue;
                }

                // If its a number, add to value stack
                if (double.TryParse((string) equation[i], out double n))
                {
                    values.Push(n);
                }
                
                // If its a closing bracket, solve all math inside brackets.
                else if (equation[i].Equals(")"))
                {
                    while (operators.Peek() != '(')
                    {
                        values.Push(Calculation());
                    }
                    // Pop '(' as we do not need it
                    operators.Pop();
                }
                
                // If next  is an operator
                else
                {
                    while (operators.Count > 0 && IsHigherPriority(char.Parse((string) equation[i]), operators.Peek()))
                    {
                        values.Push(Calculation());
                    }
                    operators.Push(char.Parse((string) equation[i]));
                }
            }

            while (operators.Count > 0)
            {
                values.Push(Calculation());
            }

            // Set result text and empty stacks
            resultText.Text = values.Pop().ToString();
            Reset();
        }

        // Execute this for bit shift operations
        private void OnBitShiftCalculation()
        {
            try
            {
                int val1 = int.Parse((string)equation[0]);
                int val2 = int.Parse((string)equation[2]);
                int bitShiftResult = 0;

                switch (equation[1])
                {
                    case "<<":
                        bitShiftResult = val1 << val2;
                        break;
                    case ">>":
                        bitShiftResult = val1 >> val2;
                        break;
                }

                // Set result text and empty stacks
                resultText.Text = bitShiftResult.ToString();
                Reset();
            }
            catch (Exception)
            {
                resultText.Text = "invalid input";
                Reset();
            }
        }

        /**
         * Check wether the coming operator has higher priority than the last
         * Operator1 is the one currently being checked, operator2 is the one on top of the stack
        */
        private bool IsHigherPriority(char operator1, char operator2)
        {
            if (operator2 == '(' || operator1 == '(') return false;
            else if ((operator1 == '*' || operator1 == '/' || operator1 == '%') && (operator2 == '+' || operator2 == '-')) return false;
            else if ((operator1 == '(') && (operator2 == '/' || operator2 == '*' || operator2 == '%')) return false;
            else return true;
        }

        // Perform basic calculation on the two most recent values in the stack
        private double Calculation()
        {
            double val1 = values.Pop();
            double val2 = values.Pop();
            char lastOp = operators.Pop();
            double tempResult = 0.0;

            switch (lastOp)
            {
                case '+':
                    tempResult = val1 + val2;
                    break;
                case '-':
                    tempResult = val2 - val1;
                    break;
                case '*':
                    tempResult = val1 * val2;
                    break;
                case '/':
                    tempResult = val2 / val1;
                    break;
                case '%':
                    tempResult = val2 % val1;
                    break;
            }

            return tempResult;
        }

        // When a calculation is done, call this in order to prepare for a new equation
        private void Reset()
        {
            doublePlaceholder = "";
            currentValues = "";
            equation.Clear();
            values.Clear();
            operators.Clear();
        }
    }
}