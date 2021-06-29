using System.Threading.Tasks;
using Childcare.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Childcare.Authorization
{
    public class PatientStaffAuthenticationHandller : AuthorizationHandler<OperationAuthorizationRequirement, Patient>
    {
       
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                            OperationAuthorizationRequirement requirement, Patient resource)
        {
            if(context.User == null || resource == null)
               return Task.CompletedTask;

            //Read ony
            if (requirement.Name != Constants.ReadOperationName)
                return Task.CompletedTask;

            if(context.User.IsInRole("Staff"))
                context.Succeed(requirement);
                
            return Task.CompletedTask;           
            
        }
    }
}