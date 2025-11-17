using Microsoft.VisualStudio.TestTools.UnitTesting;
using CalculatorLogic;
namespace TestProject1;

[TestClass] 
public class CalculatorTests
{
    
    private SimpleCalculator calculator;

    
    [TestInitialize]
    public void Setup()
    {
        
        
        var table = new Table(5, 5);
        calculator = new SimpleCalculator(table);
    }

   

    [TestMethod] // Тест 1
    public void Calculate_SimpleAddition_ReturnsCorrectSum()
    {
       
        string expression = "5+7";

        
        double result = calculator.Calculate(expression);

       
        Assert.AreEqual(12, result, "5+7 має дорівнювати 12");
    }

    [TestMethod] // Тест 2
    public void Calculate_IncFunction_ReturnsCorrectValue()
    {
       
        string expression = "inc(10)"; 

        
        double result = calculator.Calculate(expression);

       
        Assert.AreEqual(11, result, "inc(10) має дорівнювати 11");
    }

    [TestMethod] // Тест 3
    public void Calculate_DivisionByZero_ThrowsArgumentException()
    {
        
        string expression = "10/0";

       
        Assert.ThrowsException<ArgumentException>(() =>
        {
            calculator.Calculate(expression);
        });
    }

    
}
