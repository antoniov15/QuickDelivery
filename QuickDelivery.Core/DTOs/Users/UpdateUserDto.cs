﻿using System.ComponentModel.DataAnnotations;
using QuickDelivery.Core.Enums;

namespace QuickDelivery.Core.DTOs.Users
{
    public class UpdateUserDto
    {
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
        [MaxLength(100, ErrorMessage = "Username cannot exceed 100 characters")]
        public string? Username { get; set; }

        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string? FirstName { get; set; }

        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsEmailVerified { get; set; }

        public UserRole? Role { get; set; }
    }
}