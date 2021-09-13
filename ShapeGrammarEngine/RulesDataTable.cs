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

            var isOrthoColumn = new DataColumn();
            isOrthoColumn.DataType = System.Type.GetType("System.Boolean");
            isOrthoColumn.ColumnName = "IsOrtho";
            isOrthoColumn.ReadOnly = false;
            isOrthoColumn.Unique = false;
            isOrthoColumn.DefaultValue = false;
            this.Columns.Add(isOrthoColumn);

            var countColumn = new DataColumn();
            countColumn.DataType = System.Type.GetType("System.Int32");
            countColumn.ColumnName = "Count";
            countColumn.ReadOnly = false;
            countColumn.Unique = false;
            countColumn.DefaultValue = 1;
            this.Columns.Add(countColumn);

            var leftHandShapeColumn = new DataColumn();
            leftHandShapeColumn.DataType = System.Type.GetType("ShapeGrammarEngine.Shape");
            leftHandShapeColumn.ColumnName = "LeftHandShape";
            leftHandShapeColumn.ReadOnly = false;
            leftHandShapeColumn.Unique = false;
            leftHandShapeColumn.DefaultValue = new Shape(new HashSet<(int, int)>());
            this.Columns.Add(leftHandShapeColumn);

            var rightHandShapeColumn = new DataColumn();
            rightHandShapeColumn.DataType = System.Type.GetType("ShapeGrammarEngine.Shape");
            rightHandShapeColumn.ColumnName = "RightHandShape";
            rightHandShapeColumn.ReadOnly = false;
            rightHandShapeColumn.Unique = false;
            rightHandShapeColumn.DefaultValue = new Shape(new HashSet<(int, int)>());
            this.Columns.Add(rightHandShapeColumn);
        }
    }
}
