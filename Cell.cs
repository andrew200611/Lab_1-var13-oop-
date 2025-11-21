using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorLogic
{
    public class Cell
    {


        public double Value { get; private set; }
        public string Id { get; private set; }
        public bool HasError { get; private set; }
        private string expression;
        private SimpleCalculator calculator;
        public string Expression
        {
            get
            {
                return expression;
            }
            set
            {
                if (value == null)
                {
                    expression = "";
                }
                else
                {
                    expression = value;
                }
                CalculateValue();
            }
        }

        public void Refresh()
        {
            CalculateValue();
        }
        private void CalculateValue()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(expression))
                {
                    Value = 0;
                    HasError = false;
                    return;
                }

               
                Value = calculator.Calculate(expression, Id);

                HasError = false;
            }
            catch
            {
               
                HasError = true;
                Value = 0;
            }
        }


       
        public Cell(string cellId, SimpleCalculator calc)
        {
            Id = cellId;
            expression = "";
            Value = 0;
            HasError = false;
            calculator = calc;
        }


    }
}
