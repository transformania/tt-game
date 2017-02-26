using System.Linq;
using Highway.Data;
using TT.Domain.Statics;

namespace TT.Domain.Players.Queries
{
    public class PlayerIsAtBusStop : DomainQuerySingle<bool>
    {
        public string playerLocation { get; set; }

        public override bool Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                if (LocationsStatics.BusStops.Contains(playerLocation))
                {
                    return true;
                }

                return false;
            };

            return ExecuteInternal(context);
        }

    }
}
