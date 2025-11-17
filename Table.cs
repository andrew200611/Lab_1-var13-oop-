using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorLogic
{
    public class Table
    {

        private Cell[,] cells;
        private int rows;
        private int columns;
        private SimpleCalculator calculator;


        public Table(int initialRows, int initialColumns)
        {
            rows = initialRows;
            columns = initialColumns;
            cells = new Cell[rows, columns];
            calculator = new SimpleCalculator(this);

            CreateAllCells();
        }

        public int column { get { return columns; } }
        public int row { get { return rows; } }

        public string GetCellId(int row, int col)
        {
            string rowPart = "";
            string columnPart = "";

            rowPart = (row + 1).ToString();

            int columnIndex = col;
            char Letter = (char)('A' + columnIndex);
            columnPart = Letter.ToString();

            return columnPart + rowPart;

        }


        public Cell GetCell(int row, int col)
        {
            return cells[row, col];
        }

        public void AddColumn()
        {
            columns++;
            Cell[,] newCells = new Cell[rows, columns];


            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns - 1; col++)
                {
                    newCells[row, col] = cells[row, col];
                }
            }


            for (int row = 0; row < rows; row++)
            {
                string cellId = GetCellId(row, columns - 1);
                newCells[row, columns - 1] = new Cell(cellId, calculator);
            }

            cells = newCells;

        }

        public void AddRow()
        {
            rows++;
            Cell[,] newCells = new Cell[rows, columns];
            for (int row = 0; row < rows - 1; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    newCells[row, col] = cells[row, col];
                }
            }

            for (int col = 0; col < columns; col++)
            {
                string cellId = GetCellId(rows - 1, col);
                newCells[rows - 1, col] = new Cell(cellId, calculator);
            }
            cells = newCells;
        }
        public void CreateAllCells()
        {
            for (int row = 0; row < rows; row++)
                for (int col = 0; col < columns; col++)
                {
                    string cellId = GetCellId(row, col);
                    cells[row, col] = new Cell(cellId, calculator);
                }

        }


    }
}
