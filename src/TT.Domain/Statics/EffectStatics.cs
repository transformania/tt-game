using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;

namespace TT.Domain.Statics
{
    public static class EffectStatics
    {
        public static DbStaticEffect GetStaticEffect2(string dbEffectName)
        {
            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
            return effectRepo.DbStaticEffects.FirstOrDefault(s => s.dbName == dbEffectName);
        }

        public static IEnumerable<DbStaticEffect> GetEffectGainedAtLocation(string location)
        {
            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
            return effectRepo.DbStaticEffects.Where(e => e.ObtainedAtLocation == location);
        }

        public static IEnumerable<DbStaticEffect> GetAllStaticEffects()
        {
            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
            return effectRepo.DbStaticEffects.Where(e => e.dbName != null && e.dbName != "");
        }

        public static DbStaticEffect GetEffect(string dbFormName)
        {
            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
            return effectRepo.DbStaticEffects.FirstOrDefault(s => s.dbName == dbFormName);
        }

    }

}

