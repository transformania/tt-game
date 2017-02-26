using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Commands
{
    public class SetArtistBioVisibility : DomainCommand<string>
    {
        public string UserId { get; set; }
        public bool IsVisible { get; set; }

        public override string Execute(IDataContext context)
        {

            var output = "";

            ContextQuery = ctx =>
            {

                var bio = ctx.AsQueryable<ArtistBio>().Include(b => b.Owner).SingleOrDefault(t => t.Owner.Id == UserId);

                if (bio == null)
                    throw new DomainException($"Artist bio for user '{UserId}' could not be found");

                bio.SetLiveStatus(IsVisible);

                ctx.Update(bio);
                ctx.Commit();

                output = $"You set your artist bio visibility to {IsVisible}.";

            };

            ExecuteInternal(context);
            return output;

        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(UserId))
                throw new DomainException("userId is required");
        }
    }
}
