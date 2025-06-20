using System.Security.Claims;
using QuickDelivery.Core.DTOs.Orders;

namespace QuickDelivery.Core.Extensions
{
    public static class AuthorizationExtensions
    {
        public static bool CanAccessUserData(this ClaimsPrincipal user, int targetUserId)
        {
            var currentUserId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            return role == "Admin" || currentUserId == targetUserId;
        }

        public static bool CanAccessOrderData(this ClaimsPrincipal user, OrderDto order)
        {
            var currentUserId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            return role == "Admin" ||
                   role == "Manager" ||
                   order.CustomerId == currentUserId ||
                   (role == "Partner" && order.PartnerId == GetUserPartnerId(user)) ||
                   (role == "Deliverer" && order.Delivery?.DelivererId == currentUserId);
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value == "Admin";
        }

        public static bool IsAdminOrManager(this ClaimsPrincipal user)
        {
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            return role == "Admin" || role == "Manager";
        }

        public static int GetCurrentUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        public static string GetCurrentUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        private static int? GetUserPartnerId(ClaimsPrincipal user)
        {
            // Implementează logic pentru a obține Partner ID din claims sau din bază de date
            var partnerIdClaim = user.FindFirst("PartnerId")?.Value;
            return int.TryParse(partnerIdClaim, out var partnerId) ? partnerId : null;
        }
    }
}