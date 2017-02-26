using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IAIDirectiveRepository
    {

        IQueryable<AIDirective> AIDirectives { get; }

        void SaveAIDirective(AIDirective AIDirective);

        void DeleteAIDirective(int AIDirectiveId);

    }
}