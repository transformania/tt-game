﻿using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;

namespace TT.Domain.Players.Commands
{
    public class ChangeBossEnableAfterDefeat : DomainCommand
    {

        public string MembershipId { get; set; }
        public bool BossEnableAfterDefeat { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>().SingleOrDefault(cr => cr.User.Id == MembershipId);
                if (player == null)
                    throw new DomainException($"Player with MembershipID '{MembershipId}' could not be found");

                player.ChangeBossEnableAfterDefeat(BossEnableAfterDefeat);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (MembershipId == null)
                throw new DomainException("MembershipID is required!");
        }

    }
}
