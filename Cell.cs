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


                if (ContainsSelfReference(expression))
                {
                    throw new ArgumentException("Посилаємось самі на себе");
                }


                Value = calculator.Calculate(expression);
                HasError = false;
            }
            catch (DivideByZeroException ex)
            {
                HasError = true;
                Value = 0;

            }
            catch (ArgumentException ex)
            {
                HasError = true;
                Value = 0;

            }
            catch (Exception ex)
            {
                HasError = true;
                Value = 0;

            }
        }


        private bool ContainsSelfReference(string expr)
        {
            try
            {

                return expr.Contains(Id);
            }
            catch
            {
                return false;
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
