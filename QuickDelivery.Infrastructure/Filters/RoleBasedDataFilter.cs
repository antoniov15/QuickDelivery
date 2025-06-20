using QuickDelivery.Core.DTOs.Users;
using QuickDelivery.Core.DTOs.Orders;

namespace QuickDelivery.Infrastructure.Filters
{
    public class RoleBasedDataFilter
    {
        public UserDto FilterUserData(UserDto user, string requestingRole, int requestingUserId)
        {
            // Admin poate vedea toate datele
            if (requestingRole == "Admin")
                return user;

            // Utilizatorii pot vedea propriile date complete
            if (requestingUserId == user.UserId)
                return user;

            // Manager poate vedea mai multe detalii decât utilizatorii obișnuiți
            if (requestingRole == "Manager")
            {
                return new UserDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email, // Manager poate vedea email-ul
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt
                    // Nu include numărul de telefon pentru Manager
                };
            }

            // Pentru alți utilizatori - date limitate
            return new UserDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                IsActive = user.IsActive
                // Nu include informații sensibile
            };
        }

        public OrderDto FilterOrderData(OrderDto order, string requestingRole, int requestingUserId, int? requestingUserPartnerId = null)
        {
            // Admin și Manager pot vedea toate datele
            if (requestingRole == "Admin" || requestingRole == "Manager")
                return order;

            // Clientul poate vedea propriile comenzi
            if (requestingRole == "Customer" && order.CustomerId == requestingUserId)
                return order;

            // Partenerul poate vedea comenzile de la restaurantul său
            if (requestingRole == "Partner" && order.PartnerId == requestingUserPartnerId)
                return order;

            // Curierul poate vedea comenzile care îi sunt atribuite
            if (requestingRole == "Deliverer" && order.Delivery?.DelivererId == requestingUserId)
            {
                // Curierul nu trebuie să vadă informații de plată
                order.Payment = null;
                return order;
            }

            // Pentru alții - acces refuzat
            throw new UnauthorizedAccessException("Access denied to this order");
        }
    }
}