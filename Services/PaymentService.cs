using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartRide.Data;
using SmartRide.Models;
using System;

namespace SmartRide.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _dbContext;

        public PaymentService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get all saved payment methods for a passenger (from DB)
        public async Task<List<PaymentMethods>> GetPaymentMethodsByPassenger(int passengerId)
        {
            return await _dbContext.PaymentMethods
                                   .Where(pm => pm.PassengerId == passengerId)
                                   .ToListAsync();
        }

        // Get specific payment method by ID (from DB)
        public async Task<PaymentMethods> GetPaymentMethodById(int passengerId, int paymentMethodId)
        {
            return await _dbContext.PaymentMethods
                                   .FirstOrDefaultAsync(pm => pm.PassengerId == passengerId && pm.PaymentMethodId == paymentMethodId);
        }

        // Process Credit/Debit Card Payment (Simulate Payment Gateway)
        public async Task<string> ProcessCardPayment(string cardNumber, string expiryDate, string cardHolderName, decimal amount)
        {
            // Simulate external payment gateway processing
            await Task.Delay(1000); // Simulate API response time
            return $"Payment of {amount:C} processed successfully using {cardHolderName}'s card.";
        }

        // Get passenger by ID to retrieve their email
        public async Task<Passengers> GetPassengerById(int passengerId)
        {
            return await _dbContext.Set<Passengers>()
                .FirstOrDefaultAsync(p => p.PassengerId == passengerId);
        }

        // Save successful payment to Invoices table
        public async Task SaveInvoice(int rideId, decimal amount, int paymentMethodId, string paymentStatus)
        {
            var invoice = new Invoices
            {
                RideId = rideId,
                Amount = amount,
                PaymentMethodId = paymentMethodId,
                PaymentStatus = paymentStatus,
                IssuedAt = DateTime.UtcNow
            };
            _dbContext.Set<Invoices>().Add(invoice);
            await _dbContext.SaveChangesAsync();
        }
    }
}