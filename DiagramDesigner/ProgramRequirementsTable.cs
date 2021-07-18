using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiagramDesigner
{
    class ProgramRequirementsTable : DataTable
    {
        public ProgramRequirementsTable()
        {
            var nameColumn = new DataColumn();
            nameColumn.DataType = System.Type.GetType("System.String");
            nameColumn.ColumnName = "Name";
            nameColumn.ReadOnly = false;
            nameColumn.Unique = true;
            this.Columns.Add(nameColumn);

            var countColumn = new DataColumn();
            countColumn.DataType = System.Type.GetType("System.Int32");
            countColumn.ColumnName = "Count";
            countColumn.ReadOnly = false;
            countColumn.Unique = false;
            this.Columns.Add(countColumn);

            var areaColumn = new DataColumn();
            areaColumn.DataType = System.Type.GetType("System.Double");
            areaColumn.ColumnName = "Area";
            areaColumn.ReadOnly = false;
            areaColumn.Unique = false;
            this.Columns.Add(areaColumn);

            var totalAreaColumn = new DataColumn();
            totalAreaColumn.DataType = System.Type.GetType("System.Double");
            totalAreaColumn.ColumnName = "TotalArea";
            totalAreaColumn.ReadOnly = true;
            totalAreaColumn.Unique = false;
            this.Columns.Add(totalAreaColumn);

            var tagColumn = new DataColumn();
            tagColumn.DataType = System.Type.GetType("System.String");
            tagColumn.ColumnName = "Tag";
            tagColumn.ReadOnly = false;
            tagColumn.Unique = false;
            this.Columns.Add(tagColumn);

            //var isCirculationColumn = new DataColumn();
            //isCirculationColumn.DataType = System.Type.GetType("System.Bool");
            //isCirculationColumn.ColumnName = "Circulation?";
            //isCirculationColumn.ReadOnly = false;
            //isCirculationColumn.Unique = false;
            //isCirculationColumn.DefaultValue = false;
            //this.Columns.Add(isCirculationColumn);
        }
    }
}
