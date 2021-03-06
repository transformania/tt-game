﻿using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Commands;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;

namespace TT.Domain.Procedures.BossProcedures
{
    public static class BossProcedures_Loremaster
    {

        public const string FirstName = "Skaldrlyr";
        public const string LastName = "the Forbidden";

        private const int LoremasterFormId = 467;

        public static void SpawnLoremaster()
        {
            var loremaster = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.LoremasterBotId });

            if (loremaster == null)
            {
                var cmd = new CreatePlayer
                {
                    BotId = AIStatics.LoremasterBotId,
                    Level = 5,
                    FirstName = FirstName,
                    LastName = LastName,
                    Health = 100000,
                    Mana = 100000,
                    MaxHealth = 100000,
                    MaxMana = 100000,
                    Mobility = PvPStatics.MobilityFull,
                    Money = 1000,
                    FormSourceId = LoremasterFormId,
                    Location = "bookstore_back",
                    Gender = PvPStatics.GenderMale,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                var lorekeeperEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                lorekeeperEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(lorekeeperEF));
                playerRepo.SavePlayer(lorekeeperEF);
            }
        }


        public static void TransferBooksFromLindellaToLorekeeper(Player lorekeeper)
        {
            var lindella = PlayerProcedures.GetPlayerFromBotId(AIStatics.LindellaBotId);

            var allLindellaItems = ItemProcedures.GetAllPlayerItems(lindella.Id).Where(i =>
                i.Item.ConsumableSubItemType == (int) ItemStatics.ConsumableSubItemTypes.Spellbook ||
                i.Item.ConsumableSubItemType == (int) ItemStatics.ConsumableSubItemTypes.Tome);

            foreach (var i in allLindellaItems)
            {
                var cmd = new ChangeItemOwner {ItemId = i.dbItem.Id, OwnerId = lorekeeper.Id};
                DomainRegistry.Repository.Execute(cmd);
            }
        }

    }
}