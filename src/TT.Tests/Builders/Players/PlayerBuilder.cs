using System;
using System.Collections.Generic;
using TT.Domain.Entities.TFEnergies;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Domain.TFEnergies.Entities;

namespace TT.Tests.Builders.Players
{
    public class PlayerBuilder : Builder<Player, int>
    {
        public PlayerBuilder()
        {
            Instance = Create();
            With(u => u.Id, 3);
            With(u => u.Form, "some form");
            With(u => u.Mobility, PvPStatics.MobilityFull);
            With(u => u.Location, LocationsStatics.STREET_70_EAST_9TH_AVE);
            With(p => p.ActionPoints, PvPStatics.MaximumStoreableActionPoints);
            With(p => p.ActionPoints_Refill, PvPStatics.MaximumStoreableActionPoints_Refill);
            With(p => p.FirstName, "John");
            With(p => p.LastName, "Doe" );
            With(p => p.LastActionTimestamp, DateTime.UtcNow.AddMinutes(-10) );
            With(p => p.LastCombatAttackedTimestamp, DateTime.UtcNow.AddHours(-1) );
            With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddHours(-1));
            With(p => p.BotId, AIStatics.ActivePlayerBotId );
            With(p => p.Health, 100 );
            With(p => p.MaxHealth, 100 );
            With(p => p.Mana, 100);
            With(p => p.MaxMana, 100);
            With(p => p.GameMode, (int)GameModeStatics.GameModes.Protection);
            With(p => p.XP, 0);
            With(p => p.Level, 1);
            With(p => p.Gender, PvPStatics.GenderMale);
            With(p => p.OnlineActivityTimestamp, DateTime.Now);
            With(p => p.TFEnergies, new List<TFEnergy>());
            With(p => p.TFEnergiesCast, new List<TFEnergy>());
            With(p => p.PlayerLogs, new List<PlayerLog>());
            With(p => p.Items, new List<TT.Domain.Items.Entities.Item>());
            With(p => p.ShoutsRemaining, 1);
        }
    }
}
