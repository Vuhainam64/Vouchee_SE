﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Models.DTOs
{
    public class UserDTO
    {
        public string? name { get; set; }
        public string? phoneNumber { get; set; }
        public string? image { get; set; }
        public string? bankName { get; set; }
        public string? bankAccount { get; set; }
    }

    public class CreateUserDTO : UserDTO
    {
        public string? email { get; set; }
        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateUserDTO : UserDTO
    {
        public DateTime? updateDate = DateTime.Now;
    }

    public class GetUserDTO : UserDTO
    {
        public GetUserDTO()
        {
            orders = [];
            vouchers = [];
            carts = [];
            notificationFromUser = [];
            notificationToUser = [];
            wallets = [];
        }

        public Guid? id { get; set; }
        public Guid? roleId { get; set; }
        public string? email { get; set; }

        public string? roleName { get; set; }
        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }

        public virtual ICollection<CartDTO> carts { get; set; }
        public virtual ICollection<GetVoucherDTO> vouchers { get; set; }
        public virtual ICollection<GetOrderDTO>? orders { get; set; }
        public virtual ICollection<GetNotificationDTO> notificationToUser { get; set; }
        [InverseProperty(nameof(Notification.Sender))]
        public virtual ICollection<GetNotificationDTO> notificationFromUser { get; set; }
        public virtual ICollection<GetWalletDTO> wallets { get; set; }
    }

    public class RegisterDTO
    {
        public string? phoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string? hashPassword { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string? name { get; set; }
    }

    public class LoginByEmailDTO
    {
        public string? email { get; set; }
        public string? hashPassword { get; set; }
    }

    public class LoginByPhoneNumberDTO
    {
        public string? phoneNumber { get; set; }
        public string? password { get; set; }
    }
}
