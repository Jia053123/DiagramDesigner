using System.Collections.Generic;
using System.Data;

namespace ShapeGrammarEngine
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
            nameColumn.DefaultValue = "Rule1";
            this.Columns.Add(nameColumn);

            var isOrthoColumn = new DataColumn();
            isOrthoColumn.DataType = System.Type.GetType("System.Boolean");
            isOrthoColumn.ColumnName = "IsOrtho";
            isOrthoColumn.ReadOnly = true;
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
            leftHandShapeColumn.ReadOnly = true;
            leftHandShapeColumn.Unique = false;
            leftHandShapeColumn.DefaultValue = new Shape(new HashSet<Connection>());
            this.Columns.Add(leftHandShapeColumn);

            var rightHandShapeColumn = new DataColumn();
            rightHandShapeColumn.DataType = System.Type.GetType("ShapeGrammarEngine.Shape");
            rightHandShapeColumn.ColumnName = "RightHandShape";
            rightHandShapeColumn.ReadOnly = true;
            rightHandShapeColumn.Unique = false;
            rightHandShapeColumn.DefaultValue = new Shape(new HashSet<Connection>());
            this.Columns.Add(rightHandShapeColumn);
        }
    }
}
