using TT.Domain.Players.Commands;
using TT.Domain.Players.DTOs;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;

namespace TT.Domain.Procedures.BossProcedures
{
    public class BossProcedures_Bartender
    {

        private const int BartenderFormId = 403;

        public static void SpawnBartender()
        {
            PlayerDetail bartender = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.DonnaBotId });

            if (bartender == null)
            {
                var cmd = new CreatePlayer
                {
                    FirstName = "Rusty",
                    LastName = "Steamstein the Automaton Bartender",
                    Location = "tavern_counter",
                    Gender = PvPStatics.GenderMale,
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = "form_The_Perfect_Barman_Judoo",
                    FormSourceId = BartenderFormId,
                    Money = 0,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 15,
                    BotId = AIStatics.BartenderBotId,
                };
                DomainRegistry.Repository.Execute(cmd);
            }
        }
    }
}
