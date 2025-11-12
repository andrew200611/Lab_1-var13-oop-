
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.Maui.Controls;
using System;
using System.Text.Json;
using CommunityToolkit.Maui.Storage;
using System.Text;

namespace MauiApp1;



public partial class MainPage : ContentPage
{


    private Table table;
    public MainPage()
    {
        InitializeComponent();
        table = new Table(3, 3);
        UpdateSpreadsheetGrid();

    }
    private void UpdateSpreadsheetGrid()
    {
        SpreadsheetGrid.Children.Clear();
        SpreadsheetGrid.ColumnDefinitions.Clear();
        SpreadsheetGrid.RowDefinitions.Clear();


        ColumnHeaderGrid.Children.Clear();
        ColumnHeaderGrid.ColumnDefinitions.Clear();

        RowHeaderGrid.Children.Clear();
        RowHeaderGrid.RowDefinitions.Clear();


        for (int row = 0; row < table.row; row++)
        {

            SpreadsheetGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(45) });
            RowHeaderGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(45) });
            var headerLabel = new Label
            {
                Text = (row + 1).ToString(),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                WidthRequest = 40,
                HeightRequest = 40
            };
            Grid.SetRow(headerLabel, row);
            RowHeaderGrid.Children.Add(headerLabel);

        }

        for (int col = 0; col < table.column; col++)
        {
            SpreadsheetGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            ColumnHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var headerLabel = new Label
            {
                Text = ((char)('A' + col)).ToString(),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                WidthRequest = 80,
                HeightRequest = 40,

            };
            Grid.SetColumn(headerLabel, col);
            ColumnHeaderGrid.Children.Add(headerLabel);
        }

        for (int row = 0; row < table.row; row++)
        {
            for (int col = 0; col < table.column; col++)
            {
                var cell = table.GetCell(row, col);
                var cellEntry = new Entry
                {

                    WidthRequest = 80,
                    HeightRequest = 40,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                if (cell.HasError)
                {
                    cellEntry.Text = "ERROR";
                    cellEntry.BackgroundColor = Colors.Red;
                }
                else
                {

                    if (cell.Value == 0 && string.IsNullOrWhiteSpace(cell.Expression))
                    {
                        cellEntry.Text = "";
                    }
                    else
                    {
                        cellEntry.Text = cell.Value.ToString();
                    }

                }

                int currentRow = row;
                int currentCol = col;

                //вираз/значення
                cellEntry.Focused += (sender, e) =>
                {
                    var entry = sender as Entry;
                    var targetCell = table.GetCell(currentRow, currentCol);

                    entry.Text = targetCell.Expression;
                };

                cellEntry.Unfocused += (sender, e) =>
                {
                    var entry = sender as Entry;
                    var targetCell = table.GetCell(currentRow, currentCol);
                    targetCell.Expression = entry.Text;
                    UpdateSpreadsheetGrid();
                };
                Grid.SetRow(cellEntry, row);
                Grid.SetColumn(cellEntry, col);
                SpreadsheetGrid.Children.Add(cellEntry);
            }
        }
    }
    private void AddRowButton_Clicked(object sender, EventArgs e)
    {
        table.AddRow();
        UpdateSpreadsheetGrid();
    }
    private void AddColumnButton_Clicked(object sender, EventArgs e)
    {
        table.AddColumn();
        UpdateSpreadsheetGrid();
    }
    private void InfoButton_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Інформація", $"Виконав Цаліков Андрій Володимирович\n Група К-24\n Варіант 13", "OK");
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            
            var data = new SpreadsheetData
            {
                Rows = table.row,
                Columns = table.column,
                
                Expressions = new string[table.row][]
            };

           
            for (int r = 0; r < table.row; r++)
            {
               
                data.Expressions[r] = new string[table.column];
                for (int c = 0; c < table.column; c++)
                {
                    
                    data.Expressions[r][c] = table.GetCell(r, c).Expression;
                }
            }

           
            string jsonString = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

            
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString));
            await CommunityToolkit.Maui.Storage.FileSaver.Default.SaveAsync("spreadsheet.json", stream, CancellationToken.None);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", "Не вдалося зберегти файл: " , "OK");
        }
    }

    private async void OpenButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            
            var result = await Microsoft.Maui.Storage.FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Виберіть файл електронної таблиці",
            });

            if (result == null) return; 

           
            string jsonString = await File.ReadAllTextAsync(result.FullPath);

            
            var data = System.Text.Json.JsonSerializer.Deserialize<SpreadsheetData>(jsonString);

            if (data == null) throw new Exception("Невірний формат файлу");

            
            table = new Table(data.Rows, data.Columns);

            
            for (int r = 0; r < data.Rows; r++)
            {
                for (int c = 0; c < data.Columns; c++)
                {
                    
                    table.GetCell(r, c).Expression = data.Expressions[r][c];
                }
            }

          
            UpdateSpreadsheetGrid();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", "Не вдалося відкрити файл ", "OK");
        }
    }




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


    public class SpreadsheetData
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        
        public string[][] Expressions { get; set; }
    }

}