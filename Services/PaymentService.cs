using Chop9ja.API.Data;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.Options;
using Chop9ja.API.Models.Responses;
using Chop9ja.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayStack.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chop9ja.API.Services
{
    [AutoBuild]
    public class PaymentService : IPaymentService
    {
        #region Properties

        #region Services

        [DeepDependency]
        ILogger Logger { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IPayStackApi Paystack { get; }

        #endregion

        #region Options
        public PaystackOptions PaystackOptions => PaystackWrapper.Value;
        #endregion

        #region Internals
        [DeepDependency]
        IOptions<PaystackOptions> PaystackWrapper { get; }
        #endregion

        #endregion

        #region Methods

        public async Task<PaymentResult> UsePaystack(User user, decimal amount)
        {
            bool customerExists = Paystack.Customers.Fetch(user.Email).Status;

            if (!customerExists)
            {
                Paystack.Customers.Create(new CustomerCreateRequest()
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.PhoneNumber
                });
            }

            PaymentResult result = new PaymentResult();

            bool isRecurring = !string.IsNullOrWhiteSpace(user.Wallet.PaystackAuthorization);

            PaymentChannel channel = await DataContext.Store.GetOneAsync<PaymentChannel>(p => p.Type == ChannelType.Paystack);

            // Convert to kobo
            int total = (int)Math.Round(amount * channel.ConversionRate);

            // Handle Recurring payments
            if (isRecurring)
            {
                var success = Paystack.Transactions
                    .ChargeAuthorization(user.Wallet.PaystackAuthorization, user.Email, total, user.WalletId.ToString(), false);

                if (success.Status)
                {
                    result.Status = PaymentStatus.Success;
                    return result;
                }
            }
            
            var response = Paystack.Transactions.Initialize(user.Email, total, user.WalletId.ToString(), false);
            
            result.Status = response.Status ? PaymentStatus.Redirected : PaymentStatus.Failed;
            result.Message = response.Data.AuthorizationUrl;

            if (result.Status == PaymentStatus.Failed)
            {
                Logger.LogError("A Paystack Transaction appears to have failed. \n{0}", response.Message);
                result.Message = response.Message;
            }            

            return result;
        }

        public Task<bool> ValidatePaystackSignature(string signature, string request)
        {
            byte[] key = Encoding.UTF8.GetBytes(PaystackOptions.SecretKey);
            byte[] input = Encoding.UTF8.GetBytes(request);
            string paystackSignature = string.Empty;

            using (var hmac = new HMACSHA512(key))
            {
                byte[] hash = hmac.ComputeHash(input);
                paystackSignature = BitConverter.ToString(hash).Replace("-", string.Empty);
            }

            bool valid = paystackSignature.ToLower() == signature.ToLower();
            return Task.FromResult(valid);
        }

        public Task<bool> VerifyPaystack(User user, Transaction transaction)
        {
            var response = Paystack.Transactions.Verify(transaction.Id.ToString());
            if (!response.Status)
                Logger.LogError("An error occured while attempting to verify a transaction. \n{0}", response.Message);

            return Task.FromResult(response.Status);
        }

        public async Task<bool> ConcludePaystack(PaystackTransactionData data)
        {
            PaymentChannel channel = await DataContext.Store.GetOneAsync<PaymentChannel>(p => p.Type == ChannelType.Paystack);

            if (!decimal.TryParse(data.Amount, out decimal transactionAmount)) return false;
            if (!Guid.TryParse(data.Reference, out Guid walletId)) return false;

            Transaction transaction = new Transaction()
            {
                Amount = transactionAmount / channel.ConversionRate,
                PaymentChannel = channel,
                Type = TransactionType.Deposit
            };

            Wallet wallet = await DataContext.Store.GetByIdAsync<Wallet>(walletId);

            if (data.Authorization.Reusable && string.IsNullOrWhiteSpace(wallet.PaystackAuthorization))
            {
                wallet.PaystackAuthorization = data.Authorization.AuthorizationCode;
                await DataContext.Store.UpdateOneAsync(wallet.User);
            }

            await wallet.AddTransactionAsync(transaction);
            return true;
        }

        #endregion
    }
}
