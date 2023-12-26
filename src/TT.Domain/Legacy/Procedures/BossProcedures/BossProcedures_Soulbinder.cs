using System;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;

namespace TT.Domain.Procedures.BossProcedures
{
    public class BossProcedures_Soulbinder
    {

        private const int SoulbinderFormId = 1000;

        public static void SpawnSoulbinder()
        {
            var Soulbinder = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.SoulbinderBotId });

            if (Soulbinder == null)
            {
                var cmd = new CreatePlayer
                {
                    FirstName = "Karin",
                    LastName = "Kezesul-Adriz the Soulbinder",
                    FormSourceId = SoulbinderFormId,
                    Location = "stripclub_office",
                    Level = 10,
                    Mobility = PvPStatics.MobilityFull,
                    Money = 0,
                    Gender = PvPStatics.GenderFemale,
                    Health = 9999,
                    MaxHealth = 9999,
                    Mana = 9999,
                    MaxMana = 9999,
                    BotId = AIStatics.SoulbinderBotId
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                IPlayerRepository playerRepo = new EFPlayerRepository();
                var newSoulbinder = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                newSoulbinder.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(newSoulbinder));
                playerRepo.SavePlayer(newSoulbinder);
            }
        }
    }
}
