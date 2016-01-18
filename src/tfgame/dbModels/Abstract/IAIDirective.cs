using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IAIDirectiveRepository
    {

        IQueryable<AIDirective> AIDirectives { get; }

        void SaveAIDirective(AIDirective AIDirective);

        void DeleteAIDirective(int AIDirectiveId);

    }
}