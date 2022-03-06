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
            idColumn.ReadOnly = true;
            idColumn.Unique = true;
            this.Columns.Add(idColumn);

            var sampleCountColumn = new DataColumn();
            sampleCountColumn.DataType = System.Type.GetType("System.Int32");
            sampleCountColumn.ColumnName = "Sample Count";
            sampleCountColumn.ReadOnly = true;
            sampleCountColumn.Unique = false;
            sampleCountColumn.DefaultValue = 0;
            this.Columns.Add(sampleCountColumn);
        }
	}
}
