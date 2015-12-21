using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

namespace tfgame.Procedures
{
    public static class AIDirectiveProcedures
    {
        public static AIDirective GetAIDirective(int botId)
        {
            IAIDirectiveRepository directiveRepo = new EFAIDirectiveRepository();

            AIDirective directive = directiveRepo.AIDirectives.FirstOrDefault(ad => ad.OwnerId == botId);

            if (directive == null)
            {
                directive = new AIDirective
                {
                    OwnerId = botId,
                    State = "idle", // attack, move, flee
                    TargetLocation = "",
                    TargetPlayerId = -1,
                    Timestamp = DateTime.UtcNow,
                    Var1 = -1,
                    Var2 = -1,
                    Var3 = -1,
                    Var4 = -1,
                    Var5 = -1
                };
                directiveRepo.SaveAIDirective(directive);
            }

            return directive;

        }

        public static void SetAIDirective_MoveTo(int botId, string dblocationName)
        {
            IAIDirectiveRepository directiveRepo = new EFAIDirectiveRepository();
            AIDirective directive = directiveRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == botId);
            //directive.State = "move";
            directive.Timestamp = DateTime.UtcNow;
            directive.TargetLocation = dblocationName;
            directiveRepo.SaveAIDirective(directive); 
        }

        public static void SetAIDirective_Attack(int botId, int targetId)
        {
            IAIDirectiveRepository directiveRepo = new EFAIDirectiveRepository();
            AIDirective directive = directiveRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == botId);
            directive.State = "attack";
            directive.Timestamp = DateTime.UtcNow;
            directive.TargetPlayerId = targetId;
            directiveRepo.SaveAIDirective(directive);
        }

        public static void SetAIDirective_Idle(int botId)
        {
            IAIDirectiveRepository directiveRepo = new EFAIDirectiveRepository();
            AIDirective directive = directiveRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == botId);
            directive.State = "idle";
            directive.TargetPlayerId = 0;
            directive.Timestamp = DateTime.UtcNow;
            directiveRepo.SaveAIDirective(directive);
        }

        public static void DeleteAIDirectiveByPlayerId(int ownerId)
        {
             IAIDirectiveRepository directiveRepo = new EFAIDirectiveRepository();
             AIDirective directive = directiveRepo.AIDirectives.FirstOrDefault(d => d.OwnerId == ownerId);
             if (directive != null)
             {
                 directiveRepo.DeleteAIDirective(directive.Id);
             }
             
        }

        public static void DeaggroPsychopathsOnPlayer(Player player)
        {
            IAIDirectiveRepository directiveRepo = new EFAIDirectiveRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            List<AIDirective> directives = directiveRepo.AIDirectives.Where(d => d.TargetPlayerId == player.Id).ToList();

            foreach (AIDirective d in directives)
            {
                Player p = playerRepo.Players.FirstOrDefault(x => x.Id == d.OwnerId);
                if (p.FirstName.Contains("Psychopath")) {
                    d.TargetPlayerId = -1;
                    d.State = "idle";
                    directiveRepo.SaveAIDirective(d);
                }
            }

        }



    }
}