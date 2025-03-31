using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using SmartRide.Models;
using SmartRide.Data;

namespace SmartRide.Services
{
    public class PaymentService
    {
        private readonly string _paypalUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly AppDbContext _context;

        public PaymentService(IConfiguration configuration, AppDbContext context)
        {
            _paypalUrl = configuration["PayPalSettings:Url"];
            _clientId = configuration["PayPalSettings:ClientId"];
            _clientSecret = configuration["PayPalSettings:Secret"];
            _context = context;
        }

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProcessPayment>> GetPaymentMethodsByPassenger(int passengerId)
        {
            return await _context.PaymentMethods
                                 .Where(pm => pm.PassengerId == passengerId)
                                 .ToListAsync();
        }

        // PayPal: Create Order
        public async Task<string> CreatePayPalOrder(decimal amount)
        {
            string accessToken = await GetPaypalAccessToken();
            string url = $"{_paypalUrl}/v2/checkout/orders";

            var payload = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new { amount = new { currency_code = "USD", value = amount.ToString("F2") } }
                }
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var parsedJson = JsonDocument.Parse(json);
                    return parsedJson.RootElement.GetProperty("id").GetString(); // Return PayPal Order ID
                }
            }
            return null;
        }

        // PayPal: Capture Order
        public async Task<bool> CapturePayPalOrder(string orderId)
        {
            string accessToken = await GetPaypalAccessToken();
            string url = $"{_paypalUrl}/v2/checkout/orders/{orderId}/capture";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.PostAsync(url, null);
                return response.IsSuccessStatusCode; // True if payment successful
            }
        }

        // Cards: Process Credit/Debit Card Payment
        public async Task<string> ProcessCardPayment(string cardNumber, string expiryDate, string cardHolderName, decimal amount)
        {
            if (string.IsNullOrEmpty(cardNumber) || string.IsNullOrEmpty(expiryDate) || string.IsNullOrEmpty(cardHolderName))
            {
                return "Invalid card details";
            }

            await Task.Delay(1000); // Simulate processing time
            return $"Transaction successful! ID: {Guid.NewGuid()}"; // Generate a unique transaction ID
        }

        // Get PayPal Access Token
        private async Task<string> GetPaypalAccessToken()
        {
            string url = $"{_paypalUrl}/v1/oauth2/token";

            using (var client = new HttpClient())
            {
                var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") });

                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var parsedJson = JsonDocument.Parse(json);
                    return parsedJson.RootElement.GetProperty("access_token").GetString();
                }
            }
            return null;
        }
    }
}