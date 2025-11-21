using Antlr4.Runtime;
using System;
using System.Collections.Generic;

namespace CalculatorLogic;

public class SimpleCalculator
{
    private Table table;

    public SimpleCalculator(Table table)
    {
        this.table = table;
    }

    
    public double Calculate(string expression, string currentCellId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(expression)) return 0;

           
            var visited = new HashSet<string> { currentCellId };
           
            CheckLoop(expression, visited);

           
            return EvaluateExpression(expression);
        }
        catch (Exception)
        {
           
            throw new ArgumentException("Помилка обчислення");
        }
    }

    
    private void CheckLoop(string expression, HashSet<string> visited)
    {
        if (string.IsNullOrWhiteSpace(expression)) return;

        
        var lexer = new CalculatorLexer(new AntlrInputStream(expression));
        var tokens = lexer.GetAllTokens();

        foreach (var token in tokens)
        {
            if (token.Type == CalculatorLexer.CELL_REF)
            {
                string refId = token.Text; 

               
                if (visited.Contains(refId))
                {
                    throw new ArgumentException("Циклічне посилання");
                }

                
                var newVisited = new HashSet<string>(visited);
                newVisited.Add(refId);

                
                string refExpression = GetExpressionFromCellId(refId);

                
                CheckLoop(refExpression, newVisited);
            }
        }
    }

   
    private string GetExpressionFromCellId(string cellId)
    {
        try
        {
            
            int i = 0;
            while (i < cellId.Length && char.IsLetter(cellId[i])) i++;
            string colPart = cellId.Substring(0, i);
            string rowPart = cellId.Substring(i);

            int colIndex = 0;
            foreach (char c in colPart) colIndex = colIndex * 26 + (c - 'A' + 1);
            colIndex--;
            int rowIndex = int.Parse(rowPart) - 1;

            
            if (rowIndex < 0 || rowIndex >= table.row || colIndex < 0 || colIndex >= table.column)
                return "";

            return table.GetCell(rowIndex, colIndex).Expression;
        }
        catch { return ""; }
    }

    public double EvaluateExpression(string expression)
    {
        try
        {
            var inputStream = new AntlrInputStream(expression);
            var lexer = new CalculatorLexer(inputStream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CalculatorParser(tokens);

           

            var parseTree = parser.expression();

            if (parser.NumberOfSyntaxErrors > 0)
                throw new ArgumentException("Синтаксис");

            var visitor = new CalculatorVisitor(table);
            return visitor.Visit(parseTree);
        }
        catch
        {
            throw new ArgumentException("Помилка");
        }
    }
}