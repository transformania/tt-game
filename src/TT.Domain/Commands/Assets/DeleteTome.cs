using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Assets;

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
