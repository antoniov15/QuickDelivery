using Microsoft.EntityFrameworkCore;
using QuickDelivery.Core.Entities;
using QuickDelivery.Core.Enums;
using QuickDelivery.Core.Interfaces.Repositories;
using QuickDelivery.Database;

namespace QuickDelivery.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.Partner)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.PickupAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Delivery)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id, bool includeRelated = true)
        {
            if (!includeRelated)
            {
                return await _dbContext.Orders.FindAsync(id);
            }

            return await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.Partner)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.PickupAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Delivery)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            return await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.Partner)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.PickupAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Delivery)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByPartnerIdAsync(int partnerId)
        {
            return await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.Partner)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.PickupAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Delivery)
                .Where(o => o.PartnerId == partnerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.Partner)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Delivery)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            order.UpdatedAt = DateTime.UtcNow;
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null)
                return false;

            _dbContext.Orders.Remove(order);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> OrderExistsAsync(int id)
        {
            return await _dbContext.Orders.AnyAsync(o => o.OrderId == id);
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            var today = DateTime.UtcNow;
            var prefix = today.ToString("yyyyMMdd");

            // Găsește cel mai mare număr de comandă pentru ziua curentă
            var lastOrderNumber = await _dbContext.Orders
                .Where(o => o.OrderNumber.StartsWith(prefix))
                .Select(o => o.OrderNumber)
                .OrderByDescending(n => n)
                .FirstOrDefaultAsync();

            int sequence = 1;

            if (!string.IsNullOrEmpty(lastOrderNumber) &&
                lastOrderNumber.Length > prefix.Length &&
                int.TryParse(lastOrderNumber.Substring(prefix.Length), out int lastSequence))
            {
                sequence = lastSequence + 1;
            }

            return $"{prefix}{sequence:D4}";
        }
    }
}