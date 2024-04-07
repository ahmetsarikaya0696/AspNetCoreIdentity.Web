using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreIdentity.Web.Requirements
{
    public class ExchangeExpireRequirement : IAuthorizationRequirement
    {
    }

    public class ExchangeExpireRequirementHandler : AuthorizationHandler<ExchangeExpireRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpireRequirement requirement)
        {
            bool hasExchangeExpireDateClaim = context.User.HasClaim(x => x.Type == "ExchangeExpireDate");

            if (!hasExchangeExpireDateClaim)
            {
                context.Fail();
                return Task.CompletedTask;
            }


            DateTime exchangeExpireDate = Convert.ToDateTime(context.User.Claims.First(x => x.Type == "ExchangeExpireDate").Value);
            if (DateTime.Now >= exchangeExpireDate)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
