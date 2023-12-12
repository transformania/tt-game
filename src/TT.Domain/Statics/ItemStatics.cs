using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;

namespace TT.Domain.Statics
{
    public static class ItemStatics
    {
        public const int HotCoffeeMugItemSourceId = 153;

        public const int AutoTransmogItemSourceId = 171;
        public const int CurseLifterItemSourceId = 184;
        public const int ButtPlugItemSourceId = 192;

        public const int WillpowerBombWeakItemSourceId = 7;
        public const int WillpowerBombStrongItemSourceId = 8;
        public const int WillpowerBombVolatileItemSourceId = 9;

        public const int SelfRestoreItemSourceId = 129;
        public const int LullabyWhistleItemSourceId = 160;
        public const int CovenantCrystalItemSourceId = 164;

        public const int SpellWeaverDryItemSourceId = 3;
        public const int SpellWeaverFreshItemSourceId = 4;
        public const int SpellWeaverRootItemSourceId = 5;

        public const int WillflowerDryItemSourceId = 10;
        public const int WillflowerFreshItemSourceId = 1;
        public const int WillflowerRootItemSourceId = 2;

        public const int ConcealmentCookieSourceId = 123;
        public const int FireFritterSourceId = 130;
        public const int BarricadeBrownieSourceId = 133;
        public const int TrueshotTrufflesSourceId = 157;
        public const int NirvanaNuggetSourceId = 158;
        public const int PerceptionPuffSourceId = 257;
        public const int LuckyLemoncakeSourceId = 258;
        public const int DanishOfDiscoverySourceId = 585;

        public const int SpellbookSmallItemSourceId = 180;
        public const int SpellbookMediumItemSourceId = 181;
        public const int SpellbookLargeItemSourceId = 182;
        public const int SpellbookGiantItemSourceId = 183;

        public const int InflatableDollItemSourceId = 30;
        public const int TeleportationScrollItemSourceId = 6;

        public const int TgSplashOrbItemSourceId = 390;

        public const int ItemType_DungeonArtifactItemSourceId = 218;
        
        public const int MallKeySourceId = 585;

        //Defective items!
        public const int DefectiveTransmogItemSourceId = 737;
        public const int OtherRestoreItemSourceId = 774;
        public const int TomeRewards = 820;

        //Holiday Spirit gift
        public const int GiftItemSourceId = 816;

        public enum ConsumableSubItemTypes { Rune = 0, Tome = 1, Spellbook = 2, Restorative = 3, WillpowerBomb = 4 }

        public static DbStaticItem GetStaticItem(int itemSourceId)
        {
            IItemRepository itemRepo = new EFItemRepository();
            return itemRepo.DbStaticItems.FirstOrDefault(i => i.Id == itemSourceId);
        }

        public static IEnumerable<DbStaticItem> GetAllFindableItems()
        {
            IItemRepository itemRepo = new EFItemRepository();
            return itemRepo.DbStaticItems.Where(i => i.Findable);
        }

        public static IEnumerable<DbStaticItem> GetAllNonPetItems()
        {
            IItemRepository itemRepo = new EFItemRepository();
            return itemRepo.DbStaticItems.Where(i => i.ItemType != PvPStatics.ItemType_Pet);
        }

        public static IEnumerable<DbStaticItem> GetAllPetItems()
        {
            IItemRepository itemRepo = new EFItemRepository();
            return itemRepo.DbStaticItems.Where(i => i.ItemType == PvPStatics.ItemType_Pet);
        }

        private static readonly Dictionary<string, double> SellValueModifiers = new Dictionary<string, double>
        {
            {PvPStatics.ItemType_Consumable, .4},
            {PvPStatics.ItemType_Rune, .2}
        };

        public static double GetSellValueModifier(string itemType)
        {
            SellValueModifiers.TryGetValue(itemType, out var result);
            return result == 0 ? .75 : result;
        }
    }
}

