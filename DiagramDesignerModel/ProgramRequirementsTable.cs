using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiagramDesignerModel
{
    public class ProgramRequirementsTable : DataTable
    {
        public ProgramRequirementsTable()
        {
            var nameColumn = new DataColumn();
            nameColumn.DataType = System.Type.GetType("System.String");
            nameColumn.ColumnName = "Name";
            nameColumn.ReadOnly = false;
            nameColumn.Unique = true;
            nameColumn.DefaultValue = "Unnamed";
            this.Columns.Add(nameColumn);

            var countColumn = new DataColumn();
            countColumn.DataType = System.Type.GetType("System.Int32");
            countColumn.ColumnName = "Count";
            countColumn.ReadOnly = false;
            countColumn.Unique = false;
            countColumn.DefaultValue = 1;
            this.Columns.Add(countColumn);

            var areaColumn = new DataColumn();
            areaColumn.DataType = System.Type.GetType("System.Double");
            areaColumn.ColumnName = "Area";
            areaColumn.ReadOnly = false;
            areaColumn.Unique = false;
            areaColumn.DefaultValue = 0;
            this.Columns.Add(areaColumn);

            var totalAreaColumn = new DataColumn();
            totalAreaColumn.DataType = System.Type.GetType("System.Double");
            totalAreaColumn.ColumnName = "TotalArea";
            totalAreaColumn.Expression = "Count * Area";
            totalAreaColumn.ReadOnly = true;
            totalAreaColumn.Unique = false;
            totalAreaColumn.DefaultValue = 0;
            this.Columns.Add(totalAreaColumn);

            var tagColumn = new DataColumn();
            tagColumn.DataType = System.Type.GetType("System.String");
            tagColumn.ColumnName = "Tag";
            tagColumn.ReadOnly = false;
            tagColumn.Unique = false;
            tagColumn.DefaultValue = "NoTag";
            this.Columns.Add(tagColumn);

            //var isMechanicalColumn = new DataColumn();
            //isMechanicalColumn.DataType = System.Type.GetType("System.Bool");
            //isMechanicalColumn.ColumnName = "Mechanical?";
            //isMechanicalColumn.ReadOnly = false;
            //isMechanicalColumn.Unique = false;
            //isMechanicalColumn.DefaultValue = false;
            //this.Columns.Add(isMechanicalColumn);

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
