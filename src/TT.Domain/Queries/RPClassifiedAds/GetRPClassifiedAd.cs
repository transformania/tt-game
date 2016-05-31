using Highway.Data;
using TT.Domain.DTOs.RPClassifiedAds;
using TT.Domain.Entities.RPClassifiedAds;
using System.Linq;
using TT.Domain.Exceptions.RPClassifiedAds;
using AutoMapper.QueryableExtensions;
using TT.Domain.Entities.Players;
using DelegateDecompiler;
using AutoMapper;

namespace TT.Domain.Queries.RPClassifiedAds
{
    public class GetRPClassifiedAd : DomainQuerySingle<RPClassifiedAdDetail>
    {
        public int RPClassifiedAdId { get; set; }

        /// <summary>
        /// Gets or sets <see cref="UserId"/>.
        /// <para>If set this query checks if the ad with the id of <see cref="RPClassifiedAdId"/>
        /// belongs to the user with the id of <see cref="UserId"/></para>
        /// </summary>
        public string UserId { get; set; }

        public override RPClassifiedAdDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var query = from q in ctx.AsQueryable<RPClassifiedAd>()
                          where q.Id == RPClassifiedAdId
                          select q;

                var ad = query.ProjectToFirstOrDefault<RPClassifiedAdDetail>();

                if (ad == null)
                    throw new RPClassifiedAdException(string.Format("RPClassifiedAd with ID {0} could not be found", RPClassifiedAdId))
                    {
                        UserFriendlyError = "This RP Classified Ad doesn't exist.",
                        ExType = RPClassifiedAdException.ExceptionType.NoAdfound
                    };

                if (UserId != null && ad.OwnerMembershipId != UserId)
                {
                    throw new RPClassifiedAdException(string.Format("User {0} does not own RP Classified Ad Id {1}", UserId, RPClassifiedAdId))
                    {
                        UserFriendlyError = "You do not own this RP Classified Ad.",
                        ExType = RPClassifiedAdException.ExceptionType.NotOwner
                    };
                }

                return ad;
            };

            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (RPClassifiedAdId < 0)
                throw new RPClassifiedAdException(string.Format("The ID {0} can not be negative", RPClassifiedAdId))
                 {
                    ExType = RPClassifiedAdException.ExceptionType.InvalidInput
                };
        }
    }
}
