using FluentValidation;
using MediatR;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TT.Domain.Identity.CommandRequests;
using TT.Domain.Identity.PropertyValidators;
using TT.Domain.Statics;
using TT.Web.Attributes;

namespace TT.Web.Controllers.API.Admin
{
    [Authorize]
    public class AdminSignOutController : ApiController
    {
        private readonly IMediator mediator;

        public AdminSignOutController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        // POST api/AdminSignOut
        [HttpPost]
        [ValidateWebApiAntiForgeryToken]
        [System.Web.Mvc.Authorize(Roles = PvPStatics.Permissions_Moderator)]
        public async Task<HttpResponseMessage> Post(ResetSecurityStamp id)
        {
            try
            {
                await mediator.Send(new ResetSecurityStamp() { TargetUserNameId = id.TargetUserNameId });
            }
            catch (ValidationException ex)
            when (ex.Errors.FirstOrDefault()?.ErrorCode == nameof(UserHasCorrectValidator))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Error = ex.Errors.FirstOrDefault().ErrorMessage });
            }
            catch (ValidationException ex)
            when (ex.Errors.FirstOrDefault()?.ErrorCode == nameof(IsValidUserValidator))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Error = ex.Errors.FirstOrDefault().ErrorMessage });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { Result = "Successfully signed out user." });
        }
    }
}