using System;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;
using TT.Domain.Items.Queries;

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

        public static void RunSoulbinderActions(int turnNumber)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var soulbinder = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.SoulbinderBotId);


            if (turnNumber % 12 == 0)
            {
                // Get a list of all items held that are former players.
                var soulbinderItems = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = soulbinder.Id }).Where(i => i.FormerPlayer != null).ToList();

                var rand = new Random();
                int chance = 0;

                foreach (var p in soulbinderItems)
                {
                    var player = PlayerProcedures.GetPlayer(p.FormerPlayer.Id);
                    var item = ItemProcedures.GetItemViewModel(p.Id);

                    if (!player.MembershipId.IsNullOrEmpty())
                    {
                        // Roll the dice.
                        chance = rand.Next(0, 100);

                        // About 1/4 of a chance every 12 turns to be interacted with.
                        if (chance <= 24)
                        {
                            // Depending on the roll determines the interaction.
                            if (p.ItemSource.ItemType == PvPStatics.ItemType_Pet)
                            {
                                if (chance <= 8) // Praise
                                {
                                    PlayerLogProcedures.AddPlayerLog(player.Id, "Karin briefly praises you, confident your owner will return soon!", true);
                                }
                                else if (chance <= 16) // Scold
                                {
                                    PlayerLogProcedures.AddPlayerLog(player.Id, "Karin sighs as she scolds you for misbehaving.", true);
                                }
                                else // Restrain
                                {
                                    PlayerLogProcedures.AddPlayerLog(player.Id, "Karin restrains you, making sure you can't run off.", true);
                                }
                            }
                            else
                            {
                                if (chance <= 8) // Flaunt
                                {
                                    PlayerLogProcedures.AddPlayerLog(player.Id, "Karin briefly flaunts you, confident your owner will return soon!", true);
                                }
                                else if (chance <= 16) // Shun
                                {
                                    PlayerLogProcedures.AddPlayerLog(player.Id, "Karin sighs and seems to shun you, disappointed that your owner might have dumped you on her.", true);
                                }
                                else if (!item.Item.UsageMessage_Item.IsNullOrEmpty())
                                {
                                    var itemMessage = item.Item.UsageMessage_Item;
                                    var context = "Karin just used you!";
                                    itemMessage = string.IsNullOrEmpty(itemMessage) ? context : $"{itemMessage}<br />{context}";
                                    PlayerLogProcedures.AddPlayerLog(player.Id, itemMessage, true);
                                }
                                else if (p.ItemSource.Id == ItemStatics.ButtPlugItemSourceId) // A buttplug? OMG... *blushes*
                                {
                                    var itemMessage = item.Item.UsageMessage_Item;
                                    var context = $"Karin just used you to help remove a curse from a stanger! Doesn't that make you feel all warm and tingly?";
                                    itemMessage = string.IsNullOrEmpty(itemMessage) ? context : $"{itemMessage}<br />{context}";
                                    PlayerLogProcedures.AddPlayerLog(player.Id, itemMessage, true);
                                }
                                else // Hush
                                {
                                    PlayerLogProcedures.AddPlayerLog(player.Id, "Karin hushes you, seemingly tired of still carrying you.", true);
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
