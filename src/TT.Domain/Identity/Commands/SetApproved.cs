﻿using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Commands
{
    public class SetApproved : DomainCommand
    {

        public string UserId { get; set; }
        public bool Approved { get; set; }

        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {
                var user = ctx.AsQueryable<User>().SingleOrDefault(t => t.Id == UserId);
                if (user == null)
                    throw new DomainException($"User with Id '{UserId}' could not be found");

                user.SetApprovedChanges(Approved);

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
