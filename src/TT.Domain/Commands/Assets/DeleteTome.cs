using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Assets;

namespace TT.Domain.Commands.Assets
{
    public class DeleteTome : DomainCommand
    {
        public int Id { get; set; }

        public DeleteTome(int Id)
        {
            this.Id = Id;
        }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var deleteMe = ctx.AsQueryable<Tome>().First(cr => cr.Id == Id);
                ctx.Remove(deleteMe);

                ctx.Commit();
            };

            base.Execute(context);
        }
    }
}
