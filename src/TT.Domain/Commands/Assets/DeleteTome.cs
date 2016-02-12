using System.Linq;
using System.Text.RegularExpressions;
using Highway.Data;
using TT.Domain.Entities.Assets;
using TT.Domain.Entities.Items;
using TT.Domain.Entities.Identity;
using TT.Domain.DTOs;

namespace TT.Domain.Commands.Assets
{
    public class DeleteTome : Highway.Data.Command
    {

        public int Id { get; set; }

        public DeleteTome(int Id)
        {
            this.Id = Id;
        }

        public override void Execute(IDataContext context)
        {

            Validate();

            ContextQuery = ctx =>
            {

                var deleteMe = ctx.AsQueryable<Tome>().First(cr => cr.Id == this.Id);
                ctx.Remove(deleteMe);

                ctx.Commit();

            };

            base.Execute(context);

        }

        private void Validate()
        {
            
        }
    }
}
