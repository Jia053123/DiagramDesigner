using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ShapeGrammarEngine
{
	public class LayersDataTable : DataTable
	{
		public LayersDataTable()
		{
			var nameColumn = new DataColumn();
			nameColumn.DataType = System.Type.GetType("System.String");
			nameColumn.ColumnName = "Name";
			nameColumn.ReadOnly = false;
			nameColumn.Unique = true;
			nameColumn.DefaultValue = "Unnamed";
			this.Columns.Add(nameColumn);

			var isOrthoColumn = new DataColumn();
			isOrthoColumn.DataType = System.Type.GetType("System.Boolean");
			isOrthoColumn.ColumnName = "IsVisible";
			isOrthoColumn.ReadOnly = false;
			isOrthoColumn.Unique = false;
			isOrthoColumn.DefaultValue = true;
			this.Columns.Add(isOrthoColumn);
		}
	}
}
