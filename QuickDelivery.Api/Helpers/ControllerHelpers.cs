using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace QuickDelivery.Api.Helpers
{
    public static class ControllerHelpers
    {
        public static int GetCurrentUserId(this ControllerBase controller)
        {
            var userIdClaim = controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        public static string GetCurrentUserRole(this ControllerBase controller)
        {
            return controller.User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        public static int? GetCurrentUserPartnerId(this ControllerBase controller)
        {
            // Implementează logic pentru a obține Partner ID
            var partnerIdClaim = controller.User.FindFirst("PartnerId")?.Value;
            return int.TryParse(partnerIdClaim, out var partnerId) ? partnerId : null;
        }

        public static bool IsCurrentUserAuthorizedForResource(this ControllerBase controller, int resourceOwnerId)
        {
            var currentUserId = controller.GetCurrentUserId();
            var currentUserRole = controller.GetCurrentUserRole();

            return currentUserRole == "Admin" || currentUserId == resourceOwnerId;
        }
    }
}