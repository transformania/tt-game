using System;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Queries;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;
using TT.Domain.World.Queries;

namespace TT.Domain.Procedures.BossProcedures
{
    public static class BossProcedures_HolidaySpirit
    {

        private const string BossFirstName = "Holiday";
        private const string BossLastName = "Spirit";
        public const int SpiritFormSourceId = 1693;
        public const int HolidayMimicFormSourceId = 1695;
        public const int GiftRespawnThreshold = 10;
        public const int GiftLimit = 50;
        public const int MoveTime = 15;
        public const int GiftRespawnTimer = 5;
        public const int MoveDistance = 20;
        public const string SpawnLocation = "300_west_9th_ave";
        public static int LastGiftDropTurn;

        public static void SpawnHolidaySpirit()
        {
            var holidaySpirit = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.HolidaySpiritBotId });

            if (holidaySpirit == null)
            {
                var cmd = new CreatePlayer
                {
                    FirstName = BossFirstName,
                    LastName = BossLastName,
                    Location = SpawnLocation,
                    Gender = PvPStatics.GenderFemale,
                    Health = 100000,
                    Mana = 100000,
                    MaxHealth = 100000,
                    MaxMana = 100000,
                    FormSourceId = SpiritFormSourceId,
                    Money = 2500,
                    Level = 5,
                    BotId = AIStatics.HolidaySpiritBotId,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                var spiritEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);

                spiritEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(spiritEF));

                playerRepo.SavePlayer(spiritEF);

                AIDirectiveProcedures.GetAIDirective(id);
                AIDirectiveProcedures.SetAIDirective_MoveTo(id, "300_west_9th_ave");
            }
        }

        public static void RunActions(int turnNumber)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var spiritBot = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.HolidaySpiritBotId && f.Mobility == PvPStatics.MobilityFull);

            if (spiritBot != null && spiritBot.Mobility == PvPStatics.MobilityFull)
            {

                var directive = AIDirectiveProcedures.GetAIDirective(spiritBot.Id);

                //Move the Spirit to any location on the overworld map
                //Alternate between streets and non-streets (variety!)
                if (directive.TargetLocation.IsEmpty())
                {
                    AIDirectiveProcedures.SetAIDirective_MoveTo(spiritBot.Id, "300_west_9th_ave");
                }

                if (directive.TargetLocation != spiritBot.dbLocationName)
                {
                    //Get her there fast so people don't have to chase her between too many turns
                    var newplace = AIProcedures.MoveTo(spiritBot, directive.TargetLocation, MoveDistance);
                    spiritBot.dbLocationName = newplace;
                }

                //Spirit arrived at destination. She waits a bit longer than other mobile NPCs so she can be talked to
                //Give her a new random location the opposite of where she is. On street? New = off street, and vice versa
                if (directive.TargetLocation == spiritBot.dbLocationName && turnNumber % MoveTime == 0)
                {
                    if (LocationsStatics.IsLocation_OnStreets(spiritBot.dbLocationName))
                    {
                        AIDirectiveProcedures.SetAIDirective_MoveTo(spiritBot.Id, LocationsStatics.GetRandomLocation_OnStreets());
                    }
                    else
                    {
                        AIDirectiveProcedures.SetAIDirective_MoveTo(spiritBot.Id, LocationsStatics.GetRandomLocation_NoStreets());
                    }
                }

                //Save and recover
                playerRepo.SavePlayer(spiritBot);
                var box = ItemProcedures.GetPlayerBuffs(spiritBot);

                if ((spiritBot.Health < spiritBot.MaxHealth))
                {
                    if (spiritBot.Health < spiritBot.MaxHealth)
                    {
                        DomainRegistry.Repository.Execute(new Cleanse { PlayerId = spiritBot.Id, Buffs = box, NoValidate = true });
                        DomainRegistry.Repository.Execute(new Cleanse { PlayerId = spiritBot.Id, Buffs = box, NoValidate = true });
                        DomainRegistry.Repository.Execute(new Meditate { PlayerId = spiritBot.Id, Buffs = box, NoValidate = true });
                    }
                }
                else
                {
                    if (spiritBot.Mana < spiritBot.MaxMana)
                    {
                        DomainRegistry.Repository.Execute(new Meditate { PlayerId = spiritBot.Id, Buffs = box, NoValidate = true });
                        DomainRegistry.Repository.Execute(new Meditate { PlayerId = spiritBot.Id, Buffs = box, NoValidate = true });
                        DomainRegistry.Repository.Execute(new Cleanse { PlayerId = spiritBot.Id, Buffs = box, NoValidate = true });
                    }
                }

                //Drop gifts
                DropPresent();

                return;
            }
        }

        private static void DropPresent()
        {
            //There is a cap of 50 presents on the map at once. They will not be limited by game mode
            //Check based on the number of gifts on the ground, not in existence. Don't want people to hoard them and stop them spawning
            //Gifts on the ground have a dbLocationName set. Use that for tracking
            //Respawn up to 50 when the total count is <=10 so they don't have to be fully cleared out
            var giftCmd = new GetAllItemsOfType { ItemSourceId = ItemStatics.GiftItemSourceId };
            var allGifts = DomainRegistry.Repository.Find(giftCmd);
            var giftsOnGround = allGifts.Where(g => g.dbLocationName != String.Empty);
            var world = DomainRegistry.Repository.FindSingle(new GetWorld());

            //Always check for the gift respawn threshold, only check turn number if not in Chaos Mode
            if ((giftsOnGround.Count() <= GiftRespawnThreshold && (world.TurnNumber > LastGiftDropTurn + GiftRespawnTimer || world.ChaosMode)))
            {
                for (var i = giftsOnGround.Count(); i < GiftLimit; i++)
                {
                    //Half go on the street, half go off the street
                    if (i % 2 == 0)
                    {
                        var cmd = new CreateItem
                        {
                            dbLocationName = LocationsStatics.GetRandomLocation_OnStreets(),
                            IsEquipped = false,
                            IsPermanent = false,
                            Level = 0,
                            PvPEnabled = -1,
                            OwnerId = null,
                            TurnsUntilUse = 0,
                            EquippedThisTurn = false,
                            ItemSourceId = ItemStatics.GetStaticItem(ItemStatics.GiftItemSourceId).Id
                        };

                        DomainRegistry.Repository.Execute(cmd);
                    }
                    else
                    {
                        var cmd = new CreateItem
                        {
                            dbLocationName = LocationsStatics.GetRandomLocation_NoStreets(),
                            IsEquipped = false,
                            IsPermanent = false,
                            Level = 0,
                            PvPEnabled = -1,
                            OwnerId = null,
                            TurnsUntilUse = 0,
                            EquippedThisTurn = false,
                            ItemSourceId = ItemStatics.GetStaticItem(ItemStatics.GiftItemSourceId).Id
                        };

                        DomainRegistry.Repository.Execute(cmd);
                    }
                }

                //Update the last turn gifts were dropped
                LastGiftDropTurn = world.TurnNumber;
            }
        }
    }
}