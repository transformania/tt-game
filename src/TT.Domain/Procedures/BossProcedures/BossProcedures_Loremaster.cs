using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Commands.Items;
using TT.Domain.Commands.Players;
using TT.Domain.Concrete;
using TT.Domain.DTOs.Players;
using TT.Domain.Models;
using TT.Domain.Queries.Players;
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
            PlayerDetail loremaster = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.LoremasterBotId });

            if (loremaster == null)
            {
                var cmd = new CreatePlayer
                {
                    BotId = AIStatics.LoremasterBotId,
                    Level = 5,
                    FirstName = FirstName,
                    LastName = LastName,
                    Health = 5000,
                    Mana = 5000,
                    MaxHealth = 500,
                    MaxMana = 500,
                    Mobility = PvPStatics.MobilityFull,
                    Money = 1000,
                    Form = FormDbName,
                    Location = "bookstore_back",
                    Gender = PvPStatics.GenderMale,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                Player lorekeeperEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                lorekeeperEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(lorekeeperEF));
                playerRepo.SavePlayer(lorekeeperEF);
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