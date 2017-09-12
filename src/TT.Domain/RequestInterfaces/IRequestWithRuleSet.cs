using TT.Domain.Validation;

namespace TT.Domain.RequestInterfaces
{
    public interface IRequestWithRuleSet
    {
        RuleSets RuleSets { get; }
    }
}
