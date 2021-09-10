using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ShapeGrammarEngine
{
    public class ShapesDataTable : DataTable
    {
        public ShapesDataTable()
        {
            var nameColumn = new DataColumn();
            nameColumn.DataType = System.Type.GetType("System.String");
            nameColumn.ColumnName = "Name";
            nameColumn.ReadOnly = false;
            nameColumn.Unique = true;
            nameColumn.DefaultValue = "Shape1";
            this.Columns.Add(nameColumn);

            var isFixedColumn = new DataColumn();
            isFixedColumn.DataType = System.Type.GetType("System.Boolean");
            isFixedColumn.ColumnName = "IsFixed";
            isFixedColumn.ReadOnly = false;
            isFixedColumn.Unique = false;
            isFixedColumn.DefaultValue = true;
            this.Columns.Add(isFixedColumn);

            var countColumn = new DataColumn();
            countColumn.DataType = System.Type.GetType("System.Int32");
            countColumn.ColumnName = "Count";
            countColumn.ReadOnly = false;
            countColumn.Unique = false;
            countColumn.DefaultValue = 1;
            this.Columns.Add(countColumn);
        }
    }
}
