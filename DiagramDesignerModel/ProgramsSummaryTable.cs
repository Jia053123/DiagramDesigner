using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DiagramDesignerModel
{
    public class ProgramsSummaryTable : DataTable
    {
        public ProgramsSummaryTable()
        {
            var nameColumn = new DataColumn();
            nameColumn.DataType = System.Type.GetType("System.String");
            nameColumn.ColumnName = "Name";
            nameColumn.ReadOnly = false;
            nameColumn.Unique = true;
            nameColumn.DefaultValue = "Unnamed";
            this.Columns.Add(nameColumn);

            var areaColumn = new DataColumn();
            areaColumn.DataType = System.Type.GetType("System.Double");
            areaColumn.ColumnName = "TotalArea";
            areaColumn.ReadOnly = false;
            areaColumn.Unique = false;
            areaColumn.DefaultValue = 0;
            this.Columns.Add(areaColumn);
        }
    }
}
