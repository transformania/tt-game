﻿using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;

namespace TT.Domain.Players.Commands
{
    public class UpdateLastHolidaySpiritInteraction : DomainCommand
    {

        public string UserId { get; set; }
        public int LastHolidaySpiritInteraction { get; set; }

        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {
                var user = ctx.AsQueryable<Player>().SingleOrDefault(t => t.User.Id == UserId);
                if (user == null)
                    throw new DomainException($"User with Id '{UserId}' could not be found");

                user.ChangeLastHolidaySpiritInteraction(LastHolidaySpiritInteraction);

                ctx.Update(user);
                ctx.Commit();

            };

            ExecuteInternal(context);

        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(UserId))
                throw new DomainException("UserId is required");
        }
    }
}
