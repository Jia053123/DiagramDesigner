using ShapeGrammarEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiagramDesignerModel
{
	class GrammarRulesStore
	{
		internal GrammarRulesDataTable CurrentRulesInfo { get; } = new GrammarRulesDataTable();

		private HashSet<GrammarRule> GrammarRules = new HashSet<GrammarRule>();

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

        private GrammarRule GetRuleById(Guid guid) => this.GrammarRules.Where(i => i.id == guid).FirstOrDefault();
    }
}
