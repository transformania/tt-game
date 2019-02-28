using System;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;

namespace TT.Domain.Items.Commands
{
    public class SetSoulbindingConsent : DomainCommand<string>
    {
        public int PlayerId { get; set; }
        public bool IsConsenting { get; set; }

        public override string Execute(IDataContext context)
        {

            var output = String.Empty;

            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>()
                    .Include(p => p.Item)
                .SingleOrDefault(p => p.Id == PlayerId);

                if (player == null)
                    throw new DomainException($"Player with ID '{PlayerId}' not found.");

                if (player.Item == null)
                    throw new DomainException("You are not inanimate or a pet.");

                player.Item.SetSoulbindingConsent(IsConsenting);

                ctx.Commit();

                output = $"You have set your soulbinding consent to <b>{IsConsenting}</b>.";
            };

            ExecuteInternal(context);

            return output;
        }
    }
}
