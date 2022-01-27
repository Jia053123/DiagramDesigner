using ShapeGrammarEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerModel
{
	class RulesStore
	{
		internal GrammarRulesDataTable CurrentRulesInfo { get; } = new GrammarRulesDataTable();

		private List<GrammarRule> GrammarRules = new List<GrammarRule>();

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Default");

        internal void CreateNewRuleFromExample(PolylinesGeometry leftHandGeometry, PolylinesGeometry rightHandGeometry)
        {
            GrammarRule newRule = GrammarRule.CreateGrammarRuleFromOneExample(leftHandGeometry, rightHandGeometry, out _);
            this.GrammarRules.Add(newRule);

            try
            {
                var newRow = this.CurrentRulesInfo.NewRow();
                newRow["Name"] = newRule.id.ToString();
                newRow["ID"] = newRule.id;
                this.CurrentRulesInfo.Rows.Add(newRow);
            }
            catch (System.Data.ConstraintException ex)
            {
                Logger.Error(ex, "Grammar Table Constraint Failed");
            }
        }
    }
}
