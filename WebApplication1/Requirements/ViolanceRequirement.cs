using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreIdentity.Web.Requirements
{
    public class ViolanceRequirement : IAuthorizationRequirement
    {
        public int ThresholdAge { get; set; }
    }

    public class ViolanceRequirementHandler : AuthorizationHandler<ViolanceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolanceRequirement requirement)
        {
            bool hasBirthdayClaim = context.User.HasClaim(x => x.Type == "Birthday");

            if (!hasBirthdayClaim)
            {
                context.Fail();
                return Task.CompletedTask;
            }


            DateTime birthday = Convert.ToDateTime(context.User.Claims.First(x => x.Type == "Birthday").Value);
            int age = DateTime.Now.Year - birthday.Year;

            // Yaşını tam doldurmadıysa yaşı 1 azalt
            if (DateTime.Now.Month < birthday.Month || (DateTime.Now.Month == birthday.Month && DateTime.Now.Day < birthday.Day))
                age--;

            if (requirement.ThresholdAge > age)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
