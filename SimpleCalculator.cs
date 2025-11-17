using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace CalculatorLogic
{
    public class SimpleCalculator
    {
        private Table table;

        public SimpleCalculator(Table table)
        {
            this.table = table;
        }

        public double Calculate(string expression)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(expression))
                    return 0;




                return EvaluateExpression(expression);
            }

            catch (Exception ex)
            {
                throw new ArgumentException("Помилка обчислення");
            }
        }

        public double EvaluateExpression(string expression)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(expression))
                    return 0;

                var inputStream = new Antlr4.Runtime.AntlrInputStream(expression);
                var lexer = new CalculatorLexer(inputStream);
                var tokens = new Antlr4.Runtime.CommonTokenStream(lexer);
                var parser = new CalculatorParser(tokens);
                var parseTree = parser.expression();

                if (parser.NumberOfSyntaxErrors > 0)
                {

                    throw new ArgumentException("Невірний синтаксис виразу");
                }


                var visitor = new CalculatorVisitor(table);
                return visitor.Visit(parseTree);
            }
            catch (Exception ex)
            {

                throw new ArgumentException("Помилка");
            }
        }





    }
}
