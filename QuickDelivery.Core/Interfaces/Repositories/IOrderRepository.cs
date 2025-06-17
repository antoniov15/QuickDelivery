using QuickDelivery.Core.Entities;
using QuickDelivery.Core.Enums;

namespace QuickDelivery.Core.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id, bool includeRelated = true);
        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId);
        Task<IEnumerable<Order>> GetOrdersByDelivererIdAsync(int delivererId);
        Task<IEnumerable<Order>> GetOrdersByPartnerIdAsync(int partnerId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<Order> CreateAsync(Order order);
        Task<Order> UpdateAsync(Order order);
        Task<bool> DeleteAsync(int id);
        Task<bool> OrderExistsAsync(int id);
        Task<string> GenerateOrderNumberAsync();
    }
}