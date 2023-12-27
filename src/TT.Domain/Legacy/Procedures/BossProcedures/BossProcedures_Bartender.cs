using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;

namespace TT.Domain.Procedures.BossProcedures
{
    public class BossProcedures_Bartender
    {

        private const int BartenderFormId = 403;

        public static void SpawnBartender()
        {
            var bartender = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.BartenderBotId });

            if (bartender == null)
            {
                var cmd = new CreatePlayer
                {
                    FirstName = "Rusty",
                    LastName = "Steamstein the Automaton Bartender",
                    Location = "tavern_counter",
                    Gender = PvPStatics.GenderMale,
                    Health = 100000,
                    Mana = 100000,
                    MaxHealth = 100000,
                    MaxMana = 100000,
                    FormSourceId = BartenderFormId,
                    Money = 0,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 15,
                    BotId = AIStatics.BartenderBotId,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                IPlayerRepository playerRepo = new EFPlayerRepository();
                var newBartender = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                newBartender.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(newBartender));
                playerRepo.SavePlayer(newBartender);
            }
        }
    }
}
