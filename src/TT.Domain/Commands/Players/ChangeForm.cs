using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Highway.Data;
using TT.Domain.Entities.Forms;
using TT.Domain.Entities.Items;
using TT.Domain.ViewModels;

namespace TT.Domain.Commands.Players
{
    public class ChangeForm : DomainCommand
    {

        public int playerId { get; set; }
        public int formId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(cr => cr.Id == playerId);
                if (player == null)
                    throw new DomainException(string.Format("Player with ID {0} could not be found", playerId));

                var form = ctx.AsQueryable<FormSource>().SingleOrDefault(cr => cr.Id == formId);
                if (form == null)
                    throw new DomainException(string.Format("FormSource with ID {0} could not be found", formId));

                player.ChangeForm(form);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

    }
}
