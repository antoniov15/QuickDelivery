using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuickDelivery.Api.Helpers;

namespace QuickDelivery.Api.Attributes
{
    public class ResourceAuthorizationAttribute : ActionFilterAttribute
    {
        private readonly string _resourceIdParameterName;

        public ResourceAuthorizationAttribute(string resourceIdParameterName = "id")
        {
            _resourceIdParameterName = resourceIdParameterName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as ControllerBase;
            if (controller == null) return;

            var currentUserRole = controller.GetCurrentUserRole();

            // Admin poate accesa orice
            if (currentUserRole == "Admin") return;

            // Verifică dacă parametrul există
            if (context.ActionArguments.TryGetValue(_resourceIdParameterName, out var resourceIdObj))
            {
                if (int.TryParse(resourceIdObj?.ToString(), out var resourceId))
                {
                    var currentUserId = controller.GetCurrentUserId();

                    // Verifică dacă utilizatorul poate accesa resursa
                    if (currentUserId != resourceId)
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
            }

            base.OnActionExecuting(context);
        }
    }
}