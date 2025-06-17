using QuickDelivery.Core.DTOs.Common;
using QuickDelivery.Core.DTOs.Orders;
using QuickDelivery.Core.Entities;
using QuickDelivery.Core.Enums;

namespace QuickDelivery.Core.Interfaces.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId);
        Task<IEnumerable<OrderDto>> GetOrdersByDelivererIdAsync(int delivererId);
        Task<IEnumerable<OrderDto>> GetOrdersByPartnerIdAsync(int partnerId);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto);
        Task<OrderDto?> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto updateOrderDto);
        Task<bool> DeleteOrderAsync(int id);
    }
}