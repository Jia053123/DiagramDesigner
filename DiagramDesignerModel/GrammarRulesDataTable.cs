using System.Collections.Generic;
using System.Data;

namespace DiagramDesignerModel
{
	public class GrammarRulesDataTable : DataTable
    {
        public GrammarRulesDataTable()
        {
            var nameColumn = new DataColumn();
            nameColumn.DataType = System.Type.GetType("System.String");
            nameColumn.ColumnName = "Name";
            nameColumn.ReadOnly = false;
            nameColumn.Unique = true;
            nameColumn.DefaultValue = "Unnamed";
            this.Columns.Add(nameColumn);

            var idColumn = new DataColumn();
            idColumn.DataType = System.Type.GetType("System.Guid");
            idColumn.ColumnName = "ID";
            nameColumn.ReadOnly = true;
            nameColumn.Unique = true;
            this.Columns.Add(idColumn);
        }
	}
}
