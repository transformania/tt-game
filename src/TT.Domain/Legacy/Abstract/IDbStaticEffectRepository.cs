using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IDbStaticEffectRepository
    {

        IQueryable<DbStaticEffect> DbStaticEffects { get; }

        void SaveDbStaticEffect(DbStaticEffect DbStaticEffect);

        void DeleteDbStaticEffect(int DbStaticEffectId);

    }
}