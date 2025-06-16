using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuickDelivery.Core.DTOs;
using QuickDelivery.Core.DTOs.Common;
using QuickDelivery.Core.DTOs.Orders;
using QuickDelivery.Core.Interfaces.Services;

namespace QuickDelivery.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>List of all orders</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetOrdersAsync();

                _logger.LogInformation("Retrieved {Count} orders", orders.Count());

                return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(
                    orders,
                    "Orders retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders");
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving orders"));
            }
        }

        /// <summary>
        /// Get an order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order with the specified ID</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);

                if (order == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Order not found"));
                }

                _logger.LogInformation("Retrieved order with ID {OrderId}", id);

                return Ok(ApiResponse<OrderDto>.SuccessResult(
                    order,
                    "Order retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving order with ID {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving the order"));
            }
        }

        /// <summary>
        /// Get orders for a customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Orders for the customer</returns>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrdersByCustomerId(int customerId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);

                _logger.LogInformation("Retrieved {Count} orders for customer with ID {CustomerId}",
                    orders.Count(), customerId);

                return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(
                    orders,
                    "Customer orders retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders for customer with ID {CustomerId}", customerId);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving customer orders"));
            }
        }

        /// <summary>
        /// Get orders for a partner
        /// </summary>
        /// <param name="partnerId">Partner ID</param>
        /// <returns>Orders for the partner</returns>
        [HttpGet("partner/{partnerId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrdersByPartnerId(int partnerId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByPartnerIdAsync(partnerId);

                _logger.LogInformation("Retrieved {Count} orders for partner with ID {PartnerId}",
                    orders.Count(), partnerId);

                return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(
                    orders,
                    "Partner orders retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders for partner with ID {PartnerId}", partnerId);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving partner orders"));
            }
        }

        /// <summary>
        /// Get orders by status
        /// </summary>
        /// <param name="status">Order status</param>
        /// <returns>Orders with the specified status</returns>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrdersByStatus(int status)
        {
            try
            {
                var orders = await _orderService.GetOrdersByStatusAsync((Core.Enums.OrderStatus)status);

                _logger.LogInformation("Retrieved {Count} orders with status {Status}",
                    orders.Count(), (Core.Enums.OrderStatus)status);

                return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(
                    orders,
                    "Orders retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders with status {Status}", (Core.Enums.OrderStatus)status);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving orders"));
            }
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="createOrderDto">Order data</param>
        /// <returns>Created order</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                // Validate model
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid order data", errors));
                }

                var order = await _orderService.CreateOrderAsync(createOrderDto);

                _logger.LogInformation("Created order with number {OrderNumber} for customer {CustomerId}",
                    order.OrderNumber, order.CustomerId);

                return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId },
                    ApiResponse<OrderDto>.SuccessResult(
                        order,
                        "Order created successfully"
                    ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating order: {@CreateOrderDto}", createOrderDto);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while creating the order"));
            }
        }

        /// <summary>
        /// Update an order's status
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="updateOrderDto">Updated data</param>
        /// <returns>Updated order</returns>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto updateOrderDto)
        {
            try
            {
                // Validate model
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid order status update data", errors));
                }

                var order = await _orderService.UpdateOrderStatusAsync(id, updateOrderDto);

                if (order == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Order not found"));
                }

                _logger.LogInformation("Updated status for order {OrderId} to {Status}",
                    id, updateOrderDto.Status);

                return Ok(ApiResponse<OrderDto>.SuccessResult(
                    order,
                    "Order status updated successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating order status for order {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while updating the order status"));
            }
        }

        /// <summary>
        /// Delete an order
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Operation status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteOrder(int id)
        {
            try
            {
                var result = await _orderService.DeleteOrderAsync(id);

                if (!result)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Order not found"));
                }

                _logger.LogInformation("Deleted order with ID {OrderId}", id);

                return Ok(ApiResponse<object>.SuccessResult("Order deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting order with ID {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while deleting the order"));
            }
        }
    }
}