using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MiniExcel
{
    public class FormulaEngine
    {
        private DataGridView grid;

        public FormulaEngine(DataGridView grid)
        {
            this.grid = grid;
        }

        public string Evaluate(string input)
        {
            if (!input.StartsWith("="))
                return input;

            string expr = input.Substring(1);

            // Replace A1 references
            expr = Regex.Replace(expr, @"([A-Z])(\d+)", match =>
            {
                int col = match.Groups[1].Value[0] - 'A';
                int row = int.Parse(match.Groups[2].Value) - 1;

                var val = grid[col, row].Value;
                return val == null ? "0" : val.ToString();
            });

            try
            {
                DataTable dt = new DataTable();
                var result = dt.Compute(expr, "");
                return result.ToString();
            }
            catch
            {
                return "ERR";
            }
        }
    }
}