using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;

namespace TT.Domain.Statics
{
    public static class EffectStatics
    {
        
        // Psycho buffs
        public static int PSYCHO_LVL_1_EFFECT = 19;
        public static int PSYCHO_LVL_3_EFFECT = 20;
        public static int PSYCHO_LVL_5_EFFECT = 21;
        public static int PSYCHO_LVL_7_EFFECT = 22;
        public static int PSYCHO_LVL_9_EFFECT = 23;

        // Harley buffs
        public static int MOTORCYCLE_1_EFFECT = 187;
        public static int MOTORCYCLE_2_EFFECT = 188;
        public static int MOTORCYCLE_3_EFFECT = 189;
        public static int MOTORCYCLE_4_EFFECT = 190;
        public static int MOTORCYCLE_5_EFFECT = 191;

        // Joke shop buffs.
        public const int JOKESHOP_DISCIPLINE_EFFECT = 229;
        public const int JOKESHOP_PERCEPTION_EFFECT = 230;
        public const int JOKESHOP_CHARISMA_EFFECT = 231;
        public const int JOKESHOP_FORTITUDE_EFFECT = 232;
        public const int JOKESHOP_AGILITY_EFFECT = 233;
        public const int JOKESHOP_RESTORATION_EFFECT = 234;
        public const int JOKESHOP_MAGICKA_EFFECT = 235;
        public const int JOKESHOP_REGENERATION_EFFECT = 236;
        public const int JOKESHOP_LUCK_EFFECT = 237;
        public const int JOKESHOP_INVENTORY_EFFECT = 238;
        public const int JOKESHOP_MOBILITY_EFFECT = 239;

        // Super Psycho. No, I don't think you'll win.
        public static int[] SUPER_PSYCHO_EFFECT = { 19, 20, 21, 22, 23 };

        // This is just way too OP.
        public static int[] OP_EFFECT = { 187, 188, 189, 190, 191 };

        // I'm so fresh, you can suck my nuts.
        public static int[] FRESH_EFFECT = { 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239 };

        // Oh shit, what are you doing?
        public static int[] SUPER_OP_EFFECT = SUPER_PSYCHO_EFFECT.Concat(FRESH_EFFECT).Concat(OP_EFFECT).ToArray();

        public static IEnumerable<DbStaticEffect> GetEffectGainedAtLocation(string location)
        {
            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
            return effectRepo.DbStaticEffects.Where(e => e.ObtainedAtLocation == location);
        }

        public static IEnumerable<DbStaticEffect> GetAllStaticEffects()
        {
            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
            return effectRepo.DbStaticEffects;
        }

        public static DbStaticEffect GetDbStaticEffect(int effectSourceId)
        {
            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
            return effectRepo.DbStaticEffects.FirstOrDefault(s => s.Id == effectSourceId);
        }

    }

}

