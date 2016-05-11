using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Commands.Items;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Statics;

namespace TT.Domain.Procedures.BossProcedures
{
    public static class BossProcedures_Loremaster
    {

        public const string FirstName = "Skaldrlyr";
        public const string LastName = "the Forbidden";
        public const string FormDbName = "form_Exiled_Lorekeeper_Judoo";

        public static void SpawnLoremaster()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player loremaster = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.LoremasterBotId);

            if (loremaster == null)
            {
                loremaster = new Player();
                loremaster.BotId = AIStatics.LoremasterBotId;
                loremaster.Level = 5;
                loremaster.FirstName = FirstName;
                loremaster.LastName = LastName;
                loremaster.Health = 5000;
                loremaster.Mana = 5000;
                loremaster.MaxHealth = 500;
                loremaster.MaxMana = 500;
                loremaster.Mobility = PvPStatics.MobilityFull;
                loremaster.Money = 1000;
                loremaster.TimesAttackingThisUpdate = 0;
                loremaster.UnusedLevelUpPerks = 0;
                loremaster.LastActionTimestamp = DateTime.UtcNow;
                loremaster.LastCombatTimestamp = DateTime.UtcNow;
                loremaster.LastCombatAttackedTimestamp = DateTime.UtcNow;
                loremaster.OnlineActivityTimestamp = DateTime.UtcNow;
                loremaster.Form = FormDbName;
                loremaster.dbLocationName = "bookstore_back";
                loremaster.Gender = PvPStatics.GenderMale;
                loremaster.ActionPoints = 120;

                playerRepo.SavePlayer(loremaster);

            }
        }


        public static void TransferBooksFromLindellaToLorekeeper(Player lorekeeper)
        {
            Player lindella = PlayerProcedures.GetPlayerFromBotId(AIStatics.LindellaBotId);
            IItemRepository itemRepo = new EFItemRepository();

            List<Item> LindellasBooks = itemRepo.Items.Where(i => i.OwnerId == lindella.Id && (i.dbName.Contains("item_consumable_spellbook_") || i.dbName.Contains("item_consumable_tome-"))).ToList();

            foreach (Item i in LindellasBooks)
            {
                var cmd = new ChangeItemOwner {ItemId = i.Id, OwnerId = lorekeeper.Id};
                DomainRegistry.Repository.Execute(cmd);
            }


        }

    }
}