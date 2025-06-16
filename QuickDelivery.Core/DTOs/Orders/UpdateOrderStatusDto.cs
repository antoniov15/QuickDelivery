using QuickDelivery.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuickDelivery.Core.DTOs.Orders
{
    public class UpdateOrderStatusDto
    {
        [Required(ErrorMessage = "Statusul comenzii este obligatoriu")]
        public OrderStatus Status { get; set; }

        [StringLength(500, ErrorMessage = "Notele nu pot depăși 500 de caractere")]
        public string? Notes { get; set; }

        public DateTime? EstimatedDeliveryTime { get; set; }

        public DateTime? ActualDeliveryTime { get; set; }
    }
}