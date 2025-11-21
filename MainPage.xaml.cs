

using Microsoft.Maui.Controls;
using System;
using System.Text.Json;
using CommunityToolkit.Maui.Storage;
using System.Text;
using CalculatorLogic;
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
                    table.RecalculateAll();
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
        DisplayAlert("Інформація", "Виконав Цаліков Андрій Володимирович\n Група К-24\n Варіант 13", "OK");
    }

    private void DeleteRowButton_Clicked(object sender, EventArgs e)
    {
        if (table.row > 0)
        {
            table.DeleteRow();
            table.RecalculateAll();
            UpdateSpreadsheetGrid();
        }
    }

    private void DeleteColumnButton_Clicked(object sender, EventArgs e)
    {
        if (table.column > 0)
        {
            table.DeleteColumn();
            table.RecalculateAll();
            UpdateSpreadsheetGrid();
        }
    }
    private async void CloseButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Вихід", "Ви впевнені, що хочете вийти?", "Так", "Ні");
        if (answer)
        {
            Application.Current.Quit();
        }
    }
    
    private async Task<bool> SaveDataAsync()
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

            string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var result = await FileSaver.Default.SaveAsync("spreadsheet.json", stream, CancellationToken.None);

            return result.IsSuccessful;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", "Не вдалося зберегти файл", "OK");
            return false;
        }
    }
    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        await SaveDataAsync();
    }

    private async void OpenButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            bool answer = await DisplayAlert("Вихід", "Хочете зберегти поточний файл?", "Так", "Ні");
            if (answer)
            {
                await SaveDataAsync();
            }
            if (!answer)
            {
               
                bool continueAnyway = await DisplayAlert("Увага", "Файл не було збережено. Все одно відкрити новий?", "Так", "Ні");
                if (!continueAnyway) return;
            }
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




   
    

    


   

}