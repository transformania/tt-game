using FluentValidation;
using System;
using FluentValidation.Internal;
using TT.Domain.Validation;

namespace TT.Domain.ValidatorSelectors
{
    public class IntersectRulesetValidatorSelector : IValidatorSelector
    {
        private readonly RuleSets rulesetsToExecute;

        public IntersectRulesetValidatorSelector(RuleSets rulesetsToExecute)
        {
            this.rulesetsToExecute = rulesetsToExecute;
        }

        /// <summary>
        /// Taken from the source of <see cref="RulesetValidatorSelector"/> with modifications to allow more than one ruleset and to use enums.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="propertyPath"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool CanExecute(IValidationRule rule, string propertyPath, ValidationContext context)
        {
            // if RuleSet is empty, it will be considered default
            RuleSets ruleSets = string.IsNullOrEmpty(rule.RuleSet) ? RuleSets.Default : (RuleSets) Enum.Parse(typeof(RuleSets), rule.RuleSet);

            if (ruleSets == RuleSets.Default)
            {
                if (rulesetsToExecute == RuleSets.None)
                {
                    return true;
                }
                else
                {
                    if (rule is IncludeRule)
                    {
                        return true;
                    }
                    else if ((rulesetsToExecute & RuleSets.Default) == RuleSets.Default)
                    {
                        return true;
                    }
                }
            }
            else
            {
                // checks if the intersection of rulesetsToExecute and ruleSets doesn't return nothing
                if ((rulesetsToExecute & ruleSets) != RuleSets.None)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
