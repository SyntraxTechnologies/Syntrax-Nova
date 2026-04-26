using System.Collections.Generic;

namespace MiniExcel
{
    public class CellStorage
    {
        private Dictionary<string, CellModel> cells = new Dictionary<string, CellModel>();

        public CellModel Get(int r, int c)
        {
            string key = r + ":" + c;

            if (!cells.ContainsKey(key))
                cells[key] = new CellModel();

            return cells[key];
        }

        public void Set(int r, int c, CellModel model)
        {
            cells[r + ":" + c] = model;
        }
    }
}