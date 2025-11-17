using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorLogic
{
    public class SpreadsheetData
    {
        public int Rows { get; set; }
        public int Columns { get; set; }

        public string[][] Expressions { get; set; }
    }
}
