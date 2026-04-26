using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MiniExcel
{
    public static class FileManager
    {
        public static void SaveCSV(DataGridView grid, string path)
        {
            StringBuilder sb = new StringBuilder();

            for (int r = 0; r < grid.Rows.Count; r++)
            {
                for (int c = 0; c < grid.Columns.Count; c++)
                {
                    var val = grid[c, r].Value;
                    sb.Append(val != null ? val.ToString() : "");
                    if (c < grid.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
            }

            File.WriteAllText(path, sb.ToString());
        }

        public static void LoadCSV(DataGridView grid, string path)
        {
            var lines = File.ReadAllLines(path);

            for (int r = 0; r < lines.Length && r < grid.Rows.Count; r++)
            {
                var cols = lines[r].Split(',');

                for (int c = 0; c < cols.Length && c < grid.Columns.Count; c++)
                {
                    grid[c, r].Value = cols[c];
                }
            }
        }
    }
}