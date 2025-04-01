using Microsoft.Extensions.Logging;
using SmartRide.Data;
using SmartRide.Models;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmartRide.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _dbContext;
        private readonly EmailService _emailService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            AppDbContext dbContext,
            EmailService emailService,
            ILogger<NotificationService> logger)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task SendPaymentEmailNotificationAsync(int passengerId, string email, decimal amount, string paymentType, bool isSuccess, string message)
        {
            try
            {
                // Create notification record
                var notification = new PaymentNotif
                {
                    PassengerId = passengerId,
                    Email = email,
                    Amount = amount,
                    PaymentType = paymentType,
                    IsSuccess = isSuccess,
                    Message = message,
                    CreatedAt = DateTime.Now,
                    IsSent = false
                };

                // Save notification to database
                _dbContext.Set<PaymentNotif>().Add(notification);
                await _dbContext.SaveChangesAsync();

                // Prepare email content
                string subject = isSuccess
                    ? $"Payment Confirmation - SmartRide"
                    : $"Payment Failed - SmartRide";

                string emailContent = isSuccess
                    ? $"<h2>Payment Successful</h2>" +
                      $"<p>Thank you for your payment of {amount:C} using {paymentType}.</p>" +
                      $"<p>{message}</p>" +
                      $"<p>Thank you for choosing SmartRide!</p>"
                    : $"<h2>Payment Failed</h2>" +
                      $"<p>We were unable to process your payment of {amount:C} using {paymentType}.</p>" +
                      $"<p>{message}</p>" +
                      $"<p>Please try again or contact support if you need assistance.</p>";

                // Send email notification
                bool emailSent = await _emailService.SendPaymentNotificationAsync(
                    email, subject, emailContent);

                // Update notification status
                notification.IsSent = emailSent;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send payment notification email: {ex.Message}");
            }
        }
    }
}