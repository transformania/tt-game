using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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