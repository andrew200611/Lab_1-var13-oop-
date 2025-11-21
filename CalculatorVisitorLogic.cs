using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;

namespace CalculatorLogic
{
    public partial class CalculatorVisitor : CalculatorBaseVisitor<double>
    {
        private Table table;

        public CalculatorVisitor(Table table)
        {
            this.table = table;
        }

        public override double VisitExpression([NotNull] CalculatorParser.ExpressionContext context)
        {
            return Visit(context.operand());
        }
        

        public override double VisitCellOperand([NotNull] CalculatorParser.CellOperandContext context)
        {
            return GetCellValue(context.GetText());
        }


        public override double VisitParenthesizedOperand([NotNull] CalculatorParser.ParenthesizedOperandContext context)
        {
            return Visit(context.operand());
        }


        public override double VisitAdditiveOperand([NotNull] CalculatorParser.AdditiveOperandContext context)
        {
            double left = Visit(context.operand(0));
            double right = Visit(context.operand(1));

            if (context.op.Text == "+")
            {
                return left + right;
            }
            else
            {
                return left - right;
            }
        }


        public override double VisitMultiplicativeOperand([NotNull] CalculatorParser.MultiplicativeOperandContext context)
        {
            double left = Visit(context.operand(0));
            double right = Visit(context.operand(1));

            if (context.op.Text == "*")
            {
                return left * right;
            }
            else
            {
                if (right == 0)
                    throw new DivideByZeroException("Ділення на нуль");
                return left / right;
            }
        }


        public override double VisitUnaryMinusOperand([NotNull] CalculatorParser.UnaryMinusOperandContext context)
        {
            
            double value = Visit(context.operand());
            return -value;
        }

        public override double VisitFunctionOperand([NotNull] CalculatorParser.FunctionOperandContext context)
        {
            return Visit(context.functionCall());
        }


        public override double VisitIncFunction([NotNull] CalculatorParser.IncFunctionContext context)
        {
            double value = Visit(context.operand());
            return value + 1;
        }


        public override double VisitDecFunction([NotNull] CalculatorParser.DecFunctionContext context)
        {
            double value = Visit(context.operand());
            return value - 1;
        }


        public override double VisitMaxFunction([NotNull] CalculatorParser.MaxFunctionContext context)
        {
            double a = Visit(context.operand(0));
            double b = Visit(context.operand(1));
            return Math.Max(a, b);
        }


        public override double VisitMinFunction([NotNull] CalculatorParser.MinFunctionContext context)
        {
            double a = Visit(context.operand(0));
            double b = Visit(context.operand(1));
            return Math.Min(a, b);
        }


        public override double VisitMmaxFunction([NotNull] CalculatorParser.MmaxFunctionContext context)
        {
            return ProcessOperandList(context.operandList(), true);
        }


        public override double VisitMminFunction([NotNull] CalculatorParser.MminFunctionContext context)
        {
            return ProcessOperandList(context.operandList(), false);
        }


        private double ProcessOperandList(CalculatorParser.OperandListContext context, bool isMax)
        {
            if (context.operand().Length == 0)
                throw new ArgumentException("Порожній список параметрів");

            double result = Visit(context.operand(0));

            for (int i = 1; i < context.operand().Length; i++)
            {
                double value = Visit(context.operand(i));
                if (isMax)
                    result = Math.Max(result, value);
                else
                    result = Math.Min(result, value);
            }

            return result;
        }


        private double GetCellValue(string cellReference)
        {
            try
            {
                int i = 0;
                while (i < cellReference.Length && char.IsLetter(cellReference[i]))
                    i++;

                string columnPart = cellReference.Substring(0, i);
                string rowPart = cellReference.Substring(i);

                int rowNumber = int.Parse(rowPart);

                int col = 0;
                foreach (char c in columnPart)
                {
                    col = col * 26 + (c - 'A' + 1);
                }
                col--;

                int row = rowNumber - 1;

                var cell = table.GetCell(row, col);

                if (cell.HasError)
                    throw new ArgumentException("Помилка");

                return cell.Value;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Помилка");
            }
        }


        public override double VisitNumberOperand([NotNull] CalculatorParser.NumberOperandContext context)
        {
            try
            {
                string numberText = context.GetText();


                if (numberText.Length > 15)
                    throw new ArgumentException("Число занадто довге");

                double result = double.Parse(numberText);


                if (double.IsInfinity(result) || double.IsNaN(result))
                    throw new ArgumentException("Число занадто велике");

                return result;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new ArgumentException("Невірне число");
            }
        }


    }
}