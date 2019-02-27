using System;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;
using TT.Domain.Players.Entities;

namespace TT.Domain.Players.Commands
{
    public class ChangeForm : DomainCommand
    {

        public int PlayerId { get; set; }
        public int? FormId { get; set; }
        public int FormSourceId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>()
                    .Include(p => p.Item)
                    .Include(p => p.Item.Owner)
                    .SingleOrDefault(cr => cr.Id == PlayerId);

                if (player == null)
                    throw new DomainException($"Player with ID {PlayerId} could not be found");

                FormSource form;
                if (FormSourceId <= 0)
                {
                    if (FormId == null)
                        throw new DomainException("FormId or FormName are required");

                    form = ctx.AsQueryable<FormSource>().SingleOrDefault(cr => cr.Id == FormId);
                    if (form == null)
                        throw new DomainException($"FormSource with ID {FormId} could not be found");
                }
                else
                {
                    form = ctx.AsQueryable<FormSource>().SingleOrDefault(cr => cr.Id == FormSourceId);
                    if (form == null)
                        throw new DomainException($"FormSource with id '{FormSourceId}' could not be found");
                }
                player.ChangeForm(form);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

    }
}
