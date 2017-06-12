using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace WebClient
{
    // A handler that can determine whether a MaximumOfficeNumberRequirement is satisfied
    internal class MaximumOfficeNumberAuthorizationHandler : AuthorizationHandler<MaximumOfficeNumberRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MaximumOfficeNumberRequirement requirement)
        {
            // Bail out if the office number claim isn't present
            if (!context.User.HasClaim(c => c.Issuer == "http://localhost:5000" && c.Type == "office"))
            {
                return Task.CompletedTask;
            }

            // Bail out if we can't read an int from the 'office' claim
            int officeNumber;
            if (!int.TryParse(context.User.FindFirst(c => c.Issuer == "http://localhost:5000" && c.Type == "office").Value, out officeNumber))
            {
                return Task.CompletedTask;
            }

            // Finally, validate that the office number from the claim is not greater
            // than the requirement's maximum
            if (officeNumber <= requirement.MaximumOfficeNumber)
            {
                // Mark the requirement as satisfied
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    // A custom authorization requirement which requires office number to be below a certain value
    internal class MaximumOfficeNumberRequirement : IAuthorizationRequirement
    {
        public MaximumOfficeNumberRequirement(int officeNumber)
        {
            MaximumOfficeNumber = officeNumber;
        }

        public int MaximumOfficeNumber { get; private set; }
    }
}
