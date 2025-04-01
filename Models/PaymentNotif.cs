using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartRide.Models
{
    public class PaymentNotif
    {
        public int Id { get; set; }
        public int PassengerId { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsSent { get; set; }
    }
}