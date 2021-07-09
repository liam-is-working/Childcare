using System.Threading.Tasks;
using Childcare.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace Childcare.Authorization
{
    public class PatientIsOwnerAuthenticationHandller : AuthorizationHandler<OperationAuthorizationRequirement, Patient>
    {
       UserManager<ChildCareUser> _um;

       PatientIsOwnerAuthenticationHandller(UserManager<ChildCareUser> userManager){
          _um = userManager;
       }
       
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                            OperationAuthorizationRequirement requirement, Patient resource)
        {
            if(context.User == null || resource == null)
               return Task.CompletedTask;
            if (_um == null)
               throw new System.Exception("User manager is not available");

            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.DeleteOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName)
                return Task.CompletedTask;

            if(resource.Customer.ChildcareUserId == null)
               throw new System.Exception("Must pass a Patient with its Owner's userId included");

            if(resource.Customer.ChildcareUserId == _um.GetUserId(context.User) ){
               context.Succeed(requirement);
            }

            return Task.CompletedTask;           
            
        }
    }
}