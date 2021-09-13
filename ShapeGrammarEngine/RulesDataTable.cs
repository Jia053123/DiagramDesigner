using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ShapeGrammarEngine
{
    public class RulesDataTable : DataTable
    {
        public RulesDataTable()
        {
            var nameColumn = new DataColumn();
            nameColumn.DataType = System.Type.GetType("System.String");
            nameColumn.ColumnName = "Name";
            nameColumn.ReadOnly = false;
            nameColumn.Unique = true;
            nameColumn.DefaultValue = "Rule1";
            this.Columns.Add(nameColumn);

            var isExactColumn = new DataColumn();
            isExactColumn.DataType = System.Type.GetType("System.Boolean");
            isExactColumn.ColumnName = "IsExact";
            isExactColumn.ReadOnly = false;
            isExactColumn.Unique = false;
            isExactColumn.DefaultValue = true;
            this.Columns.Add(isExactColumn);

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
