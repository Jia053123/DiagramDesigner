using ShapeGrammarEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;

namespace DiagramDesignerModel
{
	class GrammarRulesStore
	{
		internal GrammarRulesDataTable CurrentRulesInfoDataTable { get; } = new GrammarRulesDataTable();

		private HashSet<GrammarRule> GrammarRules = new HashSet<GrammarRule>();

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Default");

        internal void CreateNewRuleFromExample(PolylinesGeometry leftHandGeometry, PolylinesGeometry rightHandGeometry)
        {
            GrammarRule newRule = GrammarRule.CreateGrammarRuleFromOneExample(leftHandGeometry, rightHandGeometry, out _);
            this.GrammarRules.Add(newRule);

            try
            {
                var newRow = this.CurrentRulesInfoDataTable.NewRow();
                newRow["Name"] = newRule.id.ToString();
                newRow["ID"] = newRule.id;
                newRow["Sample Count"] = newRule.SampleCount;
                this.CurrentRulesInfoDataTable.Rows.Add(newRow);
            }
            catch (System.Data.ConstraintException ex)
            {
                Logger.Error(ex, "Grammar Table Constraint Failed");
            }
        }

        /// <summary>
        /// return the corresponding GrammarRule object given its id. If id is not found, return null
        /// </summary>
        internal GrammarRule GetRuleById(Guid guid) => this.GrammarRules.Where(i => i.id == guid).SingleOrDefault();

        /// <summary>
        /// Call this method whenever a rule is updated to regenerate its info
        /// </summary>
        internal void RuleUpdated(Guid ruleId)
		{
            var updatedRule = this.GetRuleById(ruleId);
            var rowToUpdate = this.CurrentRulesInfoDataTable.AsEnumerable().SingleOrDefault(row => row.Field<Guid>("ID") == ruleId); 
            if (rowToUpdate is null)
			{
                throw new ArgumentException("invalid id");
			}
            this.UpdateRow(updatedRule, rowToUpdate);
		}

        private void UpdateRow(GrammarRule updatedRule, DataRow currentRulesInfoDataTableRowToUpdate)
		{
            currentRulesInfoDataTableRowToUpdate["Name"] = updatedRule.id.ToString();
            currentRulesInfoDataTableRowToUpdate["Sample Count"] = updatedRule.SampleCount;
		}
    }
}
