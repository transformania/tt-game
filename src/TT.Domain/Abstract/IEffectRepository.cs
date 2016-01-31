using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IEffectRepository
    {

        IQueryable<Effect> Effects { get; }

        IQueryable<DbStaticEffect> DbStaticEffects { get; }

        void SaveEffect(Effect Effect);

        void DeleteEffect(int EffectId);

        void SaveDbStaticEffect(DbStaticEffect effect);

        void DeleteDbStaticEffect(int EffectId);

    }
}