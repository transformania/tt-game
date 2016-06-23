using System;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Forms;
using TT.Domain.Procedures;

namespace TT.Domain.Commands.Players
{
    public class ChangeForm : DomainCommand
    {

        public int PlayerId { get; set; }
        public int? FormId { get; set; }
        public string FormName { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(cr => cr.Id == PlayerId);
                if (player == null)
                    throw new DomainException(string.Format("Player with ID {0} could not be found", PlayerId));

                FormSource form;
                if (FormName.IsNullOrEmpty())
                {
                    if (FormId == null)
                        throw new DomainException("FormId or FormName are required");

                    form = ctx.AsQueryable<FormSource>().SingleOrDefault(cr => cr.Id == FormId);
                    if (form == null)
                        throw new DomainException(string.Format("FormSource with ID {0} could not be found", FormId));
                }
                else
                {
                    form = ctx.AsQueryable<FormSource>().SingleOrDefault(cr => cr.dbName == FormName);
                    if (form == null)
                        throw new DomainException(string.Format("FormSource with name {0} could not be found", FormName));
                }
                player.ChangeForm(form);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

    }
}
