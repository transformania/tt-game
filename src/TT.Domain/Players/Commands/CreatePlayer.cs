using System;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.AI.Entities;
using TT.Domain.Covenants.Entities;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;
using TT.Domain.Identity.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Players.Commands
{
    public class CreatePlayer : DomainCommand<int>
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OriginalFirstName { get; set; }
        public string OriginalLastName { get; set; }
        public string UserId { get; set; }
        public string Location { get; set; }
        public decimal Health { get; set; }
        public decimal MaxHealth { get; set; }
        public decimal Mana { get; set; }
        public decimal MaxMana { get; set; }

        public decimal XP { get; set; }
        public int Level { get; set; }
        public int TimesAttackingThisUpdate { get; set; }

        public decimal ActionPoints { get; set; }
        public decimal ActionPoints_Refill { get; set; }

        public string Gender { get; set; } // TODO:  remove this as gender should be part of FormSource
        public string Mobility { get; set; } // TODO:  remove this as Mobility should be part of FormSource

        public int BotId { get; set; }
        public int NPCId { get; set; }

        public bool MindControlIsActive { get; set; }

        public string IpAddress { get; set; }

        public DateTime LastActionTimestamp { get; set; }
        public DateTime LastCombatTimestamp { get; set; }
        public DateTime LastCombatAttackedTimestamp { get; set; }

        public bool FlaggedForAbuse { get; set; }

        public int UnusedLevelUpPerks { get; set; }
        public int GameMode { get; set; }
        public bool InRP { get; set; }
        public bool InHardmode { get; set; }

        public int CleansesMeditatesThisRound { get; set; }
        public decimal Money { get; set; }
        public int? Covenant { get; set; }
        public decimal PvPScore { get; set; }
        public int DonatorLevel { get; set; }
        public string Nickname { get; set; }
        public DateTime OnlineActivityTimestamp { get; set; }
        public bool IsBannedFromGlobalChat { get; set; }
        public string ChatColor { get; set; }
        public int ShoutsRemaining { get; set; }
        public int InDuel { get; set; }
        public int InQuest { get; set; }
        public int InQuestState { get; set; }
        public int ItemsUsedThisTurn { get; set; }

        public int FormSourceId { get; set; }

        public CreatePlayer()
        {
            Health = 1000;
            MaxHealth = 1000;
            Mana = 210;
            MaxMana = 210;
            Level = 1;
            XP = 0;
            TimesAttackingThisUpdate = 0;
            ActionPoints = TurnTimesStatics.GetActionPointLimit();
            ActionPoints_Refill = TurnTimesStatics.GetActionPointReserveLimit();
            Gender = PvPStatics.GenderFemale;
            Mobility = PvPStatics.MobilityFull;
            LastActionTimestamp = DateTime.UtcNow.AddHours(-TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling());
            LastCombatTimestamp = DateTime.UtcNow.AddHours(-TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling());
            LastCombatAttackedTimestamp = DateTime.UtcNow.AddHours(-TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling());
            GameMode = 1;
            OnlineActivityTimestamp = DateTime.UtcNow;
            Money = 0;
            CleansesMeditatesThisRound = 0;
            ChatColor = "black";
        }

        public override int Execute(IDataContext context)
        {
            var result = 0;

            ContextQuery = ctx =>
            {
                var user = ctx.AsQueryable<User>()
                    .Include(u => u.Donator)
                    .SingleOrDefault(t => t.Id == UserId);

                if (user != null && user.Donator != null)
                    DonatorLevel = user.Donator.Tier;

                var npc = ctx.AsQueryable<NPC>().SingleOrDefault(t => t.Id == NPCId);

                var form = ctx.AsQueryable<FormSource>().SingleOrDefault(t => t.Id == FormSourceId);

                var covenant = Covenant == null ? null : ctx.AsQueryable<Covenant>().SingleOrDefault(t => t.Id == Covenant);

                var player = Player.Create(user, npc, form, this, covenant);

                ctx.Add(player);
                ctx.Commit();

                result = player.Id;
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
                throw new DomainException("First name is required");

            if (string.IsNullOrWhiteSpace(LastName))
                throw new DomainException("Last name is required");

            if (string.IsNullOrWhiteSpace(Location))
                throw new DomainException("Location is required");

            if (FormSourceId <= 0)
                throw new DomainException("FormSourceId is required");

            if (Health <= 0)
                throw new DomainException("Willpower must be greater than 0");

            if (MaxHealth <= 0)
                throw new DomainException("Maximum willpower must be greater than 0");

            if (Mana <= 0)
                throw new DomainException("Mana must be greater than 0");

            if (MaxMana <= 0)
                throw new DomainException("Maximum mana must be greater than 0");

            if (Level < 1)
                throw new DomainException("Level must be at least one.");

            if (TimesAttackingThisUpdate < 0)
                throw new DomainException("TimesAttackingThisUpdate must be at least 0");

            if (XP < 0)
                throw new DomainException("XP must be at least 0");

            if (ActionPoints < 0)
                throw new DomainException("ActionPoints must be at least 0");

            if (ActionPoints_Refill < 0)
                throw new DomainException("ActionPoints_Refill must be at least 0");

            if (ActionPoints < 0)
                throw new DomainException("ActionPoints must be at least 0");

            if (ActionPoints_Refill < 0)
                throw new DomainException("ActionPoints_Refill must be at least 0");

            if (ActionPoints > TurnTimesStatics.GetActionPointLimit())
                throw new DomainException("ActionPoints must be less than " + TurnTimesStatics.GetActionPointLimit());

            if (ActionPoints_Refill > TurnTimesStatics.GetActionPointReserveLimit())
                throw new DomainException("ActionPoints_Refill must be less than " + TurnTimesStatics.GetActionPointReserveLimit());

            if (Gender != PvPStatics.GenderFemale && Gender != PvPStatics.GenderMale)
                throw new DomainException("Gender must be either " + PvPStatics.GenderMale + " or " + PvPStatics.GenderFemale);
                
            if (Mobility != PvPStatics.MobilityFull &&
                Mobility != PvPStatics.MobilityInanimate &&
                Mobility != PvPStatics.MobilityPet)
                throw new DomainException("Mobility must be one of the following: " + PvPStatics.MobilityFull + ", " + PvPStatics.MobilityInanimate + ", or " + PvPStatics.MobilityPet);
        }

    }
}
